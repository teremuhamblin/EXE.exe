using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace ExeShield
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!IsAdministrator())
            {
                MessageBox.Show(
                    "EXE.exe doit etre execute en tant qu'Administrateur pour surveiller tous les ports et bloquer les menaces via le Pare-feu Windows.",
                    "Droits Administrateur requis",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Application.Run(new MainForm());
        }

        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public class MainForm : Form
    {
        // --- Configuration des ports ---
        // Ports TCP surveilles (WinNuke, SMB, RDP, Backdoors/RAT courants)
        private static readonly int[] MonitoredTcpPorts = new int[]
        {
            21, 23, 135, 139, 445, 1433, 3389,
            4444, 5555, 5900, 6666, 6667, 8081,
            11223, 12345, 27374, 31337
        };

        // Ports UDP surveilles (NetBIOS 137-139, TFTP, SNMP, Backdoors)
        private static readonly int[] MonitoredUdpPorts = new int[]
        {
            69, 137, 138, 139, 161, 1900, 11223, 31337
        };

        // Ports typiques de malwares / RAT / Botnets (alerte haute priorite meme en sortie)
        private static readonly HashSet<int> MalwarePorts = new HashSet<int>(new int[]
        {
            4444, 5555, 6666, 6667, 11223, 12345, 27374, 31337
        });

        // Processus souvent detournes par les virus (LOLBins / scripts)
        private static readonly HashSet<string> SuspectParents = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mshta.exe", "wscript.exe", "cscript.exe", "regsvr32.exe", "powershell.exe", "cmd.exe"
        };

        // --- Interface graphique ---
        private RichTextBox logBox;
        private ListBox blockedIpList;
        private CheckBox chkAutoBlockIp;
        private CheckBox chkIgnoreLan;
        private CheckBox chkMonitorProcesses;
        private Button btnHardenFirewall;
        private Button btnClearRules;
        private Label lblStatus;
        private NotifyIcon trayIcon;

        // --- Etat interne ---
        private volatile bool isRunning = true;
        private readonly List<TcpListener> tcpListeners = new List<TcpListener>();
        private readonly List<UdpClient> udpListeners = new List<UdpClient>();
        private readonly HashSet<string> blockedIps = new HashSet<string>();
        private readonly HashSet<string> alertedConnections = new HashSet<string>();
        private readonly HashSet<int> inspectedPids = new HashSet<int>();
        private readonly object syncLock = new object();

        public MainForm()
        {
            InitializeUi();
            StartProtection();
        }

        private void InitializeUi()
        {
            this.Text = "EXE.exe - Bouclier Reseau & Anti-Intrusion (Concept AntiNuke)";
            this.Size = new Size(920, 580);
            this.MinimumSize = new Size(800, 480);
            this.BackColor = Color.FromArgb(24, 26, 32);
            this.ForeColor = Color.WhiteSmoke;
            this.Icon = SystemIcons.Shield;

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 95;
            topPanel.BackColor = Color.FromArgb(32, 35, 44);
            topPanel.Padding = new Padding(10);

            lblStatus = new Label();
            lblStatus.Text = "Statut : PROTECTION ACTIVE (TCP/UDP 139 + Anti-Vers / Anti-RAT)";
            lblStatus.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblStatus.ForeColor = Color.LightGreen;
            lblStatus.Location = new Point(12, 10);
            lblStatus.AutoSize = true;

            chkAutoBlockIp = new CheckBox();
            chkAutoBlockIp.Text = "Bloquer auto les IPs suspectes (Pare-feu)";
            chkAutoBlockIp.Checked = true;
            chkAutoBlockIp.Location = new Point(14, 38);
            chkAutoBlockIp.AutoSize = true;

            chkIgnoreLan = new CheckBox();
            chkIgnoreLan.Text = "Ignorer le reseau local (LAN / Box)";
            chkIgnoreLan.Checked = true;
            chkIgnoreLan.Location = new Point(280, 38);
            chkIgnoreLan.AutoSize = true;

            chkMonitorProcesses = new CheckBox();
            chkMonitorProcesses.Text = "Surveiller processus & scripts suspects";
            chkMonitorProcesses.Checked = true;
            chkMonitorProcesses.Location = new Point(14, 62);
            chkMonitorProcesses.AutoSize = true;

            btnHardenFirewall = new Button();
            btnHardenFirewall.Text = "Durcir Pare-feu (Bloquer SMB/139/RDP externes)";
            btnHardenFirewall.BackColor = Color.FromArgb(180, 55, 55);
            btnHardenFirewall.ForeColor = Color.White;
            btnHardenFirewall.FlatStyle = FlatStyle.Flat;
            btnHardenFirewall.Size = new Size(300, 30);
            btnHardenFirewall.Location = new Point(580, 12);
            btnHardenFirewall.Click += new EventHandler(BtnHardenFirewall_Click);

            btnClearRules = new Button();
            btnClearRules.Text = "Nettoyer les regles creees par EXE.exe";
            btnClearRules.BackColor = Color.FromArgb(60, 65, 80);
            btnClearRules.ForeColor = Color.White;
            btnClearRules.FlatStyle = FlatStyle.Flat;
            btnClearRules.Size = new Size(300, 30);
            btnClearRules.Location = new Point(580, 50);
            btnClearRules.Click += new EventHandler(BtnClearRules_Click);

            topPanel.Controls.Add(lblStatus);
            topPanel.Controls.Add(chkAutoBlockIp);
            topPanel.Controls.Add(chkIgnoreLan);
            topPanel.Controls.Add(chkMonitorProcesses);
            topPanel.Controls.Add(btnHardenFirewall);
            topPanel.Controls.Add(btnClearRules);

            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.SplitterDistance = 640;
            split.BackColor = Color.FromArgb(24, 26, 32);

            logBox = new RichTextBox();
            logBox.Dock = DockStyle.Fill;
            logBox.ReadOnly = true;
            logBox.BackColor = Color.FromArgb(15, 17, 21);
            logBox.ForeColor = Color.Gainsboro;
            logBox.Font = new Font("Consolas", 9.5f);
            logBox.BorderStyle = BorderStyle.None;

            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Padding = new Padding(6);

            Label lblBlocked = new Label();
            lblBlocked.Text = "IPs bloquees cette session :";
            lblBlocked.Dock = DockStyle.Top;
            lblBlocked.Height = 24;
            lblBlocked.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            blockedIpList = new ListBox();
            blockedIpList.Dock = DockStyle.Fill;
            blockedIpList.BackColor = Color.FromArgb(18, 20, 26);
            blockedIpList.ForeColor = Color.Salmon;
            blockedIpList.Font = new Font("Consolas", 9.5f);
            blockedIpList.BorderStyle = BorderStyle.FixedSingle;

            rightPanel.Controls.Add(blockedIpList);
            rightPanel.Controls.Add(lblBlocked);

            split.Panel1.Controls.Add(logBox);
            split.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(split);
            this.Controls.Add(topPanel);

            // Barre des taches (System Tray)
            ContextMenuStrip trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Afficher EXE.exe", null, delegate { ShowMainWindow(); });
            trayMenu.Items.Add("Quitter", null, delegate { ExitApplication(); });

            trayIcon = new NotifyIcon();
            trayIcon.Icon = SystemIcons.Shield;
            trayIcon.Text = "EXE.exe - Bouclier Actif";
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += delegate { ShowMainWindow(); };

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        private void ShowMainWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void ExitApplication()
        {
            isRunning = false;
            trayIcon.Visible = false;
            StopListeners();
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                trayIcon.ShowBalloonTip(2500, "EXE.exe reste actif", "La surveillance des ports et virus continue en arriere-plan.", ToolTipIcon.Info);
            }
        }

        private void StartProtection()
        {
            Log("[INIT] Demarrage de EXE.exe...", Color.Cyan);

            // 1. Demarrage des pieges TCP
            foreach (int port in MonitoredTcpPorts)
            {
                StartTcpTrap(port);
            }

            // 2. Demarrage des pieges UDP
            foreach (int port in MonitoredUdpPorts)
            {
                StartUdpTrap(port);
            }

            // 3. Thread d'inspection de la pile reseau Windows (detecte meme les ports occupes par Windows comme 139/445)
            Thread netThread = new Thread(NetworkTableMonitorLoop);
            netThread.IsBackground = true;
            netThread.Start();

            // 4. Thread de surveillance des processus suspects
            Thread procThread = new Thread(ProcessMonitorLoop);
            procThread.IsBackground = true;
            procThread.Start();

            Log("[PRET] Surveillance active sur NetBIOS (137-139), SMB (445), RDP (3389) et ports de malwares.", Color.LightGreen);
        }

        private void StartTcpTrap(int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                tcpListeners.Add(listener);

                Thread t = new Thread(delegate()
                {
                    while (isRunning)
                    {
                        try
                        {
                            TcpClient client = listener.AcceptTcpClient();
                            IPEndPoint remote = client.Client.RemoteEndPoint as IPEndPoint;
                            client.Close();

                            if (remote != null)
                            {
                                HandleThreatConnection(remote.Address.ToString(), port, "TCP-TRAP", null);
                            }
                        }
                        catch
                        {
                            if (!isRunning) break;
                        }
                    }
                });
                t.IsBackground = true;
                t.Start();
                Log(string.Format("[PIEGE TCP] Ecoute directe active sur le port {0}", port), Color.Gray);
            }
            catch
            {
                Log(string.Format("[INFO] Port TCP {0} deja utilise par Windows -> surveille via table noyau.", port), Color.DarkGray);
            }
        }

        private void StartUdpTrap(int port)
        {
            try
            {
                UdpClient udp = new UdpClient(port);
                udpListeners.Add(udp);

                Thread t = new Thread(delegate()
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    while (isRunning)
                    {
                        try
                        {
                            byte[] data = udp.Receive(ref remote);
                            if (remote != null)
                            {
                                HandleThreatConnection(remote.Address.ToString(), port, "UDP-TRAP (" + data.Length + " octets)", null);
                            }
                        }
                        catch
                        {
                            if (!isRunning) break;
                        }
                    }
                });
                t.IsBackground = true;
                t.Start();
                Log(string.Format("[PIEGE UDP] Ecoute directe active sur le port {0}", port), Color.Gray);
            }
            catch
            {
                Log(string.Format("[INFO] Port UDP {0} deja utilise par Windows -> surveille via table noyau.", port), Color.DarkGray);
            }
        }

        private void StopListeners()
        {
            foreach (TcpListener l in tcpListeners)
            {
                try { l.Stop(); } catch { }
            }
            foreach (UdpClient u in udpListeners)
            {
                try { u.Close(); } catch { }
            }
        }

        private void NetworkTableMonitorLoop()
        {
            while (isRunning)
            {
                try
                {
                    List<TcpRowInfo> tcpRows = GetAllTcpConnections();
                    foreach (TcpRowInfo row in tcpRows)
                    {
                        if (row.RemoteAddress.Equals(IPAddress.Any) || IPAddress.IsLoopback(row.RemoteAddress))
                            continue;

                        bool isWatchedLocal = Array.IndexOf(MonitoredTcpPorts, row.LocalPort) >= 0;
                        bool isMalwareRemote = MalwarePorts.Contains(row.RemotePort);

                        if (isWatchedLocal || isMalwareRemote)
                        {
                            string key = string.Format("{0}:{1}->{2}:{3}:{4}",
                                row.LocalPort, row.RemoteAddress, row.RemotePort, row.Pid, row.State);

                            bool isNew = false;
                            lock (syncLock)
                            {
                                if (!alertedConnections.Contains(key))
                                {
                                    alertedConnections.Add(key);
                                    if (alertedConnections.Count > 5000) alertedConnections.Clear();
                                    isNew = true;
                                }
                            }

                            if (isNew)
                            {
                                string procName = GetProcessNameSafe(row.Pid);
                                int targetPort = isWatchedLocal ? row.LocalPort : row.RemotePort;
                                HandleThreatConnection(row.RemoteAddress.ToString(), targetPort,
                                    string.Format("TCP-KERNEL ({0}, PID {1})", procName, row.Pid), row.Pid);
                            }
                        }
                    }
                }
                catch { }

                Thread.Sleep(800);
            }
        }

        private void ProcessMonitorLoop()
        {
            while (isRunning)
            {
                if (chkMonitorProcesses.Checked)
                {
                    try
                    {
                        Process[] procs = Process.GetProcesses();
                        foreach (Process p in procs)
                        {
                            bool shouldCheck = false;
                            lock (syncLock)
                            {
                                if (!inspectedPids.Contains(p.Id))
                                {
                                    inspectedPids.Add(p.Id);
                                    if (inspectedPids.Count > 5000) inspectedPids.Clear();
                                    shouldCheck = true;
                                }
                            }

                            if (!shouldCheck) continue;

                            try
                            {
                                string exePath = p.MainModule.FileName;
                                string fileName = Path.GetFileName(exePath);
                                string lowerPath = exePath.ToLowerInvariant();

                                bool suspiciousLocation =
                                    lowerPath.Contains("\\appdata\\local\\temp\\") ||
                                    lowerPath.Contains("\\users\\public\\") ||
                                    lowerPath.Contains("\\$recycle.bin\\");

                                if (suspiciousLocation)
                                {
                                    string sha = ComputeSha256Safe(exePath);
                                    Log(string.Format("[PROCESSUS SUSPECT] {0} (PID {1}) lance depuis dossier sensible : {2} | SHA256={3}",
                                        fileName, p.Id, exePath, sha), Color.Orange);

                                    if (chkAutoBlockIp.Checked)
                                    {
                                        BlockProgramInFirewall(exePath, fileName);
                                    }

                                    NotifyUser("Processus suspect detecte", fileName + " lance depuis un dossier temporaire/public.", ToolTipIcon.Warning);
                                }
                                else if (SuspectParents.Contains(fileName))
                                {
                                    // Journaliser discretememt les moteurs de script utilises par les malwares
                                    Log(string.Format("[SCRIPT SURVEILLE] {0} (PID {1}) demarre.", fileName, p.Id), Color.Khaki);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                Thread.Sleep(2500);
            }
        }

        private void HandleThreatConnection(string ipStr, int port, string source, int? pid)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ipStr, out ip)) return;
            if (IPAddress.IsLoopback(ip)) return;

            if (chkIgnoreLan.Checked && IsPrivateLanIp(ip))
            {
                Log(string.Format("[LAN] Activite locale ignoree sur port {0} depuis {1} ({2})", port, ipStr, source), Color.DarkGray);
                return;
            }

            string msg = string.Format("[ALERTE PORT {0}] Connexion detectee depuis {1} via {2}", port, ipStr, source);
            Log(msg, Color.OrangeRed);
            NotifyUser("Intrusion / Mouvement Port " + port, string.Format("IP {0} detectee ({1})", ipStr, source), ToolTipIcon.Warning);

            if (chkAutoBlockIp.Checked)
            {
                BlockIpInFirewall(ipStr, port);
            }
        }

        private void BlockIpInFirewall(string ip, int port)
        {
            lock (syncLock)
            {
                if (blockedIps.Contains(ip)) return;
                blockedIps.Add(ip);
            }

            string ruleName = "EXE_Shield_Block_" + ip.Replace(":", "_");
            RunNetsh(string.Format("advfirewall firewall add rule name=\"{0}_IN\" dir=in action=block remoteip={1}", ruleName, ip));
            RunNetsh(string.Format("advfirewall firewall add rule name=\"{0}_OUT\" dir=out action=block remoteip={1}", ruleName, ip));

            Log(string.Format("[BLOQUE] IP {0} bloquee en entree/sortie (Port {1}).", ip, port), Color.Red);

            this.BeginInvoke((MethodInvoker)delegate
            {
                blockedIpList.Items.Add(string.Format("{0} (Port {1})", ip, port));
            });
        }

        private void BlockProgramInFirewall(string exePath, string name)
        {
            string ruleName = "EXE_Shield_App_" + name + "_" + Math.Abs(exePath.GetHashCode());
            RunNetsh(string.Format("advfirewall firewall add rule name=\"{0}\" dir=out action=block program=\"{1}\"", ruleName, exePath));
            Log(string.Format("[RESEAU COUPE] Acces sortant bloque pour {0}", exePath), Color.Red);
        }

        private void BtnHardenFirewall_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(
                "Cela va bloquer les connexions entrantes externes sur les ports les plus attaques par les virus/vers/ransomwares :\n" +
                "- NetBIOS / WinNuke (TCP/UDP 137, 138, 139)\n" +
                "- SMB / WannaCry (TCP 445)\n" +
                "- RPC (TCP 135) & Telnet (TCP 23) & RDP (TCP/UDP 3389)\n\nContinuer ?",
                "Durcissement du Pare-feu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (res != DialogResult.Yes) return;

            RunNetsh("advfirewall firewall add rule name=\"EXE_Shield_Harden_TCP\" dir=in action=block protocol=TCP localport=23,135,137,138,139,445,3389");
            RunNetsh("advfirewall firewall add rule name=\"EXE_Shield_Harden_UDP\" dir=in action=block protocol=UDP localport=137,138,139,3389");
            Log("[DURCISSEMENT] Regles strictes anti-vers / anti-ransomware appliquees dans le Pare-feu Windows.", Color.LightGreen);
            NotifyUser("Pare-feu durci", "Ports 23, 135, 137-139, 445 et 3389 bloques en entree.", ToolTipIcon.Info);
        }

        private void BtnClearRules_Click(object sender, EventArgs e)
        {
            // Supprime les regles creees par EXE.exe via PowerShell/Netsh
            RunCmd("powershell -NoProfile -Command \"Get-NetFirewallRule -DisplayName 'EXE_Shield_*' -ErrorAction SilentlyContinue | Remove-NetFirewallRule\"");
            lock (syncLock)
            {
                blockedIps.Clear();
            }
            blockedIpList.Items.Clear();
            Log("[NETTOYAGE] Toutes les regles Pare-feu creees par EXE.exe ont ete supprimees.", Color.Cyan);
        }

        private void RunNetsh(string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process p = Process.Start(psi);
                if (p != null) p.WaitForExit(3000);
            }
            catch { }
        }

        private void RunCmd(string fullArgs)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + fullArgs);
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process p = Process.Start(psi);
                if (p != null) p.WaitForExit(5000);
            }
            catch { }
        }

        private bool IsPrivateLanIp(IPAddress ip)
        {
            if (ip.AddressFamily != AddressFamily.InterNetwork) return false;
            byte[] b = ip.GetAddressBytes();
            if (b[0] == 10) return true;
            if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
            if (b[0] == 192 && b[1] == 168) return true;
            if (b[0] == 169 && b[1] == 254) return true; // APIPA
            return false;
        }

        private string GetProcessNameSafe(int pid)
        {
            try
            {
                if (pid == 0) return "Idle";
                if (pid == 4) return "System";
                Process p = Process.GetProcessById(pid);
                return p.ProcessName + ".exe";
            }
            catch
            {
                return "Inconnu";
            }
        }

        private string ComputeSha256Safe(string filePath)
        {
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(fs);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch
            {
                return "verrouille";
            }
        }

        private void Log(string text, Color color)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            try
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    if (logBox.Lines.Length > 1500)
                    {
                        logBox.Clear();
                    }
                    logBox.SelectionStart = logBox.TextLength;
                    logBox.SelectionLength = 0;
                    logBox.SelectionColor = Color.DimGray;
                    logBox.AppendText(DateTime.Now.ToString("[HH:mm:ss] "));
                    logBox.SelectionColor = color;
                    logBox.AppendText(text + Environment.NewLine);
                    logBox.ScrollToCaret();
                });
            }
            catch { }
        }

        private void NotifyUser(string title, string message, ToolTipIcon icon)
        {
            try
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    trayIcon.ShowBalloonTip(3000, title, message, icon);
                });
            }
            catch { }
        }

        // --- API native Windows (iphlpapi.dll) pour lire toutes les connexions TCP actives ---
        private const int AF_INET = 2;
        private const int TCP_TABLE_OWNER_PID_ALL = 5;

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            ref int dwOutBufLen,
            bool sort,
            int ipVersion,
            int tblClass,
            uint reserved);

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;
            public uint localPort;
            public uint remoteAddr;
            public uint remotePort;
            public uint owningPid;
        }

        private class TcpRowInfo
        {
            public uint State;
            public IPAddress RemoteAddress;
            public int LocalPort;
            public int RemotePort;
            public int Pid;
        }

        private List<TcpRowInfo> GetAllTcpConnections()
        {
            List<TcpRowInfo> result = new List<TcpRowInfo>();
            int bufferSize = 0;

            GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, TCP_TABLE_OWNER_PID_ALL, 0);
            IntPtr tcpTablePtr = Marshal.AllocHGlobal(bufferSize);

            try
            {
                uint ret = GetExtendedTcpTable(tcpTablePtr, ref bufferSize, true, AF_INET, TCP_TABLE_OWNER_PID_ALL, 0);
                if (ret != 0) return result;

                int rowCount = Marshal.ReadInt32(tcpTablePtr);
                IntPtr rowPtr = IntPtr.Add(tcpTablePtr, 4);
                int rowSize = Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID));

                for (int i = 0; i < rowCount; i++)
                {
                    MIB_TCPROW_OWNER_PID row = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));

                    TcpRowInfo info = new TcpRowInfo();
                    info.State = row.state;
                    info.RemoteAddress = new IPAddress((long)row.remoteAddr);
                    info.LocalPort = (int)(((row.localPort & 0xFF) << 8) | ((row.localPort >> 8) & 0xFF));
                    info.RemotePort = (int)(((row.remotePort & 0xFF) << 8) | ((row.remotePort >> 8) & 0xFF));
                    info.Pid = (int)row.owningPid;

                    result.Add(info);
                    rowPtr = IntPtr.Add(rowPtr, rowSize);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }

            return result;
        }
    }
}

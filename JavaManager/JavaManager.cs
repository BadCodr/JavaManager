using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;


namespace JavaManager
{
    public class JavaInstallerManager
    {
        private string latestVersion;
        WebBrowser webBrowser;
        bool debug;

        public void JavaManagerInit(bool _debug)
        {
            debug = _debug;
            WebBrowserInit();
            WebBrowserStartPhaseOne();
            Main();
        }

        private void WebBrowserInit()
        {
            Console.WriteLine("Initializing Webbrowser...");
            webBrowser = new WebBrowser();
            RegistryKey Regkey = null;

            string processName = Process.GetCurrentProcess().ProcessName + ".exe";
            Regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
            string processID = Convert.ToString(Regkey.GetValue(processName));

            if (processID == "8000")
            {
                Regkey.Close();
                return;
            }

            Regkey.SetValue(processName, unchecked((int)0x1F40), RegistryValueKind.DWord);
            Regkey.Close();
        }

        private void WebBrowserStartPhaseOne()
        {
            Console.Write("Finding Latest Java Version...");
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(PhaseOneWebComplete);
            webBrowser.ScriptErrorsSuppressed = true;

            webBrowser.Navigate(new Uri("https://java.com/inc/BrowserRedirect1.jsp?locale=en"));
            while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }

        private void Main()
        {
            bool is64Bit = false;
            string version = "";
            Console.Write("Checking if Java is installed...\n");
            if (IsJavaInstalled(out is64Bit))
            {
                JavaInstallDirectory(is64Bit, out version);
                {
                    Console.Write(" SUCCESS \nVERSION: {0} is currently installed\n", version);
                    if (version != latestVersion)
                        WebBrowserStartPhaseTwo();
                    else
                        Console.WriteLine("Latest Version Is Already Installed {0}", debug ? version + ' ' + latestVersion + " is the same" : "");
                }
            }

            else
                WebBrowserStartPhaseTwo();
        }

        private bool IsJavaInstalled(out bool is64Bit)
        {
                RegistryKey javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\JavaSoft\Java Runtime Environment");

            if (javaRegistryKey != null)
            {
                is64Bit = true;
                javaRegistryKey.Close();
                return true;
            }

            else
            {
                javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\JavaSoft\Java Runtime Environment");
                if (javaRegistryKey != null)
                {
                    is64Bit = false;
                    javaRegistryKey.Close();
                    return true;
                }
                else
                {
                    is64Bit = Environment.Is64BitOperatingSystem;
                    return false;
                }
            }
        }

        private string JavaInstallDirectory(bool is64Bit, out string version)
        {
            RegistryKey javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)(is64Bit ? 256 : 512)).OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment");
            string currentVerion = javaRegistryKey.GetValue("CurrentVersion").ToString();
            
            javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)(is64Bit ? 256 : 512)).OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\"+currentVerion);
            string installDirectory = javaRegistryKey.GetValue("JavaHome").ToString();

            var temp = installDirectory.Split("\\".ToCharArray());
            var temp2 = temp[temp.Length - 1].Remove(0,5).Replace('_','u');
            temp2 = temp2.Remove(temp2.IndexOf('.'), 2);

            version = temp2;
            return installDirectory;
        }        

        private void WebBrowserStartPhaseTwo()
        {
            Console.WriteLine("New Java Version Available...");
            Console.Write("Downloading");
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(PhaseTwoWebComplete);
            webBrowser.ScriptErrorsSuppressed = true;

            webBrowser.Navigate(new Uri("https://java.com/en/download/manual.jsp"));
            while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }

        private void PhaseOneWebComplete(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(PhaseOneWebComplete);
            string s = "";

            foreach (HtmlElement htmlElement in webBrowser.Document.Body.All)
            {
                if (htmlElement.GetAttribute("className") == "sub")
                    s = htmlElement.InnerText;

            }

            s = s.Split('(')[0].Replace(" Update ", "u").Split(' ')[2];

            latestVersion = s;
            Console.Write(" {0}\n", latestVersion);
        }

        private void PhaseTwoWebComplete(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Console.WriteLine(" Version {0}", latestVersion);
            webBrowser.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(PhaseTwoWebComplete);
            List<string> javaDownload = new List<string>();

            foreach (HtmlElement htmlElement in webBrowser.Document.Links)
            {
                string s = htmlElement.GetAttribute("HREF").ToString();
                if (s.Contains("AutoDL"))
                {
                    if (!javaDownload.Contains(s))
                        javaDownload.Add(s);
                }
            }
            Console.WriteLine("Obtained Download Link...");
            webBrowser.Dispose();
            System.Net.WebClient webClient = new System.Net.WebClient();

            Console.WriteLine("Downloading Java Version {0} for {1} bit system", latestVersion, Environment.Is64BitOperatingSystem ? "64" : "32");
            Console.WriteLine("Please Wait Download In Progress...");
            
            if (Environment.Is64BitOperatingSystem)
                webClient.DownloadFile(javaDownload[2], System.IO.Path.GetTempPath() + latestVersion + ".exe");
            else
                webClient.DownloadFile(javaDownload[1], System.IO.Path.GetTempPath() + latestVersion + ".exe");

            while (webClient.IsBusy)
            {
                
                Application.DoEvents();
            }
            Console.WriteLine("Downoaded latest Java Version...");
            Console.WriteLine("Proceeding To Install Java Version {0}", latestVersion);

            InstallJava(System.IO.Path.GetTempPath() + latestVersion + ".exe");
        }

        private void InstallJava(string file)
        {
            try
            {
                var process = Process.Start(file, "/s");
                Console.WriteLine("Installing Java...");
                Console.WriteLine("You May Now Close This Window");
                Console.ReadLine();
            }
            catch (Win32Exception)
            {
                throw;
            }

        }
    }
}

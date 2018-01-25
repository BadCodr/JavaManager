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
    public partial class Form1 : Form
    {
        private string latestVersion;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WebBrowserInit();
            WebBrowserStartPhaseOne();
            Main();
        }
        

        private void PhaseOneWebComplete(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(PhaseOneWebComplete);
            string s = "";
            foreach (HtmlElement htmlElement in webBrowser1.Document.Body.All)
            {
                if (htmlElement.GetAttribute("className") == "sub")
                    s = htmlElement.InnerText;

            }
            s = s.Split('(')[0].Replace(" Update ", "u").Split(' ')[2];

            latestVersion = s;
        }

        private void PhaseTwoWebComplete(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(PhaseTwoWebComplete);
            string javaDownload="";
            foreach (HtmlElement htmlElement in webBrowser1.Document.Links)
            {
                string s = htmlElement.GetAttribute("HREF").ToString();
                if (s.Contains("AutoDL"))
                {
                    javaDownload = s;
                    break;
                }
            }
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.DownloadFile(javaDownload, System.IO.Path.GetTempPath() + latestVersion+".exe");

            while (webClient.IsBusy)
                ;
            InstallJava(System.IO.Path.GetTempPath() + latestVersion + ".exe");
        }

        private void InstallJava(string file)
        {
            Process.Start(file, "/s");
        }

        private void Main()
        {
            bool is64Bit;
            string version;


            if (isJavaInstalled(out is64Bit))
            {
                JavaInstallDirectory(is64Bit, out version);
                if (version == null)
                    textBox1.Text = "something is wrong...";//                    WebBrowserStartPhaseTwo();
                else if (version != latestVersion)
                    WebBrowserStartPhaseTwo();
            } 
            else
            {
                WebBrowserStartPhaseTwo();

            }
        }

        private bool isJavaInstalled(out bool is64Bit)
        {
            
            RegistryKey javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\JavaSoft\Java Runtime Environment");

            if (javaRegistryKey != null)
            {
                is64Bit = true;
                return true;
            }

            else
            {
                javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\JavaSoft\Java Runtime Environment");
                if (javaRegistryKey != null)
                {
                    is64Bit = false;
                    return true;
                }
            }

            is64Bit = Environment.Is64BitOperatingSystem;
            return false;
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

        private void WebBrowserInit()
        {
            string processName = Process.GetCurrentProcess().ProcessName + ".exe";
            RegistryKey Regkey = null;


            Regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);

            string processID = Convert.ToString(Regkey.GetValue(processName));
            
            if(processID=="8000")
            {
                Regkey.Close();
                return;
            }

            Regkey.SetValue(processName, unchecked((int)0x1F40), RegistryValueKind.DWord);

            Regkey.Close();

        }

        private void WebBrowserStartPhaseOne()
        {
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(PhaseOneWebComplete);
            webBrowser1.ScriptErrorsSuppressed = true;

            webBrowser1.Navigate(new Uri("https://java.com/inc/BrowserRedirect1.jsp?locale=en"));
   /*         while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            } */

        }

        private void WebBrowserStartPhaseTwo()
        {
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(PhaseTwoWebComplete);
            webBrowser1.ScriptErrorsSuppressed = true;

            webBrowser1.Navigate(new Uri("https://java.com/inc/BrowserRedirect1.jsp?locale=en"));
            // download latest version XPATH?
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }
    }
}

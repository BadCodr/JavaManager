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
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WebBrowserInit();
            Main();
            
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocoCompleted);
            webBrowser1.ScriptErrorsSuppressed = true;

            webBrowser1.Navigate(new Uri("https://java.com/inc/BrowserRedirect1.jsp?locale=en"));
        }
        

        private void DocoCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            textBox1.Text = webBrowser1.Url.ToString();


        }

        private void Main()
        {
            bool is64Bit;
            string version;


            if (isJavaInstalled(out is64Bit))
                textBox1.Text = JavaInstallDirectory(is64Bit, out version);
            
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
            version = currentVerion;

            javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)(is64Bit ? 256 : 512)).OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\"+currentVerion);
            string installDirectory = javaRegistryKey.GetValue("JavaHome").ToString();

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

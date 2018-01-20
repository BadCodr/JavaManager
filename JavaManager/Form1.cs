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
            Main();
        }

        private void Main()
        {
            if(isJavaInstalled(out bool is64Bit))
                textBox1.Text = JavaInstallDirectory(is64Bit);
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



        public string JavaInstallDirectory(bool is64Bit)
        {


            RegistryKey javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)(is64Bit ? 256 : 512)).OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment");
            string currentVerion = javaRegistryKey.GetValue("CurrentVersion").ToString();

            javaRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)(is64Bit ? 256 : 512)).OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\"+currentVerion);
            string installDirectory = javaRegistryKey.GetValue("JavaHome").ToString();

            return installDirectory;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}

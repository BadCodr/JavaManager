using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            textBox1.Text = isJavaInstalled().ToString();

        }

        private bool isJavaInstalled()
        {

            if (Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.Machine) != null)
                return true;
            else if (Environment.GetEnvironmentVariable("JDK_HOME", EnvironmentVariableTarget.Machine) != null)
                return true;
            else if (System.IO.Directory.Exists(@"C:\ProgramData\Oracle\Java\javapath"))
            {
                if(System.IO.File.Exists(@"C:\ProgramData\Oracle\Java\javapath\java.jar")&&System.IO.File.Exists(@"C:\ProgramData\Oracle\Java\javapath\javaw.jar"));
                    return true;
            }
            else if (System.IO.Directory.Exists(@"C:\Program Files\Java"))
                return true;

            return false;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = isJavaInstalled().ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(System.IO.Directory.Exists(@"C:\Program Files\Java"))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(@"C:\Program Files\Java");
                var directory = directoryInfo.GetDirectories();
                
                textBox1.Text = directory[directory.Length-1].Name;
            }
        }
    }
}

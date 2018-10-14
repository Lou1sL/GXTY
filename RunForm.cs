using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GXTY_CSharp
{
    public partial class RunForm : Form
    {
        public RunForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Enabled = false;
            new MainForm(this).Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool rtn = Program.GoRun(radioButton1.Checked, textBox1.Text, textBox2.Text);
            if (rtn) MessageBox.Show("成功");
            else MessageBox.Show("失败");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            bool rtn = Program.GoFreeRun(radioButton1.Checked, textBox1.Text, textBox2.Text);
            if(rtn) MessageBox.Show("成功");
            else MessageBox.Show("失败");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        
    }
}

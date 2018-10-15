using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            Enabled = false;
            Network.ReturnMessage rm = Program.GoRun(radioButton1.Checked, textBox1.Text, textBox2.Text);
            MessageBox.Show(rm.Msg);
            Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Network.ReturnMessage rm = Program.GoFreeRun(radioButton1.Checked, textBox1.Text, textBox2.Text);
            MessageBox.Show(rm.Msg);
            Enabled = true;
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void RunForm_Load(object sender, EventArgs e)
        {

        }

        //Global variables;
        private bool _dragging = false;
        private Point _offset;
        private Point _start_point = new Point(0, 0);


        private void RunForm_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  // _dragging is your variable flag
            _start_point = new Point(e.X, e.Y);
        }

        private void RunForm_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void RunForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }
    }
}

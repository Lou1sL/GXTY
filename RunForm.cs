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
            Console.SetOut(new TextBoxWriter(textBox3));
            expandsz = Size;
            dfsz = panel2.Size;
            Size = dfsz;


            if (Properties.Settings.Default.FirstLaunch)
                MessageBox.Show(this,
                    "v1.11\n\n" +

                    "警告：使用自己绘制的路径图案未来可能导致封号！\n" +

                    "修复绘制路径时间不合理的问题\n" +
                    "紧急修复可能导致封号的途径点未经过的问题\n" +
                    "自动生成路线低级伪造\n" +
                    "安全原因暂时停用自由跑功能\n" +
                    "修正上海海洋大学的坐标点位置\n\n" +

                    "本程序不以盈利为目的\n" +
                    "如果你是花钱购买的本程序，说明你被坑了，请节哀\n\n" +

                    "制作：留白(RyuBAI)\n"
                    , "更新日志");
            Properties.Settings.Default.FirstLaunch = false;
            
            checkBox1.Checked = Properties.Settings.Default.IsSave;
            if (Properties.Settings.Default.IsSave)
            {
                textBox1.Text = Properties.Settings.Default.Mobile;
                textBox2.Text = Properties.Settings.Default.Pass;
            }

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Enabled = false;
            new MainForm(this).Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Program.GoRun(radioButton1.Checked, textBox1.Text, textBox2.Text);
            R(true);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //Program.GoFreeRun(radioButton1.Checked, textBox1.Text, textBox2.Text);
            //R(false);
        }

        private async void R(bool IsExecOrNot)
        {
            if(Properties.Settings.Default.LastSave!=null &&(DateTime.Now - Properties.Settings.Default.LastSave).TotalMinutes < 45)
            {
                MessageBox.Show("不是我犯贱，但是现在间隔时间过短的跑步数据上传对方直接机器人自动秒封，请再等待 "+ (int)(45 - (DateTime.Now - Properties.Settings.Default.LastSave).TotalMinutes) +" 分钟吧", "警告");
                //return;
            }
            Properties.Settings.Default.LastSave = DateTime.Now;
            Properties.Settings.Default.Save();

            Enabled = false;
            Network.ReturnMessage rm = IsExecOrNot?await ExecRun():await FreeRun();
            MessageBox.Show(rm.Msg);
            Enabled = true;
        }
        private Task<Network.ReturnMessage> ExecRun()
        {
            return Task.Run(() => { return Program.GoRun(radioButton1.Checked, textBox1.Text, textBox2.Text); });
        }
        private Task<Network.ReturnMessage> FreeRun()
        {
            return Task.Run(() => { return Program.GoFreeRun(radioButton1.Checked, textBox1.Text, textBox2.Text); });
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RunForm_Load(object sender, EventArgs e)
        {
            Program.WriteTitle();
        }


        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);
        private void RunForm_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  
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
                Location = new Point(p.X - _start_point.X, p.Y - _start_point.Y);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }



        bool ToolOpened = false;
        Size dfsz;
        Size expandsz;
        private void button5_Click(object sender, EventArgs e)
        {
            ToolOpened = !ToolOpened;
            button5.Text = ToolOpened ? "<" : ">";
            Size = ToolOpened ? expandsz : dfsz;
        }

        private void RunForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            Properties.Settings.Default.IsSave = checkBox1.Checked;
            if (checkBox1.Checked)
            {
                Properties.Settings.Default.Mobile = textBox1.Text;
                Properties.Settings.Default.Pass = textBox2.Text;
                Properties.Settings.Default.Save();
            }
            Properties.Settings.Default.Save();
        }
    }
}

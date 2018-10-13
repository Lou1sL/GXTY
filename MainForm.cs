using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GXTY_CSharp
{
    public partial class MainForm : Form
    {
        public List<Point> PointList { get; private set; } = new List<Point>();

        Image bgimg;
        
        public MainForm()
        {
            InitializeComponent();
            openFileDialog1.Filter = "图片|*.jpg;*.png";

        }
        

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            PointList.Add(e.Location);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (PointList.Count > 1)
            {
                Pen p = new Pen(Color.Black, 2);
                Graphics g = pictureBox1.CreateGraphics();
                for (int i = 0; i < PointList.Count - 1; i++)
                {
                    g.DrawLine(p, PointList[i], PointList[i + 1]);
                }
                p.Dispose();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            PointList.Clear();
            textBox1.Text = "";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                bgimg = Image.FromFile(openFileDialog1.FileName);
            pictureBox1.BackgroundImage = bgimg;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) pictureBox1.BackgroundImage = bgimg;
            else pictureBox1.BackgroundImage = null;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            if (PointList.Count < 2) return;
            
            double scale = Convert.ToSingle(textBox3.Text);

            RunJSON.Position lastP = new RunJSON.Position(Convert.ToSingle(textBox4.Text), Convert.ToSingle(textBox5.Text));

            double dist = 0;
            for(int i = 0; i < PointList.Count; i++)
            {
                if (i > 0)
                {
                    double dx = (PointList[i].X - PointList[i - 1].X) * scale;
                    double dy = (PointList[i].Y - PointList[i - 1].Y) * scale;
                    RunJSON.Position newP = new RunJSON.Position(lastP.Latitude + dx, lastP.Longtitude + dy);
                    dist += newP.Distance(lastP);
                    lastP = newP;

                    textBox1.Text += textBox2.Text + ".AddPosition(new RunJSON.Position(" + lastP.Latitude + "," + lastP.Longtitude + "));\r\n";
                }
                else
                {
                    textBox1.Text += textBox2.Text + ".AddPosition(new RunJSON.Position(" + lastP.Latitude + "," + lastP.Longtitude + "));\r\n";
                }
            }

            label4.Text = "距离: "+dist+" 米";
        }

        /* 
         *设置textBox只能输入数字（正数，负数，小数） 
         *使用了TextBox的KeyPress事件
         */
        private new void KeyPress(object sender, KeyPressEventArgs e)
        {
            //允许输入数字、小数点、删除键和负号  
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != (char)('.') && e.KeyChar != (char)('-'))
            {
                MessageBox.Show("请输入正确的数字");
                this.textBox1.Text = "";
                e.Handled = true;
            }
            if (e.KeyChar == (char)('-'))
            {
                if (textBox1.Text != "")
                {
                    MessageBox.Show("请输入正确的数字");
                    this.textBox1.Text = "";
                    e.Handled = true;
                }
            }
            /*小数点只能输入一次*/
            if (e.KeyChar == (char)('.') && ((TextBox)sender).Text.IndexOf('.') != -1)
            {
                MessageBox.Show("请输入正确的数字");
                this.textBox1.Text = "";
                e.Handled = true;
            }
            /*第一位不能为小数点*/
            if (e.KeyChar == (char)('.') && ((TextBox)sender).Text == "")
            {
                MessageBox.Show("请输入正确的数字");
                this.textBox1.Text = "";
                e.Handled = true;
            }
            /*第一位是0，第二位必须为小数点*/
            if (e.KeyChar != (char)('.') && ((TextBox)sender).Text == "0")
            {
                MessageBox.Show("请输入正确的数字");
                this.textBox1.Text = "";
                e.Handled = true;
            }
            /*第一位是负号，第二位不能为小数点*/
            if (((TextBox)sender).Text == "-" && e.KeyChar == (char)('.'))
            {
                MessageBox.Show("请输入正确的数字");
                this.textBox1.Text = "";
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (PointList.Count > 0) PointList.RemoveAt(PointList.Count - 1);
            pictureBox1.Refresh();
        }
    }
}

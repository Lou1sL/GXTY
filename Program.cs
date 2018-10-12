using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXTY_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Write("手机号:");
            string mobile = Console.ReadLine();
            Console.Write("密码:");
            string pass = Console.ReadLine();
            Console.WriteLine("登陆中...");
            Network.Login(mobile, pass);

            Console.WriteLine("登陆成功!请求开始体育锻炼中...");
            Network.ExecRun();

            //跑步起点
            RunJSON.Position StartP = new RunJSON.Position(30.8669741312, 121.9183560969);
            //每个点的坐标差值
            RunJSON.Position Delta = new RunJSON.Position(0.00004f, 0f);
            RunJSON runJSON = new RunJSON(StartP);
            //自动根据差值添加对应数量的坐标点
            runJSON.AutoAddPosition(Delta, 100);

            Console.WriteLine("上传体育锻炼结果中...");
            Network.SaveExecRun(runJSON);
            Console.WriteLine("上传完成!按回车退出");
            Console.ReadLine();
        }
    }
}

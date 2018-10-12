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
            Console.WriteLine(Network.Login(mobile, pass));
            Console.Write("请输入userid:");
            string userid = Console.ReadLine();
            Console.Write("请输入utoken:");
            Network.SetUtoken(Console.ReadLine());
            Console.WriteLine("请求开始自由跑中...");
            Console.WriteLine(Network.FreeRun(userid));
            Console.Write("请输入runPageId:");
            string runpgid = Console.ReadLine();

            RunJSON.Position StartP = new RunJSON.Position(30.8669741312, 121.9183560969);
            RunJSON.Position Delta = new RunJSON.Position(0.00004f, 0f);
            RunJSON runJSON = new RunJSON(StartP);
            runJSON.AutoAddPosition(Delta, 100);

            Console.WriteLine(Network.SaveRun(runJSON.ToJSON(runpgid, userid)));

            Console.ReadLine();
        }
    }
}

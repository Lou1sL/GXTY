using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GXTY_CSharp
{
    class Program
    {
        public static readonly RunJSON.Position SHOUPosition = new RunJSON.Position(30.887422, 121.902849);

        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new RunForm());
        }

        /// <summary>
        /// 体育训练跑
        /// 从登陆到获得跑步id的过程
        /// </summary>
        /// <param name="usegpx"></param>
        /// <param name="mobile"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static Network.ReturnMessage GoRun(bool usegpx, string mobile, string pass)
        {
            Network.ReturnMessage rm;

            Console.WriteLine("登陆中...");
            rm = Network.Login(mobile, pass);
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;

            Console.WriteLine("请求开始体育锻炼中...");
            rm = Network.AskExecRun();
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;

            Network.RunPackage pkg = Network.GenerateExecRunPackage(usegpx);
            Properties.Settings.Default.WaitTill = pkg.waittill;
            Properties.Settings.Default.Post = pkg.post;
            Properties.Settings.Default.Package_Cookie = pkg.cookie;
            Properties.Settings.Default.Package_Utoken = pkg.utoken;
            Properties.Settings.Default.Save();

            return rm;
        }
        /// <summary>
        /// 自由跑
        /// 从登陆到获得跑步id的过程
        /// </summary>
        /// <param name="usegpx"></param>
        /// <param name="mobile"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static Network.ReturnMessage GoFreeRun(bool usegpx, string mobile, string pass)
        {
            Network.ReturnMessage rm;

            Console.WriteLine("登陆中...");
            rm = Network.Login(mobile, pass);
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;

            Console.WriteLine("请求开始自由跑中...");
            rm = Network.AskFreeRun();
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;

            Network.RunPackage pkg = Network.GenerateFreeRunPackage(usegpx);
            Properties.Settings.Default.WaitTill = pkg.waittill;
            Properties.Settings.Default.Post = pkg.post;
            Properties.Settings.Default.Package_Cookie = pkg.cookie;
            Properties.Settings.Default.Package_Utoken = pkg.utoken;
            Properties.Settings.Default.Save();

            return rm;
        }
        /// <summary>
        /// 结束跑步的提交，这个提交必须延迟时间否则秒封
        /// </summary>
        /// <returns></returns>
        public static Network.ReturnMessage FinRun()
        {
            Network.RunPackage package = new Network.RunPackage()
            {
                waittill = Properties.Settings.Default.WaitTill,
                post = Properties.Settings.Default.Post,
                utoken = Properties.Settings.Default.Package_Utoken,
                cookie = Properties.Settings.Default.Package_Cookie,
            };
            if (string.IsNullOrEmpty(package.post)) return null;

            Properties.Settings.Default.Post = "";
            Properties.Settings.Default.Save();

            
            Console.WriteLine("上传跑步结果中...");
            Network.ReturnMessage rm = Network.SaveExecRun(package);
            Console.WriteLine(rm);
            return rm;
        }
        public static void WriteTitle()
        {
            Console.WriteLine(@"                                                                          ");
            Console.WriteLine(@"                                    ]/O[[[[[[[@\`                         ");
            Console.WriteLine(@"                               ,/[              *\@@@\]                   ");
            Console.WriteLine(@"                            ]/                     * **,\O`               ");
            Console.WriteLine(@"                          /`    ]  *]/                  **\@`             ");
            Console.WriteLine(@"                        =`   ,/  =[`   ]^  ,`**      *  ***,O\            ");
            Console.WriteLine(@"                       /    /  ,`/  ,/    / *  *,*** ******`=O@`          ");
            Console.WriteLine(@"                     ,/   ,^  O/` ,/    ,`    ,\@ =**,******OOO@          ");
            Console.WriteLine(@"                     / ,*O` /OO**OO*,]/O^^* *,O`/ =  *^*=*=OoOOO@         ");
            Console.WriteLine(@"                    / =,O^*/OOOOO\]]]oO@`*`*,O^ O*/`  O*=\/OOOOOO^        ");
            Console.WriteLine(@"                   =^=`O^*OO@@/[[`     [OOOOO^ =OOOO**OooOoOOoOOO@        ");
            Console.WriteLine(@"                   @ ^o/*/oO^\/@@@@@@/         \\/OoOOOOOOO\OooOO@        ");
            Console.WriteLine(@"                  =^=oO*=OO^         [^           ,\OO@OO^   OoOO@        ");
            Console.WriteLine(@"                  O /o^*Oo@                   ,]]],\  \@O@OOOOO@O@        ");
            Console.WriteLine(@"                 ,^ OO`=OOO             ^        [@@\  =@OOO@OO@OO        ");
            Console.WriteLine(@"               ,O\^=OO*=O@^                        ,@O =@OOO@OO@O^        ");
            Console.WriteLine(@"      ,/Ooo[[`,/OO^=o@`=oOO         ,]                 @@OoOOOOOO`        ");
            Console.WriteLine(@"        ,\\]]OO@@@O=O@^=O@/       /@@@\]  ^           /@OooOOO@OO         ");
            Console.WriteLine(@"               /`o@=O@^/OOO^     /@@@@@@@@@          /\OO^OOOO@O^         ");
            Console.WriteLine(@"   =\`     ]O[**oO@OO@@/O@O@\[^  OOOOOOO@@@        ,@O@OOo@OO@O@          ");
            Console.WriteLine(@"    O\***     /OOO@@O@OOOOOO^,^ /*OOOOOOO/^       =O/@OO/@OOO@O@          ");
            Console.WriteLine(@"     ,\/[[OOOOOO/ /o@OOOOOO^ OO/ =` [@/@[^^    ,/@O@@O/\@OO=@OOO^         ");
            Console.WriteLine(@"           [`    /`OO@O/@O^ =O/ /@`/ ,@`,^\]@@OOOOO@@\o@O@oO@OOOO`        ");
            Console.WriteLine(@"               ,/]OO@@@\/^ =@^ //=` /@@O**=@@@@@@@@/\OO@@oO@@@OOOOO]    ] ");
            Console.WriteLine(@"         ,,,]OO[oO@O@@@@@@@@@@@@/  //`,^**/@@@@@///@OO@@\O@OO ,[OOOOO/[   ");
            Console.WriteLine(@"             [`  /@@@@@@@@@@@@@@@/\` /***/@@@Oo/@@O@@@O=@OO@OO            ");
            Console.WriteLine(@"                /@@@@@@@@@@@@@@@@@,O*]]//OoO@@@@@@@@oO@@OO@OO@O@]    ` ,` ");
            Console.WriteLine(@"              ,@@@@@@@@@@@@@@@@@@@@@@@@\ *[o@@@@@@@@@@@@^[\oOOOO@OO@OO/`  ");
            Console.WriteLine(@"       ,@@@@@@@@@@@@@@@@@@@@@@@@@@O@^=@@` ,oo@O@@@@@@@@@     [[[          ");
            Console.WriteLine(@"      /@@@@@@@@@@@@@@@@@@@@@@@@@@OO@@@@O\]@@@@@@@@@@@@@ooO`               ");
            Console.WriteLine(@"     @@@@@@@@OoOO@@@@@@@@@@@@@@O@@@@o@@OO@\OO@@@@@@@@OOoooO@@\`           ");
            Console.WriteLine(@"    /@@@@@@@@@@@@@OO@@@@@@@@@@@@@@@@O@O@@@@@@@@@@@@@@\\oooo@@@@@\         ");
            Console.WriteLine(@"   =@@@@@@@@@@@OO@@@@O@@@@@Oo  ,@O@@@@@@OOOOO@@@@@@@@@/Oooo@@@@@@^        ");
            Console.WriteLine(@"   @@@@@@@@@@@@@@@OO@@@O@@@/  =/O@@@@@@OO,Oo@@@@@@@@@@^\oooO@@@@@O        ");
            Console.WriteLine(@"  /@@@@@@@@@@@@@@@@@O@@@@@o  ,OO/@@/\@OOO^ =@@@@@@@@@@@=oooo@@@@@@        ");
            Console.WriteLine(@" @@@@@@@@@@@@@@@@@@@@@@@@@O  Oo/ @@@@@O@\O =@@@@@@@@@@@oOooo@@@@@@        ");
            Console.WriteLine(@" @@@@@@@@@@@@@@@@@@@@@@@@@O O]@ ,@@@@@O=o@^@@@@@@@@@@@@OOooo@@@@@@        ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("-----------------------高校体育APP一键自动体育锻炼工具---------------------");
            Console.WriteLine("---------------------------------制作:留白---------------------------------");
            Console.WriteLine("-------------------------https://github.com/RyuBAI-------------------------");
            Console.WriteLine("---------------------------------------------------------------------------");
        }
    }
}

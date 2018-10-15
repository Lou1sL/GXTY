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
        public static RunJSON runJSON = new RunJSON(new RunJSON.Position(30.8669741312, 121.9183560969));

        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new RunForm());
        }

        public static Network.ReturnMessage GoRun(bool usegpx, string mobile, string pass)
        {
            RunJSONInit(usegpx);
            Network.ReturnMessage rm;

            Console.WriteLine("登陆中...");
            rm = Network.Login(mobile, pass);
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;

            Console.WriteLine("请求开始体育锻炼中...");
            rm = Network.ExecRun();
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;


            Console.WriteLine("上传体育锻炼结果中...");
            rm = Network.SaveExecRun(runJSON);
            Console.WriteLine(rm.Msg + " : "+rm.Data["desc"]);
            return rm;
        }
        public static Network.ReturnMessage GoFreeRun(bool usegpx, string mobile, string pass)
        {
            RunJSONInit(usegpx);
            Network.ReturnMessage rm;

            Console.WriteLine("登陆中...");
            rm = Network.Login(mobile, pass);
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;

            Console.WriteLine("请求开始自由跑中...");
            rm = Network.FreeRun();
            Console.WriteLine(rm.Msg);
            if (rm.Code != 200) return rm;

            Console.WriteLine("上传自由跑结果中...");
            rm = Network.SaveFreeRun(runJSON);
            Console.WriteLine(rm.Msg + " : " + rm.Data["desc"]);
            return rm;
        }

        private static void RunJSONInit(bool usegpx)
        {
            runJSON = new RunJSON(new RunJSON.Position(30.8669741312, 121.9183560969));
            runJSON.AutoAddPosition(new RunJSON.Position(0.0001f, 0f), new Random().Next(290, 330), 4f);

            if (usegpx)
            {
                if (!File.Exists("map.gpx"))
                    Console.WriteLine("map.gpx不存在!回退至自动生成路径!");
                else
                    runJSON.LoadGPX("map.gpx");
            }
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

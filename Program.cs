using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;

namespace GPXGen
{
    public static class GXTYUtils
    {
        private const string Salt = "lpKK*TJE8WaIg%93O0pfn0#xS0i3xE$z";

        public static string Sign(string datacontent)
        {
            String teststr = "{\"bNode\":[],\"buPin\":\"0.0\",\"duration\":\"117\",\"endTime\":\"2018-10-05 07:30:15\",\"frombp\":\"0\",\"goal\":\"2.00\",\"real\":\"500.65475\",\"runPageId\":\"6702563\",\"speed\":\"03\\u002751\\u0027\\u0027\",\"startTime\":\"2018-10-05 07:28:03\",\"tNode\":[],\"totalNum\":\"0\",\"track\":[{\"latitude\":30.89071967230903,\"longitude\":121.92207980685764},{\"latitude\":30.89121826171875,\"longitude\":121.92207980685764},{\"latitude\":30.891318359375,\"longitude\":121.92207980685764},{\"latitude\":30.8915185546875,\"longitude\":121.92207980685764},{\"latitude\":30.89161865234375,\"longitude\":121.92207980685764},{\"latitude\":30.891820203993056,\"longitude\":121.922080078125},{\"latitude\":30.892519259982638,\"longitude\":121.922080078125},{\"latitude\":30.89271918402778,\"longitude\":121.922080078125},{\"latitude\":30.892819552951387,\"longitude\":121.922080078125},{\"latitude\":30.89301947699653,\"longitude\":121.922080078125},{\"latitude\":30.89311957465278,\"longitude\":121.922080078125},{\"latitude\":30.893321397569444,\"longitude\":121.922080078125},{\"latitude\":30.894019911024305,\"longitude\":121.922080078125},{\"latitude\":30.894220377604167,\"longitude\":121.922080078125},{\"latitude\":30.894320203993054,\"longitude\":121.922080078125},{\"latitude\":30.894520399305556,\"longitude\":121.922080078125},{\"latitude\":30.894620225694446,\"longitude\":121.922080078125},{\"latitude\":30.89482231987847,\"longitude\":121.922080078125},{\"latitude\":30.894922146267362,\"longitude\":121.922080078125},{\"latitude\":30.8951220703125,\"longitude\":121.92208034939236},{\"latitude\":30.89522243923611,\"longitude\":121.92208034939236},{\"latitude\":30.89542236328125,\"longitude\":121.92208034939236},{\"latitude\":30.895520833333332,\"longitude\":121.92208034939236},{\"latitude\":30.895721028645834,\"longitude\":121.92208034939236},{\"latitude\":30.895821126302085,\"longitude\":121.92208034939236},{\"latitude\":30.896021050347223,\"longitude\":121.92208034939236},{\"latitude\":30.896121419270834,\"longitude\":121.92208034939236},{\"latitude\":30.89632297092014,\"longitude\":121.92208034939236},{\"latitude\":30.89642306857639,\"longitude\":121.92208034939236}],\"trend\":[{\"x\":0.1,\"y\":51.7},{\"x\":0.2,\"y\":51.95},{\"x\":0.3,\"y\":51.916668},{\"x\":0.4,\"y\":52.6},{\"x\":0.5,\"y\":54.333332}],\"type\":\"2\",\"userid\":\"133652\"}";
            return Str2MD5(Salt + "data" + datacontent);
        }

        private static string Str2MD5(string ClearText)
        {

            byte[] ByteData = Encoding.ASCII.GetBytes(ClearText);
            MD5 oMd5 = MD5.Create();
            byte[] HashData = oMd5.ComputeHash(ByteData);
            StringBuilder oSb = new StringBuilder();

            for (int x = 0; x < HashData.Length; x++)
            {
                oSb.Append(HashData[x].ToString("x2"));
            }
            return oSb.ToString();
        }
    }

    public class GXTY
    {
        public class Position
        {
            public double Latitude { get; private set; }
            public double Longtitude { get; private set; }
            public float Elevation { get; private set; }
            public DateTime Time { get; private set; }
            public Position(double lat, double lon, float ele = -1.15f)
            {
                Latitude = lat; Longtitude = lon; Elevation = ele; Time = DateTime.Now;
            }
            public void SetTime(DateTime t)
            {
                Time = t;
            }
            public string ToGPX()
            {
                string str = string.Empty;
                str += "<wpt lat=\"" + Latitude + "\" lon=\"" + Longtitude + "\">\r\n";
                str += "    <ele>" + Elevation + "</ele>\r\n";
                str += "    <time>" + Time.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "Z</time>\r\n";
                str += "</wpt>\r\n";
                return str;
            }

            public string ToJson()
            {
                return "{\"latitude\":" + Latitude + ",\"longitude\":" + Longtitude + "}";
            }

            public const double EARTH_RADIUS = 6371000;
            public double Distance(Position p2)
            {
                //经纬转弧度
                double lat1 = ConvertDegreesToRadians(Latitude);
                double lon1 = ConvertDegreesToRadians(Longtitude);
                double lat2 = ConvertDegreesToRadians(p2.Latitude);
                double lon2 = ConvertDegreesToRadians(p2.Longtitude);

                //差值
                var vLon = Math.Abs(lon1 - lon2);
                var vLat = Math.Abs(lat1 - lat2);

                //球体上的切面，它的圆心即是球心的一个周长最大的圆。
                var h = HaverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * HaverSin(vLon);

                var distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));

                return distance;
            }
            public static double TotalDistance(List<Position> plist)
            {
                if (plist.Count <= 1) return 0.0;
                double dist = 0.0;
                for(int i = 0; i < plist.Count - 1; i++)
                {
                    dist += plist[i].Distance(plist[i + 1]);
                }
                return dist;
            }

            private static double HaverSin(double theta)
            {
                var v = Math.Sin(theta / 2);
                return v * v;
            }
            /// <summary>
            /// 将角度换算为弧度。
            /// </summary>
            /// <param name="degrees">角度</param>
            /// <returns>弧度</returns>
            private static double ConvertDegreesToRadians(double degrees)
            {
                return degrees * Math.PI / 180;
            }
            private static double ConvertRadiansToDegrees(double radian)
            {
                return radian * 180.0 / Math.PI;
            }


        }

        private List<Position> PositionList;
        
        public GXTY(Position start)
        {
            PositionList = new List<Position> { start };
        }

        public void AddPosition(Position p)
        {
            PositionList.Add(p);
        }
        
        public string ToGPX()
        {
            string str = "<?xml version=\"1.0\"?>\r\n<gpx version=\"1.1\">\r\n";
            foreach (Position p in PositionList) str += p.ToGPX();
            str += "</gpx>";

            return str;
        }

        /// <summary>
        /// 生成数据包
        /// </summary>
        /// <param name="runpgid">本次跑步id，点击开始跑时服务器会发送过来这个值</param>
        /// <param name="userid">用户id，不会变</param>
        /// <returns></returns>
        public string ToJson(string runpgid,string userid)
        {
            
            string StartT = PositionList.First().Time.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
            string EndT = PositionList.Last().Time.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
            double Duration = (PositionList.Last().Time - PositionList.First().Time).TotalSeconds;
            double Distance = Position.TotalDistance(PositionList);
            TimeSpan Speed = TimeSpan.FromSeconds(Duration / (Distance / 1000));

            string str = string.Empty;
            str += "{";
            str += "\"bNode\":[],";
            str += "\"buPin\":\"0.0\",";
            str += "\"duration\":\"" + (int)Duration + "\",";
            str += "\"endTime\":\"" + EndT + "\",";
            str += "\"frombp\":\"0\",";
            str += "\"goal\":\"2.00\",";
            //TODO:rly?
            str += "\"real\":\"" + Distance + "\",";
            str += "\"runPageId\":\"" + runpgid + "\",";
            str += "\"speed\":\"" + Speed.Hours.ToString("00") + "\\u0027" + Speed.Minutes.ToString("00") + "\\u0027\\u0027\",";
            str += "\"startTime\":\"" + StartT + "\",";
            str += "\"tNode\":[],";
            str += "\"totalNum\":\"0\",";

            str += "\"track\":[";
            foreach (Position p in PositionList)
            {
                str += p.ToJson();
                if (p != PositionList.Last()) str += ",";
            }
            str += "],";

            
            str += "\"trend\":[";
            //TODO:Trend??
            str += "],";

            str += "\"type\":\"2\",";
            str += "\"userid\":\""+userid+"\"";
            str += "}";

            //Console.WriteLine(str);
            string signstr = "sign=" + GXTYUtils.Sign(str) + "&data=";
            return signstr + WebUtility.UrlEncode(str);
        }

        public void WriteGPX(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            using (StreamWriter writer = new StreamWriter(path, true))
                writer.WriteLine(ToGPX());
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "gen.gpx";
            GXTY.Position StartP = new GXTY.Position(30.8669741312, 121.9183560969);

            GXTY gxty = new GXTY(StartP);
            
            for (int i = 1; i < 100; i++)
            {
                GXTY.Position p = new GXTY.Position(StartP.Latitude + i*6f, StartP.Longtitude);
                p.SetTime(StartP.Time.AddSeconds(i));
                gxty.AddPosition(p);
            }

            gxty.WriteGPX(path);
            //记得改这俩值哦:)
            Console.WriteLine(gxty.ToJson("6723829", "133652"));
            Console.ReadLine();

        }
    }
}

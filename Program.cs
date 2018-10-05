using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;

namespace GPXGen
{
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
        /// <param name="sign">校验值，目前不知道计算方法，可以确定和本次跑步数据有关</param>
        /// <param name="runpgid">本次跑步id，点击开始跑时服务器会发送过来这个值</param>
        /// <param name="userid">用户id，不会变</param>
        /// <returns></returns>
        public string ToJson(string sign,string runpgid,string userid)
        {
            string signstr = "sign=" + sign + "&data=";
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
            GXTY.Position StartP = new GXTY.Position(30.866974131222978, 121.91835609691202);

            GXTY gxty = new GXTY(StartP);
            
            for (int i = 1; i < 10; i++)
            {
                GXTY.Position p = new GXTY.Position(StartP.Latitude + i*0.00005f, StartP.Longtitude);
                p.SetTime(StartP.Time.AddSeconds(i));
                gxty.AddPosition(p);
            }

            gxty.WriteGPX(path);

            Console.WriteLine(gxty.ToJson("blablabla", "12345", "12345"));
            Console.ReadLine();

        }
    }
}

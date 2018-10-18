using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GXTY_CSharp
{
    public static class Network
    {

        private const string API_ROOT = "http://gxhttp.chinacloudapp.cn/api/";
        private const string API_LOGIN = "reg/login";
        private const string API_RUN = "run/runPage";
        private const string API_SAVERUN = "run/saveRunV2";

        private static string uuid = "B5ED79A287A41BF46CA1FFFA4DAB3480";
        private static string utoken = string.Empty;
        private static string userid = string.Empty;
        private static string runpgid = string.Empty;
        private static string bNodeArray = string.Empty;
        private static string tNodeArray = string.Empty;

        private static class Json2Package
        {
            private const string Salt = "lpKK*TJE8WaIg%93O0pfn0#xS0i3xE$z";

            public static string Create(string json)
            {
                string jsonfixed = json.Replace("\r","").Replace("\n", "").Replace("\\r", "").Replace("\\n", "");
                string sign = Str2MD5(Salt + "data" + jsonfixed);
                string data = WebUtility.UrlEncode(jsonfixed);
                return "sign=" + sign + "&data=" + data;
            }
            private static string Str2MD5(string str)
            {
                byte[] ByteData = Encoding.ASCII.GetBytes(str);
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

        private static string LoginJSON(string mobile, string pass)
        {
            return "{\"info\":\"" + uuid + "\",\"mobile\":\"" + mobile + "\",\"password\":\"" + pass + "\",\"type\":\"AndroidSDKbuiltforx86\"}";
        }
        private static string ExecRunJSON()
        {
            return "{\"initLocation\":\"\",\"type\":\"1\",\"userid\":\"" + userid + "\"}";
        }
        private static string FreeRunJSON()
        {
            return "{\"initLocation\":\"\",\"type\":\"2\",\"userid\":\""+userid+"\"}";
        }

        public static ReturnMessage Login(string mobile, string pass)
        {
            ReturnMessage rm = new ReturnMessage(Request<JObject>(API_ROOT + API_LOGIN, Json2Package.Create(LoginJSON(mobile, pass))));
            if (rm.Code == 200)
            {
                userid = rm.Data["userid"].ToString();
                utoken = rm.Data["utoken"].ToString();
            }
            return rm;
        }
        public static ReturnMessage ExecRun()
        {
            ReturnMessage rm = new ReturnMessage(Request<JObject>(API_ROOT + API_RUN, Json2Package.Create(ExecRunJSON())));
            if (rm.Code == 200)
            {
                runpgid = rm.Data["runPageId"].ToString();
                bNodeArray = rm.Data["ibeacon"].ToString();
                tNodeArray = rm.Data["gpsinfo"].ToString();
            }
            return rm;
        }
        public static ReturnMessage FreeRun()
        {
            ReturnMessage rm = new ReturnMessage(Request<JObject>(API_ROOT + API_RUN, Json2Package.Create(FreeRunJSON())));
            if (rm.Code == 200)
            {
                runpgid = rm.Data["runPageId"].ToString();
            }
            return rm;
        }
        public static ReturnMessage SaveExecRun(bool readgpx)
        {

            JArray tNode = (JArray)JsonConvert.DeserializeObject(tNodeArray);
            RunJSON runJSON = new RunJSON(new RunJSON.Position(Convert.ToDouble(tNode[0]["latitude"]), Convert.ToDouble(tNode[0]["longitude"])));
            
            JArray bNode = (JArray)JsonConvert.DeserializeObject(bNodeArray);
            foreach (JObject bn in bNode)
                runJSON.AddPosition(new RunJSON.Position(Convert.ToDouble(bn["position"]["latitude"]), Convert.ToDouble(bn["position"]["longitude"])));

            while (runJSON.TotalDistance() < 2000)
                runJSON.AddPosition(new RunJSON.Position(runJSON.PositionList.Last().Latitude + 0.0001f, 0f));


            runJSON.DistributeTimeSpan(TimeSpan.FromMinutes(25), DateTime.Now - TimeSpan.FromMinutes(25));

            if (readgpx)
            {
                if (!runJSON.LoadGPX("map.gpx"))
                    Console.WriteLine("map.gpx不存在/有问题!回退至自动生成路径!");
            }

            string json = runJSON.ToJSON(runpgid, userid, bNodeArray, tNodeArray);
            string pkg = Json2Package.Create(json);
            ReturnMessage rm = new ReturnMessage(Request<JObject>(API_ROOT + API_SAVERUN, "", pkg));
            return rm;

        }
        public static ReturnMessage SaveFreeRun(bool readgpx)
        {
            RunJSON runJSON = new RunJSON(Program.SHOUPosition);
            runJSON.AutoAddPosition(new RunJSON.Position(0.0001f, 0f), new Random().Next(240, 290), 4f);
            if (readgpx)
            {
                if (!runJSON.LoadGPX("map.gpx"))
                    Console.WriteLine("map.gpx不存在/有问题!回退至自动生成路径!");
            }

            string json = runJSON.ToJSON(runpgid, userid);
            string pkg = Json2Package.Create(json);
            ReturnMessage rm = new ReturnMessage(Request<JObject>(API_ROOT + API_SAVERUN, "", pkg));
            return rm;
        }

        private static CookieContainer cookie = new CookieContainer();
        private static T Request<T>(string url, string get, string post = "")
        {
            var request = (HttpWebRequest)WebRequest.Create(url + (get == "" ? "" : "?" + get));

            if (post != "")
            {
                request.Method = "POST";
                request.Timeout = 5000;
                request.AllowAutoRedirect = false;
                request.ContentType = "application/x-www-form-urlencoded";
                request.KeepAlive = true;
            }

            request.UserAgent = "okhttp-okgo/jeasonlzy";
            request.Headers.Add("versionCode: 296");
            request.Headers.Add("versionName: 2.2.0");
            request.Headers.Add("platform: android");
            request.Headers.Add("xxversionxx: 20180601");
            request.Headers.Add("uuid: " + uuid);
            request.Headers.Add("utoken: " + utoken);
            request.Headers.Add("BDA9F42E0C8A294ECDF5CC72AAE6A701: 0,0,0,0,1");
            request.CookieContainer = cookie;

            if (post != "")
            {
                byte[] postBytes = Encoding.UTF8.GetBytes(post);
                request.ContentLength = postBytes.Length;
                Stream postDataStream = request.GetRequestStream();
                postDataStream.Write(postBytes, 0, postBytes.Length);
                postDataStream.Close();
            }
            
            var response = (HttpWebResponse)request.GetResponse();
            //response.Cookies = cookie.GetCookies(request.RequestUri);
            string str = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (typeof(T) == typeof(string)) return (T)(str as object);
            if (typeof(T) == typeof(JObject))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(str);
                    return (T)(jo as object);
                }else
                {
                    throw new Exception("Server responded nothing!");
                }
            }
            return default(T);
        }


        public class ReturnMessage
        {
            public int Code { get; private set; }
            public string Msg { get; private set; }
            public JObject Data { get; private set; }

            public ReturnMessage(JObject jo)
            {
                Code = Convert.ToInt32(jo["code"].ToString());
                Msg = jo["msg"].ToString();
                if (jo["data"] != null) Data = (JObject)JsonConvert.DeserializeObject(jo["data"].ToString());
            }
        }
    }
}

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

        public static bool Login(string mobile, string pass)
        {
            JObject jo = Request<JObject>(API_ROOT + API_LOGIN, Json2Package.Create(LoginJSON(mobile, pass)));
            Console.WriteLine(jo["msg"]);
            if (jo["code"].ToString() != "200") return false;


            userid = jo["data"]["userid"].ToString();
            utoken = jo["data"]["utoken"].ToString();

            return true;
        }
        public static bool ExecRun()
        {
            JObject jo = Request<JObject>(API_ROOT + API_RUN, Json2Package.Create(ExecRunJSON()));
            Console.WriteLine(jo["msg"]);
            if (jo["code"].ToString() != "200") return false;

            runpgid = jo["data"]["runPageId"].ToString();
            bNodeArray = jo["data"]["ibeacon"].ToString();
            tNodeArray = jo["data"]["gpsinfo"].ToString();

            return true;
        }
        public static  bool FreeRun()
        {
            JObject jo = Request<JObject>(API_ROOT + API_RUN, Json2Package.Create(FreeRunJSON()));
            Console.WriteLine(jo["msg"]);
            if (jo["code"].ToString() != "200") return false;

            runpgid = jo["data"]["runPageId"].ToString();

            return true;
        }
        public static bool SaveExecRun(RunJSON runjson)
        {
            string json = runjson.ToJSON(runpgid, userid, bNodeArray, tNodeArray);
            string pkg = Json2Package.Create(json);
            JObject jo = Request<JObject>(API_ROOT + API_SAVERUN, "", pkg);
            Console.WriteLine(jo["msg"] + " : " + jo["data"]["desc"]);
            return (jo["code"].ToString() == "200");
        }
        public static bool SaveFreeRun(RunJSON runjson)
        {
            JObject jo = Request<JObject>(API_ROOT + API_SAVERUN, "", Json2Package.Create(runjson.ToJSON(runpgid, userid)));
            Console.WriteLine(jo["msg"]+" : "+jo["data"]["desc"]);
            return (jo["code"].ToString() == "200");
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
                }
            }
            return default(T);
        }
    }
}

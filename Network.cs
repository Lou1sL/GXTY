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
                string sign = Str2MD5(Salt + "data" + json);
                string data = WebUtility.UrlEncode(json);
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

        public static void Login(string mobile, string pass)
        {
            JObject jo = Request(API_ROOT + API_LOGIN, Json2Package.Create(LoginJSON(mobile, pass)));
            userid = jo["data"]["userid"].ToString();
            utoken = jo["data"]["utoken"].ToString();
        }
        public static void ExecRun()
        {
            JObject jo = Request(API_ROOT + API_RUN, Json2Package.Create(ExecRunJSON()));
            runpgid = jo["data"]["runPageId"].ToString();
            bNodeArray = jo["data"]["ibeacon"].ToString();
            tNodeArray = jo["data"]["gpsinfo"].ToString();
        }
        public static void FreeRun()
        {
            JObject jo = Request(API_ROOT + API_RUN, Json2Package.Create(FreeRunJSON()));
            runpgid = jo["data"]["runPageId"].ToString();
        }
        public static void SaveExecRun(RunJSON runjson)
        {
            string json = runjson.ToJSON(runpgid, userid);
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            jo["bNode"] = bNodeArray;
            jo["tNode"] = tNodeArray;
            jo["type"] = "2";
            Console.WriteLine(jo.ToString());
            string pkg = Json2Package.Create(jo.ToString());
            Request(API_ROOT + API_SAVERUN, "", pkg);
        }
        public static void SaveFreeRun(RunJSON runjson)
        {
            Request(API_ROOT + API_SAVERUN, "", Json2Package.Create(runjson.ToJSON(runpgid, userid)));
        }

        private static CookieContainer cookie = new CookieContainer();
        private static JObject Request(string url, string get, string post = "")
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
            JObject jo = (JObject)JsonConvert.DeserializeObject(str);
            if (jo["code"].ToString() != "200")
            {
                Console.WriteLine("步骤执行失败!以下是服务器的返回值:");
                Console.WriteLine(str);
                Console.ReadLine();
                Environment.Exit(0);
            }
            return jo;
        }
    }
}

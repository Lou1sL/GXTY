using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace GXTY_CSharp
{
    public static class Network
    {
        private const string API_ROOT = "http://gxhttp.chinacloudapp.cn/api/";
        private const string API_LOGIN = "reg/login";
        private const string API_RUN = "run/runPage";
        private const string API_SAVERUN = "run/saveRunV2";



        private static class Json2Package
        {
            private const string Salt = "lpKK*TJE8WaIg%93O0pfn0#xS0i3xE$z";

            public static string Create(string json)
            {
                string sign = Str2MD5(Salt + "data" + json);
                string data = WebUtility.UrlEncode(json);
                return "sign=" + sign + "&data=" + data;
            }
            public static string Reverse(string package)
            {
                return WebUtility.UrlDecode(package);
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
        private static string ExecRunJSON(string userid)
        {
            return "{\"initLocation\":\"\",\"type\":\"1\",\"userid\":\"" + userid + "\"}";
        }
        private static string FreeRunJSON(string userid)
        {
            return "{\"initLocation\":\"\",\"type\":\"2\",\"userid\":\""+userid+"\"}";
        }
        private static string SaveRunJSON(string json)
        {
            return json;
        }

        private static string uuid = "B5ED79A287A41BF46CA1FFFA4DAB3480";
        private static string utoken = string.Empty;
        private static CookieContainer cookie = new CookieContainer();
        private static string GetRequest(string url, string get)
        {
            var request = (HttpWebRequest)WebRequest.Create(url+"?"+get);
            request.CookieContainer = cookie;

            request.UserAgent = "okhttp-okgo/jeasonlzy";
            request.Headers.Add("versionCode: 296");
            request.Headers.Add("versionName: 2.2.0");
            request.Headers.Add("platform: android");
            request.Headers.Add("xxversionxx: 20180601");
            request.Headers.Add("uuid: " + uuid);
            request.Headers.Add("utoken: "+utoken);
            request.Headers.Add("BDA9F42E0C8A294ECDF5CC72AAE6A701: 0,0,0,0,1");

            var response = (HttpWebResponse)request.GetResponse();
            //response.Cookies = cookie.GetCookies(request.RequestUri);
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        private static string PostRequest(string url,string post)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 5000;
            request.AllowAutoRedirect = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            request.CookieContainer = cookie;

            request.UserAgent = "okhttp-okgo/jeasonlzy";
            request.Headers.Add("versionCode: 296");
            request.Headers.Add("versionName: 2.2.0");
            request.Headers.Add("platform: android");
            request.Headers.Add("xxversionxx: 20180601");
            request.Headers.Add("uuid: " + uuid);
            request.Headers.Add("utoken: " + utoken);
            request.Headers.Add("BDA9F42E0C8A294ECDF5CC72AAE6A701: 0,0,0,0,1");

            byte[] postBytes = Encoding.UTF8.GetBytes(post);
            request.ContentLength = postBytes.Length;
            Stream postDataStream = request.GetRequestStream();
            postDataStream.Write(postBytes, 0, postBytes.Length);
            postDataStream.Close();


            var response = (HttpWebResponse)request.GetResponse();
            //response.Cookies = cookie.GetCookies(request.RequestUri);

            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public static string Login(string mobile, string pass)
        {
            return Json2Package.Reverse(GetRequest(API_ROOT + API_LOGIN, Json2Package.Create(LoginJSON(mobile, pass))));
        }
        public static void SetUtoken(string utoken)
        {
            Network.utoken = utoken;
        }
        public static string ExecRun(string uid)
        {
            return Json2Package.Reverse(GetRequest(API_ROOT + API_RUN, Json2Package.Create(ExecRunJSON(uid))));
        }
        public static string FreeRun(string uid)
        {
            return Json2Package.Reverse(GetRequest(API_ROOT + API_RUN, Json2Package.Create(FreeRunJSON(uid))));
        }
        public static string SaveRun(string runjson)
        {
            return Json2Package.Reverse(PostRequest(API_ROOT + API_SAVERUN, Json2Package.Create(SaveRunJSON(runjson))));
        }
    }
}

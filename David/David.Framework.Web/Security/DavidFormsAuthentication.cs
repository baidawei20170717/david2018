using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace David.Framework.Web.Security
{
    public class DavidFormsAuthentication
    {
        private const string AuthKey = "davidzc8";//密钥，且必须为8位。
        private const string AuthCookieName = "DavidAuth";//cookie名称
        private const int ExpiresNumOfDay = 7;//cookie过期天数

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userId">用户ID</param>
        /// <param name="createPersistentCookie">是否记住登录</param>
        public static void SignIn(string userName, int userId, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");

            string cookieSaveValue = string.Format("username={0};userid={1};ip={2}",
                userName, userId, HttpContext.Current.Request.UserHostAddress);//保存在Cookie中的值

            string cookie = EncryptionHelper.EncryptBalance(cookieSaveValue, AuthKey);

            HttpCookie authCookie = new HttpCookie(AuthCookieName, cookie);
            string ip = HttpContext.Current.Request.UserHostAddress;
      
            authCookie.Path = "/";

            if (createPersistentCookie)
            {
                authCookie.Expires = DateTime.Now.AddDays(ExpiresNumOfDay);
            }

            HttpContext.Current.Response.Cookies.Add(authCookie);
            //装填token，防攻击
            string token = Guid.NewGuid().ToString().Replace("-", "");
            HttpCookie tokenCookie = new HttpCookie("DavidToken", token);
            HttpContext.Current.Session[userName] = token;
            HttpContext.Current.Response.Cookies.Add(tokenCookie);
        }
        /// <summary>
        /// 是否登录
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            string value = GetCookieValue();
            string username = GetUserName();
            string token = HttpContext.Current.Request.Headers["token"];
            if (value.Contains("username=") && value.Contains(";userid=") //登陆信息验证
                && HttpContext.Current.Session[username] != null
                && GetUserIP() == HttpContext.Current.Request.UserHostAddress) //XSS身份伪装验证
            {
                if (HttpContext.Current.Request.RequestType == "GET")  //CSRF攻击验证)
                {
                    return true;
                }
                else if (token != null && token.ToString() == HttpContext.Current.Session[username].ToString())
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 取到用户名
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            string value = GetCookieValue();

            if (value.Contains("username=") && value.Contains(";userid="))
            {
                return value.Split(';')[0].Split('=')[1];
            }

            return "";
        }
        /// <summary>
        /// 取到用户ID
        /// </summary>
        /// <returns></returns>
        public static int GetUserId()
        {
            string value = GetCookieValue();

            if (value.Contains("username=") && value.Contains(";userid="))
            {
                int userId = 0;
                int.TryParse(value.Split(';')[1].Split('=')[1], out userId);
                return userId;
            }

            return 0;
        }
        public static string GetUserIP()
        {
            string value = GetCookieValue();
            string ip = "0.0.0.0";
            if (value.Contains("username=") && value.Contains(";userid=") && value.Contains(";ip="))
            {
                ip = value.Split(';')[2].Split('=')[1];
            }
            return ip;
        }
        /// <summary>
        /// 退出
        /// </summary>
        public static void SignOut()
        {
            var cookie = HttpContext.Current.Request.Cookies[AuthCookieName];

            if (cookie != null)
            {
                cookie.Value = "";
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        private static string GetCookieValue()
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(AuthCookieName);

            if (cookie != null)
            {
                return EncryptionHelper.DecryptBalance(cookie.Value, AuthKey);
            }

            return "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using David.Framework.Core.Cache;

namespace David.Framework.Web.Captcha
{
    public enum CaptchaDispalyType
    {
        Login = 1
    }
    public class CaptchaDisplayHelper
    {
        private static readonly string CacheKey = "David_CaptchaDisplay";
        private static readonly int CaptchaShowTime = 120; //分钟
        public static string Ip
        {
            get
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        public static bool IsDisplay(CaptchaDispalyType type)
        {
            var cacheValue = MemCache.Get(GetId(type));

            if (cacheValue == null)
                return false;
            return true;
        }

        public static void SetDisplay(CaptchaDispalyType type)
        {
            MemCache.Add(GetId(type), "CaptchaDispaly", TimeSpan.FromMinutes(CaptchaShowTime));
        }

        public static void SetHide(CaptchaDispalyType type)
        {
            MemCache.Remove(GetId(type));
        }

        private static string GetId(CaptchaDispalyType type)
        {
            return CacheKey + "_" + type.ToString() + "_" + Ip;
        }
        public static bool Check(string guid, string code)
        {
            if (String.IsNullOrEmpty(guid))
            {
                return false;
            }

            // get values
            CaptchaImage image = CaptchaImage.GetCachedCaptcha(guid);
            string expectedValue = image == null ? String.Empty : image.Text;

            if (String.IsNullOrEmpty(code) || String.IsNullOrEmpty(expectedValue) || !String.Equals(code, expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 设置错误计数
        /// </summary>
        /// <param name="count"></param>
        public static void SetErrorCount(int count)
        {
            MemCache.Add(CacheKey + "_" + "ErrorCount" + "_" + Ip, count, TimeSpan.FromMinutes(CaptchaShowTime));
        }
        /// <summary>
        /// 获取错误计数
        /// </summary>
        /// <returns></returns>
        public static string GetErrorCount()
        {
            object countObj = MemCache.Get(CacheKey + "_" + "ErrorCount" + "_" + Ip);
            if (countObj != null)
            {
                return countObj.ToString();
            }
            return null;
        }

        /// <summary>
        /// 删除错误计数
        /// </summary>
        public static void DeleteErrorCount()
        {
            MemCache.Remove(CacheKey + "_" + "ErrorCount" + "_" + Ip);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using David.Framework.Core.Cache;

namespace David.Framework.Web.Captcha
{
    public static class CaptchaImageHelper
    {
        public static void RemoveCurrentCapcha(HttpContextBase ctx)
        {
            string guid = ctx.Request.Form["captcha-guid"];
            CaptchaImage image = Captcha.CaptchaImage.GetCachedCaptcha(guid);
            image.ResetText();
            MemCache.Add(guid, image, TimeSpan.FromMinutes(5));
        }

        public static void RemoveCurrentCapcha(string key)
        {
            CaptchaImage image = Captcha.CaptchaImage.GetCachedCaptcha(key);
            if (image != null)
            {
                image.ResetText();
                MemCache.Add(key, image, TimeSpan.FromMinutes(5));
            }
        }

        public static bool Check(string guid, string code)
        {
            if (String.IsNullOrEmpty(guid))
            {
                return false;
            }

            // get values
            CaptchaImage image = Captcha.CaptchaImage.GetCachedCaptcha(guid);
            string expectedValue = image == null ? String.Empty : image.Text;

            //if(!image.IsDisplay)
            //{
            //    return true;
            //}

            // validate the captch
            if (String.IsNullOrEmpty(code) || String.IsNullOrEmpty(expectedValue) || !String.Equals(code, expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public static string CaptchaImage(this HtmlHelper helper, string guid, string id, int height, int width, string className, string inputid = "captcha-guid")
        {
            CaptchaImage image = new CaptchaImage { Height = height, Width = width };

            MemCache.Add(guid, image, TimeSpan.FromMinutes(5));

            StringBuilder stringBuilder = new StringBuilder(256);
            stringBuilder.Append("<input type=\"hidden\" id=\"" + inputid + "\" name=\"" + inputid + "\" value=\"");
            stringBuilder.Append(guid);
            stringBuilder.Append("\" />");
            stringBuilder.AppendLine();
            stringBuilder.Append("<img src=\"");
            string imgUrl = "/Captcha/Image";
            stringBuilder.Append(imgUrl + "?guid=" + guid);
            stringBuilder.Append("\" alt=\"CAPTCHA\" id=\"" + id + "\" width=\"");
            stringBuilder.Append(width);
            stringBuilder.Append("\" height=\"");
            stringBuilder.Append(height);
            stringBuilder.Append("\" class=\"");
            stringBuilder.Append(className);
            stringBuilder.Append("\" />");

            return stringBuilder.ToString();
        }

        public static string CaptchaTextBox(this HtmlHelper helper, string name, string className)
        {
            return String.Format(@"<input type=""text"" id=""{0}"" name=""{0}"" value="""" maxlength=""{1}"" maxlength=""5"" autocomplete=""off"" class=""{2}"" />",
                name,
                Captcha.CaptchaImage.TextLength,
                className
                );

        }

    }
}

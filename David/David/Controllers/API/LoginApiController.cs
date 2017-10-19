using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using David.Framework.Web.Controller;
using David.Framework.Web.Security;
using David.WebSite.Models.API;

namespace David.WebSite.Controllers.API
{
    public class LoginApiController : BaseApiController
    {
        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<object> Logout()
        {
            var result = new Response<object>();
            DavidFormsAuthentication.SignOut();
            result.data = new { result = true };
            return result;
        }

        /// <summary>
        /// 登录
        /// GET:/api/Login/Login
        /// </summary>
        [HttpPost]
        public Response<object> Login(LoginRequest loginRequest)
        {
            Response<object> result = new Response<object>();
            bool showCaptcha = false;
            if (loginRequest.account.IsNullOrEmpty())
            {
                if (!loginRequest.isheadlogin)
                {
                    showCaptcha = OprateLoginCaptcha();
                }
                result.data = new
                {
                    result = false,
                    errorcode = 1,
                    showcaptcha = showCaptcha
                };
                return result;
            }

            Regex passwordRegex = new Regex("[A-Za-z].*[0-9]|[0-9].*[A-Za-z]");
            if (loginRequest.password.IsNullOrEmpty())
            {
                if (!loginRequest.isheadlogin)
                {
                    showCaptcha = OprateLoginCaptcha();
                }
                result.data = new { result = false, errorcode = 2, showcaptcha = showCaptcha };
                return result;
            }
            if (loginRequest.password == Encryption.SHA256Encrypt(Encryption.MD5(loginRequest.password) + YOY.Const.YOYConst.PostPonySecretKey))
            {
                if (!loginRequest.isheadlogin)
                {
                    showCaptcha = OprateLoginCaptcha();
                }
                result.data = new { result = false, errorcode = 2, showcaptcha = showCaptcha };
                return result;
            }
            if (!loginRequest.isheadlogin)
            {
                if (CaptchaDisplayHelper.IsDisplay(CaptchaDispalyType.Login) && !CaptchaImageHelper.Check(loginRequest.key, loginRequest.code))
                {
                    CaptchaDisplayHelper.SetDisplay(CaptchaDispalyType.Login);
                    result.data = new { result = false, errorcode = 6, showcaptcha = true };
                    return result;
                }
            }

            ResultOfUserLoginInfoDtoUserLoginReturnEnum loginResult = UserDataService.UserLoginValidation(loginRequest.account, Encryption.SHA256Encrypt(Encryption.MD5(loginRequest.password) + YOY.Const.YOYConst.PostPonySecretKey));

            if (loginResult == null)
            {
                if (!loginRequest.isheadlogin)
                {
                    showCaptcha = OprateLoginCaptcha();
                }

                result.data = new { result = false, errorcode = 7, showcaptcha = showCaptcha };
                return result;
            }

            switch (loginResult.Status)
            {
                case UserLoginReturnEnum.Success:
                    try
                    {
                        LogDataService.Log("Save The Login userInfo", LogType.SaveUserLoginInfo, $"userid={loginResult.Data.UserId};userIp={IPHelper.GetClicentIp()};userMacAddress={CommonTool.GetMacAddress()};");
                    }
                    catch (Exception ex)
                    {
                        LogDataService.Exception("Save The Login userInfo Exception", ex.Message, ex.Source, $"userid={loginResult.Data.UserId}");
                    }

                    break;
                case UserLoginReturnEnum.Closed://账户关闭，客户登录失败
                    result.data = new { result = false, errorcode = 8 };
                    return result;

                case UserLoginReturnEnum.NullOrEmpty:
                    if (!loginRequest.isheadlogin)
                    {
                        showCaptcha = OprateLoginCaptcha();

                    }
                    result.data = new { result = false, errorcode = 9, showcaptcha = showCaptcha };
                    return result;

                case UserLoginReturnEnum.SystemError:
                    if (!loginRequest.isheadlogin)
                    {
                        showCaptcha = OprateLoginCaptcha();

                    }
                    result.data = new { result = false, errorcode = 10, showcaptcha = showCaptcha };
                    return result;

                case UserLoginReturnEnum.UnActivate:
                    LoginSuccessOprate(loginResult.Data.UserId, loginResult.Data.UserName, loginRequest.key, loginRequest.remember, loginRequest.account);
                    result.data = new { result = false, errorcode = 11, userid = loginResult.Data.UserId };
                    return result;
                case UserLoginReturnEnum.UnFound:
                    if (!loginRequest.isheadlogin)
                    {
                        showCaptcha = OprateLoginCaptcha();

                    }
                    result.data = new { result = false, errorcode = 12, showcaptcha = showCaptcha };
                    return result;
            }

            LoginSuccessOprate(loginResult.Data.UserId, loginResult.Data.UserName, loginRequest.key, loginRequest.remember, loginRequest.account);
            //播种Cookie种子
            var seed = new CookiesPlant().GetSeed();
            SowCookie("SEED", seed, false, DateTime.MaxValue);

            result.data = new { result = true };
            return result;
        }

        private string SowCookie(string name, string value, bool isForce, DateTime expiresTime)
        {
            if (HttpContext.Current.Request.Cookies.Get(name) == null || isForce)
            {
                HttpContext.Current.Response.Cookies[name].Value = value;
                HttpContext.Current.Response.Cookies[name].Expires = expiresTime;
                return value;
            }
            else
                return HttpContext.Current.Request.Cookies[name].Value;
        }

        private bool OprateLoginCaptcha()
        {
            string errountStr = CaptchaDisplayHelper.GetErrorCount();
            if (!string.IsNullOrEmpty(errountStr))
            {
                int errcount = int.Parse(errountStr);
                if (errcount >= 4)
                {
                    CaptchaDisplayHelper.SetDisplay(CaptchaDispalyType.Login);
                    CaptchaDisplayHelper.DeleteErrorCount();
                    return true;
                }
                else
                {
                    CaptchaDisplayHelper.DeleteErrorCount();
                    CaptchaDisplayHelper.SetErrorCount(errcount + 1);
                }
            }
            else
            {
                CaptchaDisplayHelper.SetErrorCount(1);
            }
            return false;

        }

        private void LoginSuccessOprate(int userid, string username, string key, bool remember, string account)
        {
            DavidFormsAuthentication.SignIn(username, userid, remember);
            //设置用户名框里显示名字
            int days = int.Parse(ConfigurationManager.AppSettings["UserAccountDisplayTime"]);

            var useraccountCookie = new HttpCookie("DisplayUserAccount", account)
            {
                Expires = DateTime.Now.AddDays(days),
                Domain = ".postpony.com"
            };
            HttpContext.Current.Response.Cookies.Add(useraccountCookie);
            //设置验证码隐藏
            CaptchaDisplayHelper.SetHide(CaptchaDispalyType.Login);
            //删除验证码值
            CaptchaImageHelper.RemoveCurrentCapcha(key);
            //删除错误计数
            CaptchaDisplayHelper.DeleteErrorCount();
        }

        /// <summary>
        /// 是否显示验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<object> CaptchaImage()
        {
            Response<object> result = new Response<object>();
            var DisplayCaptcha = CaptchaDisplayHelper.IsDisplay(CaptchaDispalyType.Login);
            result.data = DisplayCaptcha ? new { result = true } : new { result = false };
            return result;
        }

        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        [HttpGet]
        public Response<object> CaptchaImage(int height, int width)
        {
            Response<object> result = new Response<object>();
            CaptchaImage image = new CaptchaImage { Height = height, Width = width };
            var guid = Guid.NewGuid().ToString("N");
            MemCache.Add(guid, image, TimeSpan.FromMinutes(5));
            result.data = new
            {
                captchaGuid = guid,
                captchaimageSrc = $"https://www.postpony.com/Captcha/Image?guid={guid}"
            };
            return result;
        }

        /// <summary>
        /// 检查验证码是否正确
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public Response<object> CaptchaImage(string key, string code)
        {
            Response<object> result = new Response<object>();
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(code) && CaptchaImageHelper.Check(key, code))
            {
                result.data = new { result = true };
                return result;
            }
            result.data = new { result = false };
            return result;
        }
    }
}

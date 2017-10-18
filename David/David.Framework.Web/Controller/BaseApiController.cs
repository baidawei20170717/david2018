using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace David.Framework.Web.Controller
{
    public abstract class BaseApiController: ApiController
    {
        /// <summary>
        /// 判断是否登录
        /// </summary>
        public bool IsLogged
        {
            get { return DavidAuthentication.IsLogin(); }
        }
    }
}

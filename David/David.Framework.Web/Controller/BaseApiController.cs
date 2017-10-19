using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using David.Framework.Web.Security;

namespace David.Framework.Web.Controller
{
    public abstract class BaseApiController: ApiController
    {
        /// <summary>
        /// 判断是否登录
        /// </summary>
        public bool IsLogged
        {
            get { return DavidFormsAuthentication.IsLogin(); }
        }
        private int _currentUserId = -999;
        /// <summary>
        /// 当前登录用户ID
        /// </summary>
        public int CurrentUserId
        {
            get
            {
                if (_currentUserId == -999)
                {
                    _currentUserId = DavidFormsAuthentication.GetUserId();
                }

                return _currentUserId;
            }
        }
        private string _currentUserName;
        /// <summary>
        /// 当前登录用户名
        /// </summary>
        public string CurrentUserName
        {
            get { return _currentUserName ?? (_currentUserName = DavidFormsAuthentication.GetUserName()); }
        }

    }
}

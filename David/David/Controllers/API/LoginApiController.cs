using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace David.WebSite.Controllers.API
{
    public class LoginApiController : BaseApiController
    {
        // GET: api/LoginApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/LoginApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LoginApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/LoginApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/LoginApi/5
        public void Delete(int id)
        {
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<object> Logout()
        {
            var result = new Response<object>();
            PostPonyFormsAuthentication.SignOut();
            result.data = new { result = true };
            return result;
        }
    }
}

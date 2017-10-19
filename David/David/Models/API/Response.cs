using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace David.WebSite.Models.API
{
    public class Response<T>
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public bool results { get; set; }
        /// <summary>
        /// 返回码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回码描述
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 明细返回码
        /// </summary>
        public string sub_code { get; set; }
        /// <summary>
        /// 明细返回码描述
        /// </summary>
        public string sub_msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }
    }
}
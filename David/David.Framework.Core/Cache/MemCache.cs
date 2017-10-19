using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace David.Framework.Core.Cache
{
    public class MemCache
    {
        public static void Add(string key, object data) => HttpRuntime.Cache.Insert(key, data);

        public static bool Add(string key, object data, long lNumofMilliSeconds)
        {
            HttpRuntime.Cache.Insert(key, data, null, DateTime.Now.AddMilliseconds(lNumofMilliSeconds), TimeSpan.Zero);

            return true;
        }

        public static bool Add(string key, object data, TimeSpan tspan)
        {
            HttpRuntime.Cache.Insert(key, data, null, DateTime.Now.Add(tspan), TimeSpan.Zero);

            return true;
        }

        public static object Get(string key) => HttpRuntime.Cache.Get(key);

        public static IDictionary<string, object> Get(params string[] keys)
        {
            var dic = new Dictionary<string, object>();

            foreach (var key in keys)
            {
                dic.Add(key, HttpRuntime.Cache.Get(key));
            }
            return dic;
        }


        public static T Get<T>(string key)
        {
            return (T)HttpRuntime.Cache.Get(key);
        }

        public static void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public static void RemoveAll()
        {
            for (int i = 0; i < HttpRuntime.Cache.Count; i++)
            {
                HttpRuntime.Cache.Remove(HttpRuntime.Cache.GetEnumerator().Key.ToString());
            }

        }
    }
}

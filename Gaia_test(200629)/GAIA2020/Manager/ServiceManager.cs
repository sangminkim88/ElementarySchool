using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAIA2020.Manager
{
    public class ServiceManager
    {
        // 
        static Dictionary<string, object> serviceDictionary = new Dictionary<string, object>();

        /// <summary>
        /// 현재 서비스
        /// </summary>
        public static object CurrentService;

        public static void Register<T>(T service, string name = "")
        {

            serviceDictionary[typeof(T).Name + name] = service;
            //CurrentService = service;
        }


        public static T GetService<T>(string name = "")
        {
            T instance = default(T);

            if (string.IsNullOrEmpty(name)) // modified (2019-04-05:SJY -- null input parameter 대응
            {
                if (CurrentService is T)
                    return (T)CurrentService;
                else
                {
                    var prRightSvc = serviceDictionary.First(x => x.Value is T);
                    return (T)prRightSvc.Value;
                }

            }

            if (serviceDictionary.ContainsKey(typeof(T).Name + name) == true)
            {
                instance = (T)serviceDictionary[typeof(T).Name + name];

            }

            return instance;
        }


    }
}

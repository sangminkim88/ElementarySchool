using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAIA2020.Utilities
{
    public class CmdParmParser
    {
        /// <summary>
        /// 문자열로 작성된 "키=값;키=값;" 에서 키에 해당하는 값을 찾는다
        /// </summary>
        /// <param name="str">키=값 으로 작성된 문자열</param>
        /// <param name="strkey">키</param>
        /// <returns></returns>
        public static string GetValuefromKey(string vstr, string strkey, params char[] separator)
        {
            List<string> vlist = vstr.Split(separator).ToList();

            string valstr = GetValuefromKey(vlist, strkey);
            return valstr;
        }

        /// <summary>
        /// 리스트로 작성된 "키=값" 집합에서 키에 해당하는 값을 찾는다
        /// </summary>
        /// <param name="vlist">키=값 으로 작성된 리스트</param>
        /// <param name="strkey">키</param>
        /// <returns></returns>
        public static string GetValuefromKey(List<string> vlist, string strkey)
        {
            Dictionary<string, string> vdic = new Dictionary<string, string>();
            string skey, sval;
            foreach (var v in vlist)
            {
                string[] arr = v.Split('=');
                if (arr.Length == 2)
                {
                    skey = arr[0];
                    sval = arr[1];
                    vdic.Add(skey, sval);
                }
            }

            // 검색
            if (vdic.ContainsKey(strkey))
            {
                return vdic[strkey];
            }
            else
                return string.Empty;
        }

        public static string GetKeyfromString(string str)
        {
            string valstr = string.Empty;
            string[] arr = str.Split('=');
            if (arr.Length == 2)
            {
                valstr = arr[0];
            }
            return valstr.Trim();
        }
    }
}

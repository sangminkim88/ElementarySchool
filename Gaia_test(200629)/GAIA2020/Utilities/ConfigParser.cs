using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAIA2020.Utilities
{
    public class ConfigParser
    {
        /// <summary>
        /// 키=값 형식으로 저장된 텍스트형식의 설정파일을 읽고 검색하는 기능을 한다
        /// </summary>

        // 키와 값을 딕셔너리로 관리한다
        Dictionary<string, string> config;
        string configname = string.Empty;

        public ConfigParser()
        {
            config = new Dictionary<string, string>();
        }

        public ConfigParser(string strConfig) : this()
        {
            Load(strConfig);
        }

        /// <summary>
        /// 설정파일읽기
        /// </summary>
        /// <param name="strfname"></param>
        public void Load(string strConfig)
        {
            Clear();

            StreamReader sr = new StreamReader(strConfig);            
            string[] arrResourceData = sr.ReadToEnd().Split('\r');

            string line;
            foreach (string res in arrResourceData)
            {
                line = res.Trim();

                // 빈문자열
                if (string.IsNullOrEmpty(line))
                    continue;
                // 주석문자열
                if (line[0] == '#')
                    continue;
                // 문자열분리
                string[] lines = line.Split('=');
                if (2 != lines.Length)
                    continue;

                // 키와 갑을 저장
                SetValue(lines[0], lines[1]);
            }
        }

        /// <summary>
        /// Key list를 넘겨준다.
        /// </summary>
        /// <returns>키 리스트</returns>
        public List<string> GetKeys()
        {
            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, string> item in config)
            { keys.Add(item.Key); }
            return keys;
        }

        /// <summary>
        /// Value list를 넘겨준다.
        /// </summary>
        /// <returns>값 리스트</returns>
        public List<string> GetValues()
        {
            List<string> values = new List<string>();
            foreach (KeyValuePair<string, string> item in config)
            { values.Add(item.Value); }
            return values;
        }

        /// <summary>
        /// 키로 값을 탐색한다
        /// </summary>
        /// <param name="keystr">키</param>
        /// <returns>값</returns>
        public string GetValue(string keystr)
        {
            keystr = keystr.Trim();
            if (config.ContainsKey(keystr))
                return config[keystr];
            else
                return string.Empty;
        }

        /// <summary>
        /// 값으로 키를 탐색한다
        /// </summary>
        /// <param name="valstr">값</param>
        /// <returns>키 리스트</returns>
        public List<string> GetKeys(string valstr)
        {
            valstr = valstr.Trim();
            if (config.ContainsValue(valstr))
                return (from entry in config where entry.Value == valstr select entry.Key).ToList();
            else
                return new List<string>();
        }

        /// <summary>
        /// string 값을 color로 변환
        /// </summary>
        /// <param name="valstr">값</param>
        /// <returns></returns>
        public System.Drawing.Color GetValueAsColor(string keystr)
        {
            string valstr = GetValue(keystr);
            return System.Drawing.ColorTranslator.FromHtml(valstr);
        }

        /// <summary>
        /// 키와 값을 저장한다
        /// </summary>
        /// <param name="keystr">키</param>
        /// <param name="valstr">값</param>
        /// <returns></returns>
        public bool SetValue(string keystr, string valstr)
        {
            bool flag;
            // 정상문자열
            keystr = keystr.Trim();
            valstr = valstr.Trim();
            flag = config.ContainsKey(keystr);
            if (flag)
            {
                // 업데이트
                config[keystr] = valstr;
            }
            else
            {
                // 추가
                config.Add(keystr, valstr);
            }

            return flag;
        }

        /// <summary>
        /// 주어진 파일이름으로 설정을 저장한다
        /// </summary>
        /// <param name="strfname"></param>
        /// <returns></returns>
        public bool SaveAs(string strfname)
        {
            using (StreamWriter sw = new StreamWriter(strfname))
            {
                //List<string> vlist = new List<string>();
                string strline;
                foreach (KeyValuePair<string, string> v in config)
                {
                    strline = string.Format("{0}={1}", v.Key, v.Value);
                    //vlist.Add(strline);
                    sw.WriteLine(strline);
                }
            }

            return true;
        }
        public bool Save()
        {
            if (!string.IsNullOrEmpty(configname))
                return false;
            if (!File.Exists(configname))
                return false;

            return SaveAs(configname);
        }

        /// <summary>
        /// 청소
        /// </summary>
        private void Clear()
        {
            config.Clear();
        }

        /// <summary>
        /// 모든 값을 split하고 중복 확인 하고 넘겨준다.
        /// </summary>
        /// <param name="c">char 값</param>
        /// <returns></returns>
        public List<string> GetValuesBySplit(char c)
        {
            List<string> strResults = new List<string>();

            List<string> strVals = GetValues();
            foreach (string strVal in strVals)
            {
                List<string> strVals2 = SplitValue(strVal, c);
                foreach (string strVal2 in strVals2)
                {
                    if (!strResults.Contains(strVal2))
                    { strResults.Add(strVal2); }
                }
            }
            return strResults;
        }

        /// <summary>
        /// 키에 해당 값을 char으로 split 해준다.
        /// </summary>
        /// <param name="strKey">키</param>
        /// <param name="c">char 값</param>
        /// <returns></returns>
        public List<string> GetValueBySplit(string strKey, char c)
        {
            string strVal = GetValue(strKey);
            return SplitValue(strVal, c);
        }

        /// <summary>
        /// 값을 char으로 split 해준다.
        /// </summary>
        /// <param name="strVal">값</param>
        /// <param name="c">char 값</param>
        /// <returns></returns>
        private List<string> SplitValue(string strVal, char c)
        {
            List<string> strResults = new List<string>();

            if (strVal.Contains(c))
            {
                string[] values = strVal.Split(c);
                foreach (string val in values)
                {
                    if (!strResults.Contains(val))
                    { strResults.Add(val); }
                }
            }
            else
            { strResults.Add(strVal); }

            return strResults;
        }
    }
}

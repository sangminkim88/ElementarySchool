namespace GAIA2020.Utilities
{
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the <see cref="StringUtil" />.
    /// </summary>
    public static class StringUtil
    {
        #region Methods

        /// <summary>
        /// 소수점 이하 자릿수 검사.
        /// </summary>
        /// <param name="length">최대 자릿수<see cref="int"/>.</param>
        /// <param name="str">The str<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CheckDecimalPlace(int length, string str)
        {
            if (IsNumeric(str))
            {
                if (str.Contains('.'))
                {
                    if (str.Substring(str.IndexOf('.')).Length - 1 > length)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The GetFirstIntFromString.
        /// </summary>
        /// <param name="input">The input<see cref="string"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int GetFirstIntFromString(string input)
        {
            string tmp = input;
            while (true)
            {
                if (tmp.Length.Equals(0)) return -1;

                string tmp2 = new string(tmp.TakeWhile(char.IsNumber).ToArray());
                if (!tmp2.Equals(string.Empty))
                {
                    return int.Parse(tmp2);
                }
                tmp = tmp.Replace(new string(tmp.TakeWhile(char.IsLetter).ToArray()), string.Empty);
            }
        }

        /// <summary>
        /// The GetNumeric.
        /// </summary>
        /// <param name="str">The str<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetNumeric(string str)
        {
            return Regex.Replace(str, @"\D", string.Empty);
        }

        /// <summary>
        /// 구분자의 n번째를 기준으로 나누어 리턴
        /// 구분자의 수보다 n값이 크다면 빈 문자열 반환
        /// </summary>
        /// <param name="input">대상 문자열<see cref="string"/>.</param>
        /// <param name="divideChar">구분자<see cref="char"/>.</param>
        /// <param name="n">시작은 1부터<see cref="int"/>.</param>
        /// <param name="isBefore">n번째 구분자를 기준으로 앞쪽은 true, 뒷쪽은 false<see cref="bool"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetSubstringDividedNIndex(string input, char divideChar, int n, bool isBefore)
        {
            string[] dividedString = input.Split(divideChar);
            if (dividedString.Length > n)
            {
                StringBuilder sb = new StringBuilder();
                if (isBefore)
                {
                    for (int i = 0; i < n; i++)
                    {
                        sb.Append(dividedString[i]);
                    }
                }
                else
                {
                    for (int i = n; i < dividedString.Length; i++)
                    {
                        sb.Append(dividedString[i]);
                    }
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// The IsNumeric.
        /// </summary>
        /// <param name="str">The str<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsNumeric(string str)
        {
            Regex regex = new Regex(@"^-?[0-9]*(?:\.[0-9]*)?$");
            return regex.IsMatch(str);
        }

        #endregion
    }
}

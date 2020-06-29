namespace GAIA2020.Utilities
{
    using GAIA2020.Enums;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="ConfigHelper" />.
    /// </summary>
    public static class ConfigHelper
    {
        #region Methods

        /// <summary>
        /// The GetConfigFileName.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetConfigFileName()
        {
            return Path.ChangeExtension(Path.GetFileName(Assembly.GetExecutingAssembly().Location), GaiaConstants.CONFIG_EXTENSION);
        }

        /// <summary>
        /// The GetDefaultDataFilePath.
        /// </summary>
        /// <param name="strfname">The strfname<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetDefaultDataFilePath(string strfname = "")
        {
            string returnData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                GaiaConstants.PROG_CODE_NAME, strfname);

            if (!Directory.Exists(returnData))
            {
                Directory.CreateDirectory(returnData);
            }

            return returnData;
        }

        /// <summary>
        /// The GetExeuteFileName.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetExeuteFileName()
        {
            return Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// The GetExeuteFilePath.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetExeuteFilePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// 문서\HANMAC\GAIA + \'strfname' 
        /// </summary>
        /// <param name="strfname">The strfname<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetProgramFilePath(string strfname = "")
        {
            string returnData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                GaiaConstants.PROG_CATEGORY_NAME, GaiaConstants.PROG_CODE_NAME, strfname);

            if (!Directory.Exists(Path.GetDirectoryName(returnData)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(returnData));
            }

            if (!Path.GetExtension(returnData).Equals(string.Empty) && !File.Exists(returnData))
            {
                using (FileStream fs = File.Create(returnData))
                {
                    // 다음 사용을 위해 파일닫기
                    fs.Dispose();
                }
            }

            return returnData;
        }

        /// <summary>
        /// The MakeConfig.
        /// </summary>
        public static void MakeConfig()
        {
            string configPath = GetProgramFilePath(GetConfigFileName());

            if (!File.Exists(configPath))
            {
                string path = GetProgramFilePath();
                WriteProfileString(eConfigSection.Default, eConfigKey.Directory, path);
            }
        }

        /// <summary>
        /// The ReadProfileString.
        /// </summary>
        /// <param name="section">The section<see cref="eConfigSection"/>.</param>
        /// <param name="key">The key<see cref="eConfigKey"/>.</param>
        /// <param name="defaultValue">The defaultValue<see cref="string"/>.</param>
        /// <param name="size">The size<see cref="int"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string ReadProfileString(eConfigSection section, eConfigKey key, string defaultValue, int size = 256)
        {
            StringBuilder strtemp = new StringBuilder(size);
            int r = GetPrivateProfileString(section.ToString(), key.ToString(), defaultValue, strtemp, size, GetProgramFilePath(GetConfigFileName()));
            if (r > 0)
                return strtemp.ToString();
            else
                return defaultValue;
        }

        /// <summary>
        /// The WriteProfileString.
        /// </summary>
        /// <param name="section">The section<see cref="eConfigSection"/>.</param>
        /// <param name="key">The key<see cref="eConfigKey"/>.</param>
        /// <param name="val">The val<see cref="string"/>.</param>
        /// <returns>The <see cref="long"/>.</returns>
        public static long WriteProfileString(eConfigSection section, eConfigKey key, string val)
        {
            return WritePrivateProfileString(section.ToString(), key.ToString(), val, GetProgramFilePath(GetConfigFileName()));
        }

        /// <summary>
        /// The GetPrivateProfileString.
        /// </summary>
        /// <param name="section">The section<see cref="String"/>.</param>
        /// <param name="key">The key<see cref="String"/>.</param>
        /// <param name="def">The def<see cref="String"/>.</param>
        /// <param name="retVal">The retVal<see cref="StringBuilder"/>.</param>
        /// <param name="Size">The Size<see cref="int"/>.</param>
        /// <param name="filePath">The filePath<see cref="String"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int Size, String filePath);

        /// <summary>
        /// The WritePrivateProfileString.
        /// </summary>
        /// <param name="section">The section<see cref="String"/>.</param>
        /// <param name="key">The key<see cref="String"/>.</param>
        /// <param name="val">The val<see cref="String"/>.</param>
        /// <param name="filePath">The filePath<see cref="String"/>.</param>
        /// <returns>The <see cref="long"/>.</returns>
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(String section, String key, String val, String filePath);

        #endregion
    }
}

namespace FileManager
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public static class ConfigManager
    {
        #region Methods

        public static string ReadProfileString(string section, string key, string def, int size = 256)
        {
            StringBuilder strtemp = new StringBuilder(size);
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ryan", "Config");
            string fileName = Path.Combine(Assembly.GetEntryAssembly().GetName().Name + ".cfg");

            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
            }

            string filePath = Path.Combine(defaultPath, fileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            int r = GetPrivateProfileString(section, key, def, strtemp, size, filePath);
            if (r > 0)
                return strtemp.ToString();
            else
                return def;
        }

        public static long WriteProfileString(string section, string key, string val)
        {
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ryan", "Config");
            string fileName = Path.Combine(Assembly.GetEntryAssembly().GetName().Name + ".cfg");
            string filePath = Path.Combine(defaultPath, fileName);

            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
            }
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            long r = WritePrivateProfileString(section, key, val, filePath);
            return r;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int Size, String filePath);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(String section, String key, String val, String filePath);

        #endregion
    }
}

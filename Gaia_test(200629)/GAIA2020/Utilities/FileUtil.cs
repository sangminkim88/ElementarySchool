namespace GAIA2020.Utilities
{
    using System.IO;

    /// <summary>
    /// Defines the <see cref="StringUtil" />.
    /// </summary>
    public static class FileUtil
    {
        #region Methods

        /// <summary>
        /// The FolderCreate.
        /// </summary>
        /// <param name="strpath">The strpath<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CreateFolder(string strpath)
        {
            try
            {
                if (!Directory.Exists(strpath))
                {
                    Directory.CreateDirectory(strpath);
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// The GetCheckedDuplicateFilePath.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <param name="extension">The extension<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetCheckedDuplicateFilePath(string path, string fileName, string extension)
        {
            try
            {
                // ('\', ' / ', ':', ' * ', ' ? ', '"', '<', '>', '|') 파일명 룰
                string changedFileName = fileName.Replace('\\', '_').Replace('/', '_').Replace(':', '_').
                    Replace('*', '_').Replace('?', '_').Replace('"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_');
                string fullPath = Path.Combine(path, changedFileName + extension);
                int i = 0;
                while (true)
                {
                    if (File.Exists(fullPath))
                    {
                        fullPath = Path.Combine(path, changedFileName + " (" + i++ + ")" + extension);
                    }
                    else
                    {
                        break;
                    }
                }
                return fullPath;
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}

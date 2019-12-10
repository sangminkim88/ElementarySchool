namespace FileManager
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public static class XmlManager
    {
        #region Methods

        public static Object Deserialize(string strFileName, Type i_type)
        {
            FileStream fs = null;
            XmlReader reader = null;
            try
            {
                if (!File.Exists(strFileName))
                {
                    File.Create(strFileName);
                }
                XmlSerializer xSerializer = new XmlSerializer(i_type);

                fs = new FileStream(strFileName, FileMode.Open);
                reader = XmlReader.Create(fs);

                Object tempObj = xSerializer.Deserialize(reader);

                return tempObj;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (fs != null) fs.Close();
            }
        }

        public static bool Serialize(Object i_object, string strFileName)
        {
            StreamWriter stream = null;
            try
            {
                stream = new StreamWriter(strFileName);
                XmlSerializer xSerializer = new XmlSerializer(i_object.GetType());
                xSerializer.Serialize(stream, i_object);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }

        #endregion
    }
}

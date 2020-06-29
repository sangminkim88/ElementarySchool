using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LogStyle
{
    public class DeepCopy
    {
        // 1. Deep Clone 구현
        public static T DeepClone<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null.");

            return (T)Process(obj, new Dictionary<object, object>() { });
        }

        private static object Process(object obj, Dictionary<object, object> circular)
        {
            if (obj == null)
                return null;            

            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            if (type.IsArray)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj];

                string typeNoArray = type.FullName.Replace("[]", string.Empty);
                Type elementType = Type.GetType(typeNoArray + ", " + type.Assembly.FullName);
                var array = obj as Array;
                Array arrCopied = Array.CreateInstance(elementType, array.Length);

                circular[obj] = arrCopied;

                for (int i = 0; i < array.Length; i++)
                {
                    object element = array.GetValue(i);
                    object objCopy = null;

                    if (element != null && circular.ContainsKey(element))
                        objCopy = circular[element];
                    else
                        objCopy = Process(element, circular);

                    arrCopied.SetValue(objCopy, i);
                }

                return Convert.ChangeType(arrCopied, obj.GetType());
            }

            if (type.IsClass)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj];

                object objValue = Activator.CreateInstance(obj.GetType());
                circular[obj] = objValue;
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);

                    if (fieldValue == null)
                        continue;

                    object objCopy = circular.ContainsKey(fieldValue) ? circular[fieldValue] : Process(fieldValue, circular);
                    field.SetValue(objValue, objCopy);
                }

                return objValue;
            }
            else
                throw new ArgumentException("Unknown type");
        }


        // 2. Serializable 객체에 대한  Deep Clone
        public static T SerializableDeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var bformatter = new BinaryFormatter();
                bformatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)bformatter.Deserialize(ms);
            }
        }



        // 출처: https://bemeal2.tistory.com/64 [헝그리개발자]
    }
}

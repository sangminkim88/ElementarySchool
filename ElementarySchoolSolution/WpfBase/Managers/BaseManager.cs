namespace WpfBase.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BaseManager<T> where T : class
    {
        #region Fields

        private static Dictionary<Type, T> Keys;

        #endregion

        #region Constructors

        static BaseManager()
        {
            Keys = new Dictionary<Type, T>();
        }

        #endregion

        #region Methods

        public static bool AddValue(Type type, T v)
        {
            if (!Keys.ContainsKey(type))
            {
                // 없으면 추가
                Keys.Add(type, v);
                return true;
            }
            else
                return false;
        }

        public static bool RemoveValue(Type type)
        {
            if (Keys.ContainsKey(type))
            {
                // 없으면 추가
                Keys[type] = null;
                Keys.Remove(type);
                return true;
            }
            else
                return false;
        }

        public static void Cleanup()
        {
            foreach (var v in Keys.ToList())
            {
                Keys[v.Key] = null;
            }
            Keys.Clear();
        }

        public static T GetValue(Type type, bool create = true)
        {
            T v = default(T);
            if (Keys.ContainsKey(type))
            {
                v = Keys[type];
                if (null == v)
                {
                    v = Activator.CreateInstance(type) as T;
                    Keys[type] = v;
                }
            }
            else if (create)
            {
                // 없으면 생성
                v = Activator.CreateInstance(type) as T;
                if (!Keys.ContainsKey(type))
                {
                    Keys.Add(type, v);
                }
            }

            return v;
        }

        #endregion
    }
}

namespace GAIA2020.Utilities
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Defines the <see cref="StringUtil" />.
    /// </summary>
    public static class EnumUtil
    {
        #region Methods

        /// <summary>
        /// The GetEnumDescription.
        /// </summary>
        /// <param name="value">The value<see cref="Enum"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        //public static TEnum GetEnumFromDescription<TEnum>(string value, TEnum defaultValue = default(TEnum))
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());

        //    DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        //    if (attributes != null && attributes.Any())
        //    {
        //        return attributes.First().Description;
        //    }

        //    return value.ToString();
        //}
        public static TEnum GetEnumFromDescription<TEnum>(string description, TEnum defaultValue = default(TEnum))
        {
            var type = typeof(TEnum);

            if (!typeof(TEnum).IsEnum)
            {
                Console.WriteLine(typeof(TEnum).ToString() + "is not an enumerated type\n");
            }

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (TEnum)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (TEnum)field.GetValue(null);
                }
            }
            return defaultValue;
        }

        public static string GetDescriptionFromString<T>(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();

            if (name == null)
            {
                return string.Empty;
            }

            var customAttribute = type.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);

            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }

        /// <summary>
        /// The ParseEnum.
        /// </summary>
        /// <typeparam name="TEnum">.</typeparam>
        /// <param name="value">The value<see cref="string"/>.</param>
        /// <param name="ignoreCase">The ignoreCase<see cref="bool"/>.</param>
        /// <param name="defaultValue">The defaultValue<see cref="TEnum"/>.</param>
        /// <returns>The <see cref="TEnum"/>.</returns>
        public static TEnum GetEnumFromSting<TEnum>(this string value, bool ignoreCase = true, TEnum defaultValue = default(TEnum))
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                Console.WriteLine(typeof(TEnum).ToString() + "is not an enumerated type\n");
            }

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            TEnum lResult;
            if (Enum.TryParse(value, ignoreCase, out lResult))
            {
                return lResult;
            }
            else
            {
                return defaultValue;
            }
        }

        #endregion
    }
}

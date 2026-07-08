using System;
using System.ComponentModel;

namespace Magicat.Helpers
{
    public static class Converter
    {
        /// <summary>
        /// Converts any string value to a specified type T.
        /// returns default value if conversion fails for any reason.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(string value)
        {
            if (value is T variable) 
            {
                return variable;
            }

            try
            {
                //Handling Nullable types i.e, int?, double?, bool? .. etc
                if (Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Attempts to convert any string vlaue to a specified type T.
        /// returns false if the conversion fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="converted"></param>
        /// <returns></returns>
        public static bool TryConvertTo<T>(string value, out T converted)
        {
            if (value is T variable)
            {
                converted = variable;
                return true;
            }

            try
            {
                //Handling Nullable types i.e, int?, double?, bool? .. etc
                if (Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    converted = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
                    return true;
                }

                converted = (T)Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (Exception)
            {
                converted = default(T);
                return false;
            }
        }
    }
}

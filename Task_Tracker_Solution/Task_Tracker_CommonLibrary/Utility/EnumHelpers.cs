using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Task_Tracker_CommonLibrary.Utility
{
    public static class EnumHelpers
    {
        public static Expected GetAttributeValue<T, Expected>(this Enum enumeration, Func<T, Expected> expression)
    where T : Attribute
        {
            T attribute =
              enumeration
                .GetType()
                .GetMember(enumeration.ToString())
                .Where(member => member.MemberType == MemberTypes.Field)
                .FirstOrDefault()
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .SingleOrDefault();

            if (attribute == null)
                return default(Expected);

            return expression(attribute);
        }

        public static string GetDescription(this Enum enumeration)
        {
            if (enumeration == null)
                return null;
            return enumeration.GetAttributeValue<DescriptionAttribute, string>(x => x.Description);
        }
        public static string GetEnumMemberVal(this Enum enumeration)
        {
            if (enumeration == null)
                return null;
            return enumeration.GetAttributeValue<EnumMemberAttribute, string>(x => x.Value);
        }

        public static string ToJSONString(this Type enumType)
        {
            Type t = enumType;//enumType.GetType();
            if (t != null && t.IsEnum)
            {
                var values = Enum.GetValues(t).Cast<int>();
                var enumDictionary = values.ToDictionary(value => Enum.GetName(t, value));
                return JsonConvert.SerializeObject(enumDictionary);
            }
            else
                return "This funtion only supports Enum type to Json string.";
        }

        public static TEnum ConvertToEnum<TEnum>(this string value, TEnum defaultVal = default(TEnum)) where TEnum : struct
        {
            TEnum tmp;

            if (!Enum.TryParse<TEnum>(value, true, out tmp))
            {
                tmp = defaultVal;
            }
            return tmp;
        }
        #region Use Below for performance for converting string value to enum later on
        //public static bool TryParse<T>(string value, out T result)
        //where T : struct
        //{
        //    var cacheKey = "Enum_" + typeof(T).FullName;

        //    // [Use MemoryCache to retrieve or create&store a dictionary for this enum, permanently or temporarily.
        //    // [Implementation off-topic.]
        //    var enumDictionary = CacheHelper.GetCacheItem(cacheKey, CreateEnumDictionary<T>, EnumCacheExpiration);

        //    return enumDictionary.TryGetValue(value.Trim(), out result);
        //}

        //private static Dictionary<string, T> CreateEnumDictionary<T>()
        //{
        //    return Enum.GetValues(typeof(T))
        //        .Cast<T>()
        //        .ToDictionary(value => value.ToString(), value => value, StringComparer.OrdinalIgnoreCase);
        //}

        //private static Dictionary<Type, Dictionary<string, object>> dicEnum = new Dictionary<Type, Dictionary<string, object>>();
        //public static T ToEnum<T>(this string value, T defaultValue)
        //{
        //    var t = typeof(T);
        //    Dictionary<string, object> dic;
        //    if (!dicEnum.ContainsKey(t))
        //    {
        //        dic = new Dictionary<string, object>();
        //        dicEnum.Add(t, dic);
        //        foreach (var en in Enum.GetValues(t))
        //            dic.Add(en.ToString(), en);
        //    }
        //    else
        //        dic = dicEnum[t];
        //    if (!dic.ContainsKey(value))
        //        return defaultValue;
        //    else
        //        return (T)dic[value];
        //} 
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NWJ.Cmn
{
    public class CommonEnum
    {
       
        public enum TemplateType
        {
            [Description("春")]
            Spring = 1,
            [Description("夏")]
            Summer = 2,
            [Description("秋")]
            Autumn = 3,
            [Description("冬")]
            Winter = 4
        }
        public static Dictionary<int, string> GetAuditState()
        {
            return GetStatus(typeof(TemplateType));
        }
        public static string GetAuditState(object v)
        {
            return GetDescription(typeof(TemplateType), v);
        }
        #region 工具函数
        private static Dictionary<int, string> GetStatus(System.Type t)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();

            Array a = System.Enum.GetValues(t);
            for (int i = 0; i < a.Length; i++)
            {
                string enumName = a.GetValue(i).ToString();
                int enumKey = (int)System.Enum.Parse(t, enumName);
                string enumDescription = GetDescription(t, enumKey);
                list.Add(enumKey, enumDescription);
            }
            return list;
        }


        private static string GetName(System.Type t, object v)
        {
            try
            {
                return System.Enum.GetName(t, v);
            }
            catch
            {
                return "";
            }
        }

        private static Dictionary<string, string> GetStatuNames(System.Type t)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            Array a = System.Enum.GetValues(t);
            for (int i = 0; i < a.Length; i++)
            {
                string enumName = a.GetValue(i).ToString();
                int enumKey = (int)System.Enum.Parse(t, enumName);
                string enumDescription = GetDescription(t, enumKey);
                list.Add(enumName, enumDescription);
            }
            return list;
        }


        /// <summary>
        /// 返回指定枚举类型的指定值的描述
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="v">枚举值</param>
        /// <returns></returns>
        private static string GetDescription(System.Type t, object v)
        {
            try
            {
                FieldInfo fi = t.GetField(GetName(t, v));
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : GetName(t, v);
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
}

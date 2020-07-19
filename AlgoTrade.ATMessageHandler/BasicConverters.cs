using System;

namespace MessageHandler
{
    public class BasicConverters
    {
        public static string ConvertSimpleToString(object pValue, Type pEnumType = null)
        {
            return pValue.ToString();
        }

        public static object ConvertStringToInt(string pStrValue, Type pEnumType = null)
        {
            return int.Parse(pStrValue);
        }

        public static string ConvertPriceToString(object pValue, Type pEnumType = null)
        {
            return ((double)pValue).ToString("0000.00");
        }

        public static object ConvertStringToPrice(string pStrValue, Type pEnumType = null)
        {
            return double.Parse(pStrValue);
        }

        public static string ConvertEnumToString(object pValue, Type pEnumType = null)
        {
            return Enum.GetName(pEnumType, pValue);
        }

        public static object ConvertStringToEnum(string pStrValue, Type pEnumType = null)
        {
            return Enum.Parse(pEnumType, pStrValue);
        }
    }
}
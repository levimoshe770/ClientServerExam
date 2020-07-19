using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using static System.String;

namespace MessageHandler
{
    public class MessageConverter
    {
        public static string GetMessageId(byte[] pBuffer)
        {
            string message = Encoding.ASCII.GetString(pBuffer);

            return message.Split(',')[0];
        }
    }

    public class MessageConverter<T> where T : class, new()
    {
        #region Public

        #region Methods static

        public static T RawMessageToObject(byte[] pBuffer)
        {
            string messageString = Encoding.ASCII.GetString(pBuffer);

            //Logger.LoggerSingle.Debug("Msg To Obj: {0}", messageString);

            return StringMessageToObject(messageString);
        }

        public static byte[] ObjectToRawMessage(T pObject)
        {
            string messageString = ObjectToMessageString(pObject);

            //Logger.LoggerSingle.Debug("Obj to Msg: {0}", messageString);

            return Encoding.ASCII.GetBytes(messageString);
        }

        /// <summary>
        /// Message has the following format:
        /// [ClassName],[propety=value],[property=value]....
        /// </summary>
        /// <param name="pMessage"></param>
        /// <returns></returns>
        public static T StringMessageToObject(string pMessage)
        {
            string[] fields = pMessage.Split(',');
            string className = fields[0];
            string messageName;

            if (!MessageClassAtributeFound(out messageName))
            {
                Logger.Logger.Log(string.Format("Message {0} must use MessageClass attribute", typeof(T).Name));
                return null;
            }

            if (CompareOrdinal(className, messageName) != 0)
            {
                Logger.Logger.Log(string.Format("Got message {0} for incompatible class {1}", className, messageName));

                return null;
            }

            T res = new T();

            for (int i = 1; i < fields.Length; i++)
            {
                string[] prop = fields[i].Split('=');

                string property = prop[0];
                string value = prop[1];

                PropertyInfo propertyInfo = typeof(T).GetProperty(property);
                if (propertyInfo == null)
                {
                    Logger.Logger.Log(string.Format("Illegal property {0} in message {1}",property,className));

                    return null;
                }

                Attribute[] attributes = (Attribute[]) propertyInfo.GetCustomAttributes();
                string format;
                MessageFieldTypesEn fieldType;
                if (!MessageFieldAttributeFound(attributes, out format, out fieldType))
                {
                    Logger.Logger.Log("Property {0} must have a <MessageField> attribute");
                    return null;
                }

                propertyInfo.SetValue(res, ConvertToValue(value, format, propertyInfo.PropertyType));
            }

            return res;
        }

        public static string ObjectToMessageString(T pObject)
        {
            string res;

            if (!MessageClassAtributeFound(out res))
                return Empty;

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                string format;
                MessageFieldTypesEn fieldType;
                if (!MessageFieldAttributeFound((Attribute[]) propertyInfo.GetCustomAttributes(),
                    out format, out fieldType))
                    continue;

                switch (fieldType)
                {
                    case MessageFieldTypesEn.Scalar:
                        res += ",";
                        res += propertyInfo.Name + "=" + ConvertToString(propertyInfo.GetValue(pObject), format,
                                   propertyInfo.PropertyType);
                        break;
                    //case MessageFieldTypesEn.Array:
                    //    res += ",";
                    //    res += propertyInfo.Name + "=";

                    //    break;

                }
            }

            return res;
        }

        #endregion

        #endregion

        #region Private

        #region Methods

        private static object ConvertToValue(string pValue, string pFormat, Type pType)
        {
            if (pType.IsEnum)
                return Enum.Parse(pType, pValue);

            if (pType.UnderlyingSystemType == typeof(int))
                return int.Parse(pValue);

            if (pType.UnderlyingSystemType == typeof(double))
                return double.Parse(pValue);

            if (pType.UnderlyingSystemType == typeof(decimal))
                return decimal.Parse(pValue);

            if (pType.UnderlyingSystemType == typeof(float))
                return float.Parse(pValue);

            if (pType.UnderlyingSystemType == typeof(DateTime))
                return DateTime.ParseExact(pValue, pFormat, CultureInfo.CurrentCulture);

            if (pType.UnderlyingSystemType == typeof(bool))
                return bool.Parse(pValue);

            if (pType.IsArray)
            {
                string[] members = pValue.Split('+');

                object[] res = new object[members.Length];

                for (int i = 0; i < members.Length; i++)
                {
                    res[i] = ConvertToValue(members[i], pFormat, pType.GetElementType());
                }

                return res;
            }

            return pValue;
        }

        private static string ConvertToString(object pValue, string pFormat, Type pType)
        {
            if (pType.IsEnum)
                return Enum.GetName(pType, pValue);

            if (pFormat != Empty)
            {
                if (pType.UnderlyingSystemType == typeof(int))
                    return ((int) pValue).ToString(pFormat);

                if (pType.UnderlyingSystemType == typeof(double))
                    return ((double) pValue).ToString(pFormat);

                if (pType.UnderlyingSystemType == typeof(decimal))
                    return ((decimal) pValue).ToString(pFormat);

                if (pType.UnderlyingSystemType == typeof(float))
                    return ((float) pValue).ToString(pFormat);

                if (pType.UnderlyingSystemType == typeof(DateTime))
                    return ((DateTime) pValue).ToString(pFormat);

                if (pType.UnderlyingSystemType == typeof(bool))
                    return ((bool) pValue).ToString();

                if (pType.IsArray)
                {
                    object[] array = (object[]) pValue;
                    Type arrayType = pType.GetElementType();

                    string res = "";

                    for (int i = 0; i < array.Length; i++)
                    {
                        res += "+" + ConvertToString(array[i], pFormat, arrayType);
                    }

                    return res;
                }

            }

            return pValue.ToString();
        }

        private static bool MessageClassAtributeFound(out string pMessageName)
        {
            pMessageName = "";
            bool found = false;
            foreach (Attribute attribute in typeof(T).GetCustomAttributes())
            {
                if (attribute is MessageClassAttribute)
                {
                    found = true;
                    pMessageName = ((MessageClassAttribute) attribute).MessageName;
                    break;
                }
            }
            return found;
        }

        private static bool MessageFieldAttributeFound(
            Attribute[] pAttributes, 
            out string pFormat,
            out MessageFieldTypesEn pFieldType)
        {
            bool attributeFound = false;
            pFormat = Empty;
            pFieldType = MessageFieldTypesEn.Scalar;

            foreach (Attribute attribute in pAttributes)
            {
                if (attribute is MessageFieldAttribute)
                {
                    attributeFound = true;
                    pFormat = ((MessageFieldAttribute) attribute).Format;
                    pFieldType = ((MessageFieldAttribute) attribute).FieldType;
                    break;
                }
            }

            return attributeFound;
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageHandler
{
    public delegate string ConvertToStringDlg(object pValue, Type pEnumType = null);

    public delegate object ConvertToValueDlg(string pStrValue, Type pEnumType = null);

    [AttributeUsage(AttributeTargets.Class)]
    public class MessageClassAttribute : Attribute
    {
        public MessageClassAttribute(string pMessageName) : this(pMessageName, false)
        {
        }

        public MessageClassAttribute(string pMessageName, bool pBlob)
        {
            MessageName = pMessageName;
            Blob = pBlob;
        }

        public string MessageName { get; set; }
        public bool Blob { get; set; }
    }

    public enum MessageFieldTypesEn
    {
        Scalar,
        Group,
        Array,
        Blob
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MessageFieldAttribute : Attribute
    {
        public MessageFieldAttribute() : this(MessageFieldTypesEn.Scalar, string.Empty)
        {
        }

        public MessageFieldAttribute(MessageFieldTypesEn pFieldType, string pFormat)
        {
            FieldType = pFieldType;
            Format = pFormat;
        }

        public MessageFieldAttribute(MessageFieldTypesEn pFieldType) : this(pFieldType, string.Empty)
        {
        }

        public MessageFieldAttribute(string pFormat) : this(MessageFieldTypesEn.Scalar, pFormat)
        {
        }

        //public MessageFieldAttribute(Type pArrayType, string pFormat) :
        //    this(pFormat)
        //{
        //    ArrayType = pArrayType;
        //}

        //public MessageFieldAttribute(Type pArrayType) : this(string.Empty)
        //{
        //    ArrayType = pArrayType;
        //}

        public string Format { get; set; }
        public MessageFieldTypesEn FieldType { get; set; }
        //public Type ArrayType { get; set; }
    }
}

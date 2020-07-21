using MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlMessages
{
    [MessageClass("BlobMsg", true)]
    public class BlobMsg
    {
        [MessageField(MessageFieldTypesEn.Blob)]
        public byte[] Blob { get; set; }
    }
}

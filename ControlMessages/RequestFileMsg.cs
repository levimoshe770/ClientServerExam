using MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlMessages
{
    [MessageClass("TransferFileMsg")]
    public class RequestFileMsg
    {
        [MessageField]
        public string File { get; set; }
    }
}

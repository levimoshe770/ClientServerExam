using MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlMessages
{
    [MessageClass("FileListBegin")]
    public class FileListBegin
    {
    }

    [MessageClass("FileMsg")]
    public class FileMsg
    {
        [MessageField]
        public string FileName { get; set; }
    }

    [MessageClass("FileListEnd")]
    public class FileListEnd
    {

    }
}

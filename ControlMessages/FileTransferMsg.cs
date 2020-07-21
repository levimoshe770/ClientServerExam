using MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlMessages
{
    [MessageClass("FileTransferMsg")]
    public class FileTransferMsg
    {
        [MessageField]
        public string Name { get; set; }

        [MessageField]
        public long Size { get; set; }

        [MessageField]
        public long NumOfChunks { get; set; }

        [MessageField]
        public int ChunkSize { get; set; }
    }
}

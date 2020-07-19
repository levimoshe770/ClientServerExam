using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageHandler;
using System.Threading.Tasks;

namespace ControlMessages
{
    [MessageClass("CreateUserMessage")]
    public class CreateUserMessage
    {
        [MessageField]
        public string UserName { get; set; }

        [MessageField]
        public string Password { get; set; }

        [MessageField]
        public string HomePath { get; set; }

    }
}

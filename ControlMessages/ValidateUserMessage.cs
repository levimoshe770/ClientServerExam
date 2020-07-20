using MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ControlMessages
{
    [MessageClass("ValidateUserMessage")]
    public class ValidateUserMessage
    {
        [MessageField]
        public string UserName { get; set; }

        [MessageField]
        public string Password { get; set; }

    }
}

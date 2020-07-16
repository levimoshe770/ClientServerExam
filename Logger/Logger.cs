using System;
using System.IO;

namespace Logger
{
    public class Logger
    {
        static Logger()
        {
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            if (exeName.Contains("Server"))
                sLogFile = @"server.log";
            else
                sLogFile = @"client.log";
        }

        public static void Log(string pMsg)
        {
            using (StreamWriter w = File.AppendText(sLogFile))
            {
                w.WriteLine("{0:dd/MM/yyyy HH:mm:ss} - {1}",DateTime.Now, pMsg);
            }
        }

        static private string sLogFile;
    }
}

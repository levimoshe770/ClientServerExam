using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace Logger
{
    public class Logger
    {
        static Logger()
        {
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            if (exeName.Contains("Server"))
                sLogFile = @"audit.log";
            else
                sLogFile = @"client.log";
        }

        public static void Log(string pMsg)
        {
            WriteMsgToFile(pMsg);
        }

        public static void Log(string pFormat, params object[] pArgs)
        {
            WriteMsgToFile(string.Format(pFormat, pArgs));
        }

        static private void WriteMsgToFile(string pMsg)
        {
            try
            {
                StackTrace st = new StackTrace(2);
                using (StreamWriter w = File.AppendText(sLogFile))
                {
                    string msg = string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1} ({2}) - {3} - {4}\n",
                        DateTime.Now,
                        Path.GetFileName(st.GetFrame(0).GetFileName()),
                        st.GetFrame(0).GetFileLineNumber(),
                        st.GetFrame(0).GetMethod(),
                        pMsg);
                    w.WriteAsync(msg);
                }
            }
            catch(Exception e)
            {

            }
        }

        static private string sLogFile;
    }
}

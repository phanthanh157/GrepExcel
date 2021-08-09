﻿#define DEBUG
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;



namespace GrepExcel
{

    public static class F

    {
        // This method returns the callers filename and line number

        public static string FL([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)

        {
            // Remove path leaving only filename
            while (file.IndexOf("\\") >= 0)
                file = file.Substring(file.IndexOf("\\") + 1);
            return String.Format("{0} {1}:", file, line);
        }


        public static string FLM([CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            // Remove path leaving only filename
            while (file.IndexOf("\\") >= 0)
                file = file.Substring(file.IndexOf("\\") + 1);

            return String.Format("{0} {1} {2}:", file, line, member);
        }


        public static string FLMD([CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            DateTime time = DateTime.Now;
            // Remove path leaving only filename
            while (file.IndexOf("\\") >= 0)

                file = file.Substring(file.IndexOf("\\") + 1);
            return String.Format("{0} {1} {2} {3}:", time.ToString(), file, line, member);
        }
    }


    public static class ShowDebug
    {
        private const string fileLog = @"Log\DebugLog.txt";
#if DEBUG
        private const bool flag = true;
#endif

        private static int fisrtDebug = 0;
        // Log a formatted message. Filename and line number of location of call
        // to Msg method is automatically appended to start of formatted message.
        // Must be called with this syntax:
        // Log.Msg(F.L(), "Format using {0} {1} etc", ...);
        public static void Msg(string fileLine, string format, params object[] parms)
        {
#if DEBUG
            if (flag == true)
            {
                if (fisrtDebug == 0)
                {
                    Trace.WriteLine("PRINTER INFOMATION LOG -- START DEBUG LOG");
                    fisrtDebug++;
                }

                string message = String.Format(format, parms);

                Debug.WriteLine("{0} {1}", fileLine, message);

            }
#endif
        }


        public static void MsgErr(string fileLine, string format, params object[] parms)
        {
            if (flag == true)
            {

                string message = String.Format(format, parms);

                Debug.WriteLine("ERROR: {0} {1}", fileLine, message);

            }

        }

    }
}

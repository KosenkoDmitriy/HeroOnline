using LOLServices.Models;
using System;
using System.IO;

namespace LOLServices.Helpers
{
    public class LogService
    {
        public static void WriteLine(string format, params object[] args)
        {
            using (StreamWriter logWriter = new StreamWriter(Common.LogFileName, true))
            {
                logWriter.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss - "));
                logWriter.WriteLine(format, args);
            }
        }

        public static void WriteLine(Exception ex)
        {
            string logContent = string.Format("{0:yyyy-MM-dd HH:mm:ss}: {1}.\n", DateTime.Now, ex.Message);
            if (ex.InnerException != null)
                logContent += string.Format("Inner-Exception: {0}\n", ex.InnerException.Message);
            logContent += string.Format("{0}\n", ex.StackTrace);
            File.AppendAllText(Common.LogFileName, logContent);
        }
    }
}

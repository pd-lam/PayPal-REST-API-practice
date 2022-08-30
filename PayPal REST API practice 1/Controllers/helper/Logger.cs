using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayPal_REST_API_practice_1.Controllers.helper
{
    public class Logger
    {
        public static string LogDirectoryPath = Environment.CurrentDirectory;
        public static void Log(String lines)
        {
            //Ghi lại nhật ký
            try
            {
                System.IO.StreamWriter file = new
                System.IO.StreamWriter(LogDirectoryPath + "\\Error.log", true);
                file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " --> " + lines);
                file.Close();
            }
            catch
            { }
        }
    }
}
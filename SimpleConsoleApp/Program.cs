using Microsoft.Diagnostics.Instrumentation.Extensions.Intercept;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Decorator.IsHostEnabled())
            {
                Console.WriteLine("Please run this application using Startup.cmd script.");
                return;
            }

            Console.WriteLine("Agent version: " + Decorator.GetAgentVersion());

            Decorator.InitializeExtension();
            Functions.Decorate("System", "System.dll", "System.Net.Mail.SmtpClient.Send", OnBegin, OnEnd, OnException, false);

            // this wait is required in case you've already called "SmtpClient.Send" method before. Just give some time to requiest re-JIT
            Thread.Sleep(1000);

            Execute();

            Console.ReadLine();
        }

        public static void Execute()
        {
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Send(null);
            }
            catch (Exception)
            {
                //this is expected to throw
            }
        }

        public static object OnBegin(object thisObj, object arg1)
        {
            Console.WriteLine("Begin callback");
            Console.WriteLine("Callstack: " + new StackTrace().ToString());

            return null;
        }

        public static object OnEnd(object context, object returnValue, object thisObj, object arg1)
        {
            Console.WriteLine("End callback");
            Console.WriteLine("Callstack: " + new StackTrace().ToString());

            return returnValue;
        }

        public static void OnException(object context, object exception, object thisObj, object arg1)
        {
            Console.WriteLine("Exception callback");
            Console.WriteLine("Callstack: " + new StackTrace().ToString());
        }
    }
}

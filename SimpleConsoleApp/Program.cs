using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Diagnostics.Instrumentation.Extensions.Intercept;

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

            TelemetryConfiguration.Active.InstrumentationKey = "test";

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
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("host.myserver.abc", 443);
                client.Send(new MailMessage(
                    from: "sergkanz@microsoft.com", 
                    to: "chucknorris@microsoft.com", 
                    subject: "This is RTIA example", 
                    body: "Check out this example on github"));
            }
            catch (Exception)
            {
                //this is expected to throw
            }
        }

        public static object OnBegin(object thisObj, object arg1)
        {
            // diagnostics output
            Console.WriteLine("Begin callback");
            Console.WriteLine("Callstack: " + new StackTrace().ToString());

            // start the operation
            var operation = new TelemetryClient().StartOperation<DependencyTelemetry>("Send");
            operation.Telemetry.Type = "Smtp";
            operation.Telemetry.Target = ((SmtpClient)thisObj).Host;
            if (arg1 != null)
            {
                operation.Telemetry.Data = ((MailMessage)arg1).Subject;
            }

            // save the operation in the local context
            return operation;
        }

        public static object OnEnd(object context, object returnValue, object thisObj, object arg1)
        {
            // diagnostics output
            Console.WriteLine("End callback");
            Console.WriteLine("Callstack: " + new StackTrace().ToString());

            // stop the operation. Getting the operation from the context
            var operation = (IOperationHolder<DependencyTelemetry>)context;
            new TelemetryClient().StopOperation(operation);

            // you must return original return value unless you want to change it
            return returnValue;
        }

        public static void OnException(object context, object exception, object thisObj, object arg1)
        {
            // diagnostics output
            Console.WriteLine("Exception callback");
            Console.WriteLine("Callstack: " + new StackTrace().ToString());

            // mark operation as failed and stop it. Getting the operation from the context
            var operation = (IOperationHolder<DependencyTelemetry>)context;
            operation.Telemetry.Success = false;
            operation.Telemetry.ResultCode = exception.GetType().Name;
            new TelemetryClient().StopOperation(operation);

            // this is just to trace the result:
            Console.WriteLine("Telemetry item: " + Encoding.Default.GetString(
                JsonSerializer.Serialize(new List<ITelemetry>() { operation.Telemetry }, false)));
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTelemetry.Logs;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Examples.AspNetCore
{
 
    public class GenevaLogger
    {
        private ConcurrentDictionary<Type, ILogger> loggers = new ConcurrentDictionary<Type, ILogger>();

        public GenevaLogger()
        {
            GenevaEvent[] eventTypes = new GenevaEvent[]
            {
                new GatewayApiEvent(),
                new GatewayGetStatusEvent()

            };

            foreach (var eventType in eventTypes)
            {
                var loggerType = eventType.GetType();
                //var columns = new List<string> { "RequestId", "ClientId" };
                var columns = eventType.GetAllEventColumns();
                foreach (var col in columns)
                {
                    Debug.WriteLine("col: " + col);
                }
                //Console.WriteLine(eventType.name);
                var loggerFactory = LoggerFactory.Create(builder => builder
                .AddOpenTelemetry(options =>
                {
                    options.AddConsoleExporter();
                    options.AddGenevaLogExporter(options =>
                    {
                        options.ConnectionString = "EtwSession=OpenTelemetry";
                        options.CustomFields = columns;
                        options.PrepopulatedFields = new Dictionary<string, object>
                        {
                            ["cloud.role"] = "onebox",
                            ["cloud.roleInstance"] = "geneva-otel"
                        };
                        options.TableNameMappings = new Dictionary<string, string>
                        {
                            [loggerType.FullName] = loggerType.Name,

                        };
                    });

                }));
                var logger = loggerFactory.CreateLogger(loggerType.FullName);
                //Debug.WriteLine(logger.GetType());
                this.loggers.TryAdd(loggerType, logger); 
            }
            

        }

        public void logEvent(Type type, params object[] args)
        {
            string loggerstring = getLoggerString(type);
            var logger = this.loggers[type];
            logger.LogInformation(loggerstring, args);
        }

        private string getLoggerString(Type type)
        {
            string loggerstring = "";

            foreach (var prop in type.GetProperties())
            {
                loggerstring = loggerstring + "{" + prop.Name + "}";
            }
            return loggerstring;
        }
    }
}

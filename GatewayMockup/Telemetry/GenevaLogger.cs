using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace Telemetry
{

    public class GenevaLogger
    {
        private ConcurrentDictionary<Type, ILogger> loggers = new ConcurrentDictionary<Type, ILogger>();

        private List<string> GetAllEventColumns(Type type)
        {
            List<string> columns = new List<string>();
            var members = type.GetProperties();

            foreach (var member in members)
            {
                columns.Add(member.Name);
            }

            return columns;
        }

        public GenevaLogger()
        {
            Type[] eventTypes = new Type[]
            {
                typeof(GatewaySetStatusEvent),
                typeof(GatewayUpdateStatusEvent),
                typeof(GatewayGetStatusEvent)

            };

            foreach (var loggerType in eventTypes)
            {
                //var loggerType = eventType.GetType();
                //var columns = new List<string> { "RequestId", "ClientId" };
                var columns = GetAllEventColumns(loggerType);
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
                this.loggers.TryAdd(loggerType, logger);

                //switch (eventType)
                //{
                //    case GatewayGetStatusEvent ls: this.loggers.TryAdd(loggerType, loggerFactory.CreateLogger<GatewayGetStatusEvent>()); break;
                //    case GatewaySetStatusEvent ls: this.loggers.TryAdd(loggerType, loggerFactory.CreateLogger<GatewaySetStatusEvent>()); break;
                //    case GatewayUpdateStatusEvent ls: this.loggers.TryAdd(loggerType, loggerFactory.CreateLogger<GatewayUpdateStatusEvent>()); break;
                //    default:
                //        throw new ArgumentException();
                //}
            }


        }

        public void logEvent(Type type, params object[] args)
        {
            string loggerstring = getLoggerString(type);
            var logger = this.loggers[type];
            logger.LogInformation(loggerstring, args);
        }

        public void logEvent(GenevaEvent genevaEvent)
        {
            var type = genevaEvent.GetType();
            string loggerstring = getLoggerString(type);
            var logger = this.loggers[type];
            List<object> properties = new List<object>();
            foreach (var prop in type.GetProperties())
            {
                properties.Add(prop.GetValue(genevaEvent));
            }
            logger.LogInformation(loggerstring, properties.ToArray());
        }

        //public void LogEvent(GenevaEvent ev) {
        //    var type = ev.GetType();
        //    var logger = this.loggers[type];
        //    logger.LogInformation("{ev}", ev);

        //}
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
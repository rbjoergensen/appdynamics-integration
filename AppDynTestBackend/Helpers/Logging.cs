using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace AppDynTestBackend.Helpers
{
    public class Logging
    {
        public static LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch();
        public static Logger log = new LoggerConfiguration().WriteTo.Console().MinimumLevel.ControlledBy(levelSwitch).CreateLogger();
        public static void SetLogLevel(string logLevel)
        {
            levelSwitch.MinimumLevel = logLevel switch
            {
                "Verbose" => LogEventLevel.Verbose,
                "Debug" => LogEventLevel.Debug,
                "Information" => LogEventLevel.Information,
                "Warning" => LogEventLevel.Warning,
                "Error" => LogEventLevel.Error,
                "Fatal" => LogEventLevel.Fatal,
                _ => LogEventLevel.Information,
            };
        }
    }
}

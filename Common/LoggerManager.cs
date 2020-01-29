using Serilog;

namespace Common
{
    public class LoggerManager : ILoggerManager
    {
        public ILogger GetLogger(object owner)
        {
            //const string customTemplate = "Message {Timestamp:yyyy-MMM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";
            //return new LoggerConfiguration().WriteTo.File("serilog.txt", outputTemplate: customTemplate).CreateLogger().ForContext(owner.GetType());
            return new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger()
                .ForContext(owner.GetType());
        }
    }
}
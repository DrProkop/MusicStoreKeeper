using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Common
{
    /// <summary>
    /// Base class for classes with logging functional.
    /// </summary>
    public abstract class LoggingBase
    {
        protected LoggingBase(ILoggerManager manager)
        {
            log = manager.GetLogger(this);
        }

        protected readonly ILogger log;

    }
}

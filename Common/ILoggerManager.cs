using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Common
{
    public interface ILoggerManager
    {
        ILogger GetLogger(object owner);
    }
}

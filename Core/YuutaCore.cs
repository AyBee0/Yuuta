using Serilog;
using System;
using System.IO;

namespace Core
{
    public static class YuutaCore
    {
        public static void SetupLogger()
        {
            Serilog.Core.Logger log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine("logs", DateTime.Now.ToString("MM-dd-yyyy H m") + ".txt"))
                .CreateLogger();
            Log.Logger = log;
        }
    }
}

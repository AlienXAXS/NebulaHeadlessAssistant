using BepInEx.Logging;

namespace NebulaHeadlessAssistant
{
    public static class Log
    {
        private static ILogger _logger;

        public static void Init(ILogger logger)
        {
            _logger = logger;
        }

        public static void LogDebug(object data)
        {
            _logger.LogDebug(data);
        }

        public static void LogInfo(object data)
        {
            _logger.LogInfo(data);
        }

        public static void LogWarning(object data)
        {
            _logger.LogWarning(data);
        }

        public static void LogError(object data)
        {
            _logger.LogError(data);
        }
    }

    public class BepInExLogger : ILogger
    {
        private readonly ManualLogSource logger;

        public BepInExLogger(ManualLogSource logger)
        {
            this.logger = logger;
        }

        public void LogDebug(object data)
        {
            logger.LogDebug(data);
        }

        public void LogError(object data)
        {
            logger.LogError(data);
        }

        public void LogInfo(object data)
        {
            logger.LogInfo(data);
        }

        public void LogWarning(object data)
        {
            logger.LogWarning(data);
        }
    }
}

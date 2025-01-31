using AltTester.AltDriver.Logging;

namespace AltTester.AltDriver.Notifications
{
    public class AltLogNotificationResultParams
    {
        public string message;
        public string stackTrace;
        public AltLogLevel level;

        public AltLogNotificationResultParams(string message, string stackTrace, AltLogLevel level)
        {
            this.message = message;
            this.stackTrace = stackTrace;
            this.level = level;
        }
    }
}
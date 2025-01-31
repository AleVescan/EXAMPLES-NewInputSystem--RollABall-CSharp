using System.Reflection;
using AltTester.AltDriver.Logging;
using AltTester.AltDriver.Notifications;
using AltTester.Communication;
using UnityEngine;

namespace AltTester.Notification
{
    public class AltLogNotification : BaseNotification
    {
        public AltLogNotification(ICommandHandler commandHandler, bool isOn) : base(commandHandler)
        {
            Application.logMessageReceived -= onLogReceived;

            if (isOn)
            {
                Application.logMessageReceived += onLogReceived;
            }

        }

        static void onLogReceived(string message, string stackTrace, LogType type)
        {
            var data = new AltLogNotificationResultParams(message, stackTrace, logTypeToLogLevel(type));
            SendNotification(data, "logNotification");
        }

        private static AltLogLevel logTypeToLogLevel(LogType type)
        {
            if ((int)type == (int)LogType.Error || (int)type == (int)LogType.Exception)
                return AltLogLevel.Error;
            else if ((int)type == (int)LogType.Assert || (int)type == (int)LogType.Log)
                return AltLogLevel.Debug;
            return AltLogLevel.Warn;
        }
    }
}
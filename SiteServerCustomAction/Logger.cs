using Acr.Delivery.Commons.Logging;
using Microsoft.Deployment.WindowsInstaller;

namespace Acr.SiteServer.InstallUtil
{
	internal sealed class Logger : ILogger
	{
        private readonly Session _session; 

        public Logger(Session session)
        {
            _session = session;
        }

		public void Log(LogLevel level, string message)
		{
			Log(new LogMessage(level, message));
		}

		public void Log(LogMessage message)
		{
            _session.Log($"[{message.Timestamp:yyyy-MM-dd HH:mm:ss}] [{message.Level}] - {message.Message}");
		}
	}
}

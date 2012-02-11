using System.Diagnostics;
using System.ServiceProcess;
using ServiceWrapper.Properties;
using log4net;

namespace ServiceWrapper
{
	public sealed class ServiceWrapper : ServiceBase
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (ServiceWrapper));
		public static IServiceInstallHelper InstallHelper = new ServiceInstallHelper(false);

		public ServiceWrapper()
		{
			ServiceName = InstallHelper.ServiceName;
			EventLog.Log = Settings.Default.EventLog;

			CanStop = true;
			CanHandlePowerEvent = true;
			CanPauseAndContinue = true;
			CanShutdown = true;
			CanHandleSessionChangeEvent = true;
		}

		protected override void Dispose(bool disposing)
		{
			Log.Debug("Service is being disposed.");
			base.Dispose(disposing);
		}

		protected override void OnStart(string[] args)
		{
			if (Settings.Default.Debug) Debugger.Launch();
			base.OnStart(args);
			Log.Info("Service started.");
		}

		protected override void OnStop()
		{
			base.OnStop();
			Log.Info("Service stopped.");
		}

		protected override void OnPause()
		{
			base.OnPause();
			Log.Info("Service paused.");
		}

		protected override void OnContinue()
		{
			base.OnContinue();
			Log.Info("Service resumed.");
		}

		protected override void OnShutdown()
		{
			base.OnShutdown();
			Log.Info("System shutdown event received.");
		}

		protected override void OnCustomCommand(int command)
		{
			base.OnCustomCommand(command);
			Log.InfoFormat("Custom command received. Identifier: {0}", command);
		}

		protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
		{
			Log.Info("Power status updated.");
			Log.DebugFormat("Status: {0}", powerStatus);

			return base.OnPowerEvent(powerStatus);
		}

		protected override void OnSessionChange(SessionChangeDescription changeDescription)
		{
			base.OnSessionChange(changeDescription);
			
			Log.Info("Session changed.");
			Log.DebugFormat("Session Id: {0}, Reason: {1}", 
				changeDescription.SessionId, changeDescription.Reason);
		}
	}
}
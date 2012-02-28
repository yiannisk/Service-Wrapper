using System.Diagnostics;
using System.ServiceProcess;
using ServiceWrapper.Properties;
using log4net;

namespace ServiceWrapper
{
	public sealed class ServiceWrapper : ServiceBase
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (ServiceWrapper));
		private ServiceProviderLoader ProviderLoader { get; set; }

		public ServiceWrapper()
		{
			ConfigureHandler();
			LoadServiceProvider();
		}

		#region Initialization
		private void ConfigureHandler()
		{
			ServiceName = Settings.Default.ServiceName;
			EventLog.Log = Settings.Default.EventLog;

			CanStop = true;
			CanHandlePowerEvent = true;
			CanPauseAndContinue = true;
			CanShutdown = true;
			CanHandleSessionChangeEvent = true;
		}

		private void LoadServiceProvider()
		{
			if (ProviderLoader == null) ProviderLoader = new ServiceProviderLoader();
			ProviderLoader.Load(Settings.Default.ServiceProviderName, Settings.Default.ServiceProviderPath);
		}
		#endregion

		#region IDisposable implementation
		protected override void Dispose(bool disposing)
		{
			Log.Debug("Service is being disposed.");
			base.Dispose(disposing);
		}
		#endregion

		#region Service event overrides
		protected override void OnStart(string[] args)
		{
			if (Settings.Default.Debug) Debugger.Launch();
			base.OnStart(args);
			Log.Info("Service started.");
			ProviderLoader.Provider.Start(args);
		}

		protected override void OnStop()
		{
			base.OnStop();
			Log.Info("Service stopped.");
			ProviderLoader.Provider.Stop();
		}

		protected override void OnPause()
		{
			base.OnPause();
			Log.Info("Service paused.");
			ProviderLoader.Provider.Pause();
		}

		protected override void OnContinue()
		{
			base.OnContinue();
			Log.Info("Service resumed.");
			ProviderLoader.Provider.Continue();
		}

		protected override void OnShutdown()
		{
			base.OnShutdown();
			Log.Info("System shutdown event received.");
			ProviderLoader.Provider.Shutdown();
		}

		protected override void OnCustomCommand(int command)
		{
			base.OnCustomCommand(command);
			Log.InfoFormat("Custom command received. Identifier: {0}", command);
			ProviderLoader.Provider.CustomCommand(command);
		}

		protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
		{
			Log.Info("Power status updated.");
			Log.DebugFormat("Status: {0}", powerStatus);
			return base.OnPowerEvent(powerStatus) && ProviderLoader.Provider.PowerEvent(powerStatus);
		}

		protected override void OnSessionChange(SessionChangeDescription changeDescription)
		{
			base.OnSessionChange(changeDescription);
			
			Log.Info("Session changed.");
			Log.DebugFormat("Session Id: {0}, Reason: {1}", 
				changeDescription.SessionId, changeDescription.Reason);

			ProviderLoader.Provider.SessionChange(changeDescription);
		}
		#endregion
	}
}
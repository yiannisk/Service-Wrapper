using System;
using System.Linq;
using System.ServiceProcess;
using log4net;

namespace ServiceWrapper
{
	class Program
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
		protected static IServiceInstallHelper InstallHelper = new ServiceInstallHelper(false);

		static void Main(string[] args)
		{
			if (args.Contains("-i") || args.Contains("--install"))
			{
				Log.InfoFormat("Attempting to install service {0}.", InstallHelper.ServiceName);
				InstallHelper.InstallService();
				return;
			}
			
			if (args.Contains("-u") || args.Contains("--uninstall"))
			{
				Log.InfoFormat("Attempting to uninstall service {0}.", InstallHelper.ServiceName);
				InstallHelper.UninstallService();
				return;
			}

			try { ServiceBase.Run(new ServiceWrapper()); }
			catch (Exception exception)
			{
				Log.Error("Service could not start.", exception);
			}
			
		}
	}
}
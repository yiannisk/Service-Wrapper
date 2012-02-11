using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using ServiceWrapper.Properties;

namespace ServiceWrapper
{
	[RunInstaller(true)]
	public class ServiceWrapperInstaller : Installer
	{
		protected static IServiceInstallHelper InstallHelper = new ServiceInstallHelper(false);

		public ServiceWrapperInstaller()
		{
			var processInstaller = new ServiceProcessInstaller
			{
				Account = (ServiceAccount) 
					Enum.Parse(typeof (ServiceAccount), Settings.Default.ServiceAccount),
				Username = Settings.Default.AccountUsername == string.Empty
					? null
					: Settings.Default.AccountUsername,
				Password = Settings.Default.AccountPassword == string.Empty
					? null
					: Settings.Default.AccountPassword
			};

			var installer = new ServiceInstaller
			{
				DisplayName = InstallHelper.ServiceName,
				StartType = (ServiceStartMode)
					Enum.Parse(typeof(ServiceStartMode), Settings.Default.ServiceStartMode),
				ServiceName = InstallHelper.ServiceName,
				Description = Settings.Default.ServiceDescription
			};

			Installers.Add(processInstaller);
			Installers.Add(installer);
		}
	}
}
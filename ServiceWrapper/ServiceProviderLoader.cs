using System;
using System.IO;
using log4net;

namespace ServiceWrapper
{
	public class ServiceProviderLoader
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ServiceProviderLoader));

		public virtual AppDomain Domain { get; protected set; }
		public virtual ServiceProvider Provider { get; protected set; }

		public void Load(string name, string path)
		{
			if (Provider != null && !Release())
			{
				const string message = "Could not release application domain. Service is stopping and needs a restart.";
				Log.Fatal(message);
				throw new Exception(message);
			}

			var pathToAssembly = ResolvePath(path);

			if (String.IsNullOrEmpty(pathToAssembly))
				throw new FileNotFoundException("Could not locate ServiceProvider assembly in given path.", path);

			var domainSetup = new AppDomainSetup { PrivateBinPath = pathToAssembly };
			Domain = AppDomain.CreateDomain("Domain_" + Guid.NewGuid(), null, domainSetup);
			Provider = (ServiceProvider) Domain.CreateInstanceFromAndUnwrap(pathToAssembly, name);

			Log.Info("Initialized application domain.");
			Log.InfoFormat("Domain Name: {0}", Domain.FriendlyName);
		}

		public bool Release(int maxRetries = 3)
		{
			if (Domain == null) return true;

			var retries = 0;
			while (retries++ <= maxRetries)
			{
				try
				{
					Log.Info("Attempting to release application domain.");

					if (Domain != null)
					{
						Log.InfoFormat("Domain Name: {0}", Domain.FriendlyName);
						AppDomain.Unload(Domain);
					}

					Provider = null;
					Domain = null;

					Log.Info("Succeeded.");

					return true;
				}
				catch (AppDomainUnloadedException appDomainUnloadedException)
				{
					Log.InfoFormat("Could not release domain. Retrying {0} out of {1}...", retries, maxRetries);
					Log.Info("Exception: ", appDomainUnloadedException);
				}
			}

			return false;
		}

		internal static string ResolvePath(string path)
		{
			if (File.Exists(path)) return path;
			var supposedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
			return File.Exists(supposedPath) ? supposedPath : string.Empty;
		}
	}
}
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Utilities;
using ServiceWrapper.Properties;

namespace ServiceWrapper
{
	internal class ServiceInstallHelper : IServiceInstallHelper
	{
		public virtual string ServiceName
		{
			get { return Settings.Default.ServiceName; }
		}

		public virtual string ServicePath
		{
			get
			{
				var process = Process.GetCurrentProcess();
				return Path.GetFileName(process.MainModule.FileName);
			}
		}

		private bool Verbose { get; set; }

		internal ServiceInstallHelper(bool verbose)
		{
			Verbose = verbose;
		}

		public virtual void InstallService()
		{
			ProcessStartInfo processStartInfo;

			try { processStartInfo = GetProcessStartInfoTemplate(ServicePath); }
			catch (FileNotFoundException exception)
				{ throw new Exception("Could not install service", exception); }

			var process = Process.Start(processStartInfo);
			process.WaitForExit();

			if (!Verbose) return;

			File.WriteAllText("servinst.log", 
				"INSTALLATION OUTPUT: \n" 
					+ process.StandardOutput.ReadToEnd()
					+ "\nINSTALLATION ERRORS: \n"
					+ process.StandardError.ReadToEnd());

			Console.WriteLine("Installation complete. Placed installation log in [servinst.log].");
		}

		public virtual void UninstallService()
		{
			ProcessStartInfo processStartInfo;

			try { processStartInfo = GetProcessStartInfoTemplate("/u " + ServicePath); }
			catch (FileNotFoundException exception)
				{ throw new Exception("Could not uninstall service", exception); }

			var process = Process.Start(processStartInfo);
			process.WaitForExit();

			if (!Verbose) return;

			File.WriteAllText("servuninst.log",
				"UNINSTALLATION OUTPUT: \n"
					+ process.StandardOutput.ReadToEnd()
					+ "\nUNINSTALLATION ERRORS: \n"
					+ process.StandardError.ReadToEnd());

			Console.WriteLine("Uninstallation complete. Placed uninstallation log in [servuninst.log].");
		}

		public virtual ProcessStartInfo GetProcessStartInfoTemplate(string args)
		{
			var dir = ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version40);
			if (dir == null)
				throw new FileNotFoundException("Could not obtain the Microsoft.NET installation directory.");

			return new ProcessStartInfo
			{
				Arguments = args,
				FileName = dir + "\\InstallUtil.exe",
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				UseShellExecute = false
			};
		}
	}
}

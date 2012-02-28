using System.Diagnostics;
using System.IO;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using ServiceWrapper;

namespace ServiceWrapperTests
{
	[TestFixture]
	public class ServiceInstallHelperTests
	{
		[Test]
		public void TestServicePath()
		{
			var installHelper = new ServiceInstallHelper(true);
			var process = Process.GetCurrentProcess();
			var expected = Path.GetFileName(process.MainModule.FileName);
			Assert.AreEqual(expected, installHelper.ServicePath);
		}

		[Test]
		public void TestGetProcessStartInfoTemplate()
		{
			var installHelper = new ServiceInstallHelper(true);
			var startInfo = installHelper.GetProcessStartInfoTemplate("args");
			Assert.AreEqual(startInfo.Arguments, "args");
			Assert.AreEqual(startInfo.FileName, 
				ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version40) + "\\InstallUtil.exe");

			Assert.IsTrue(startInfo.RedirectStandardError);
			Assert.IsTrue(startInfo.RedirectStandardOutput);
			Assert.IsTrue(startInfo.CreateNoWindow);
			Assert.IsFalse(startInfo.UseShellExecute);
		}
	}
}

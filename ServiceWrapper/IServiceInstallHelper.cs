namespace ServiceWrapper
{
	public interface IServiceInstallHelper
	{
		string ServiceName { get; }
		string ServicePath { get; }
		void InstallService();
		void UninstallService();
	}
}
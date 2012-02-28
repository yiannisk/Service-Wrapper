using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServiceWrapper;

namespace ServiceWrapperTests.ClassFixtures
{
	public class DummyServiceProvider : ServiceProvider
	{
		public override void Start(string[] args) {}
		public override void Stop() {}
		
		public void ThreadSleep()
		{
			Thread.Sleep(int.MaxValue);
		}

		public void SocketBlock()
		{
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			var endpoint = new IPEndPoint(IPAddress.Loopback, 50);
			socket.Bind(endpoint);
			socket.Blocking = true;
			socket.Connect(endpoint);
			var buffer = new byte[1024];
			socket.Receive(buffer); // blocking call.
		}
	}
}

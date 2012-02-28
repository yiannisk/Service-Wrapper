using System;
using System.ServiceProcess;

namespace ServiceWrapper
{
	public abstract class ServiceProvider : MarshalByRefObject
	{
		public abstract void Start(string[] args);
		public abstract void Stop();

		public virtual void Pause() {}
		public virtual void Continue() {}
		public virtual void Shutdown() {}
		public virtual void CustomCommand(int command) {}
		public virtual bool PowerEvent(PowerBroadcastStatus status) { return true; }
		public virtual void SessionChange(SessionChangeDescription description)  {}

		// The (pointless?) override below is to insure that 
		// when created as a Singleton, the instance never dies, no
		// matter how long between calls.
		public override object InitializeLifetimeService() { return null; }
	}
}
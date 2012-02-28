using System;
using NUnit.Framework;
using ServiceWrapper;
using ServiceWrapperTests.ClassFixtures;

namespace ServiceWrapperTests
{
	[TestFixture]
	public class ServiceProviderLoaderTests
	{
		[Test]
		public void TestCanResolvePathToThisAssembly()
		{
			Assert.IsNotEmpty(ServiceProviderLoader.ResolvePath("ServiceWrapperTests.dll"));
		}

		[Test]
		public void TestCantResolveNonexistentAssemly()
		{
			Assert.IsEmpty(ServiceProviderLoader.ResolvePath("Blah.dll"));
		}

		[Test]
		public void TestLoadUnloadServiceProvider()
		{
			var providerLoader = new ServiceProviderLoader();
			providerLoader.Load("ServiceWrapperTests.ClassFixtures.DummyServiceProvider", "ServiceWrapperTests.dll");
			Assert.IsNotNull(providerLoader.Domain);
			Assert.IsNotNull(providerLoader.Provider);
			Assert.IsTrue(providerLoader.Release());
		}

		[Test]
		public void TestReleaseAppDomainWhileThreadIsSleeping()
		{
			var providerLoader = new ServiceProviderLoader();
			providerLoader.Load("ServiceWrapperTests.ClassFixtures.DummyServiceProvider", "ServiceWrapperTests.dll");
			var dummyProvider = (DummyServiceProvider) providerLoader.Provider;
			Action a = dummyProvider.ThreadSleep;
			a.BeginInvoke(null, null);
			Assert.IsTrue(providerLoader.Release());
		}

		[Test]
		public void TestReleaseAppDomainWhileSocketIsBlocked()
		{
			var providerLoader = new ServiceProviderLoader();
			providerLoader.Load("ServiceWrapperTests.ClassFixtures.DummyServiceProvider", "ServiceWrapperTests.dll");
			var dummyProvider = (DummyServiceProvider)providerLoader.Provider;
			Action a = dummyProvider.SocketBlock;
			a.BeginInvoke(null, null);
			Assert.IsTrue(providerLoader.Release());
		}

		[Test]
		public void TestSocketReceiveReleaseAndRebindInDifferentAppDomain()
		{
			var providerLoader = new ServiceProviderLoader();
			providerLoader.Load("ServiceWrapperTests.ClassFixtures.DummyServiceProvider", "ServiceWrapperTests.dll");
			var dummyProvider = (DummyServiceProvider)providerLoader.Provider;
			Action a = dummyProvider.SocketBlock;
			a.BeginInvoke(null, null);
			Assert.IsTrue(providerLoader.Release());

			providerLoader.Load("ServiceWrapperTests.ClassFixtures.DummyServiceProvider", "ServiceWrapperTests.dll");
			dummyProvider = (DummyServiceProvider)providerLoader.Provider;
			a = dummyProvider.SocketBlock;
			a.BeginInvoke(null, null);
			Assert.IsTrue(providerLoader.Release());
		}

		[Test]
		public void TestEachLoadCreatesNewAppDomain()
		{
			var providerLoader = new ServiceProviderLoader();
			providerLoader.Load("ServiceWrapperTests.ClassFixtures.DummyServiceProvider", "ServiceWrapperTests.dll");
			var firstDomain = providerLoader.Domain;
			providerLoader.Load("ServiceWrapperTests.ClassFixtures.DummyServiceProvider", "ServiceWrapperTests.dll");
			var secondDomain = providerLoader.Domain;
			Assert.AreNotSame(firstDomain, secondDomain);

			// any method of the application domain would work for the delegate below. What we 're after is 
			// the exception that tells us the domain is unloaded.
			Assert.Throws<AppDomainUnloadedException>(() => firstDomain.IsFinalizingForUnload());
		}
	}
}
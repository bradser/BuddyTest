using System;
using System.Linq;
using Buddy;
using BuddyTest;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHarness
{
    [TestClass]
    public class LocationTrackerTests : SilverlightTest
    {
        [TestMethod]
        public void Constructor()
        {
            var locationTracker = LocationTracker.GetInstance(positionChangedArgs => { });

            Assert.IsNotNull(locationTracker);
        }

        [TestMethod]
        [Asynchronous]
        public void TestTrackerByWalkingAround()
        {
            var locationTracker = LocationTracker.GetInstance(positionChangedArgs =>
            {
                Assert.IsNotNull(positionChangedArgs.Position);

                EnqueueTestComplete();
            });
        }
    }
}
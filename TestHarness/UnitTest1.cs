using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuddyTest;
using Buddy;
using System.Linq;

namespace TestHarness
{
    [TestClass]
    public class SampleTests : SilverlightTest
    {
        [TestMethod]
        public void EnumHelperOnlyWorksOnEnums()
        {
            new EnumHelper<BuddyClient>();
        }

        [TestMethod]
        public void VerifyAlphabeticalNames()
        {
            var enumHelper = new EnumHelper<UserStatus>();

            var firstName = enumHelper.AlphabeticalNames.First();

            Assert.AreEqual(firstName, "Any");
        }

        [TestMethod]
        public void VerifyEnumHelperRoundTripping()
        {
            var enumHelper = new EnumHelper<UserStatus>();

            var roundTripped = enumHelper.Names.Contains(Enum.GetName(typeof(UserStatus), UserStatus.Married));

            Assert.IsTrue(roundTripped);
        }


        [TestMethod]
        public void VerifyNamesAreAlphabetialNames()
        {
            var enumHelper = new EnumHelper<UserStatus>();

            var countIsSame = enumHelper.Names.Count() == enumHelper.Names.Intersect(enumHelper.AlphabeticalNames).Count();

            Assert.IsTrue(countIsSame);
        }
    }
}

using System;
using System.Linq;
using Buddy;
using BuddyTest;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHarness
{
    [TestClass]
    public class SampleTests : SilverlightTest
    {
        [TestMethod]
        public void EnumHelperOnlyWorksOnEnums()
        {
            var emptyEnumHelper = EnumHelper<BuddyClient>.GetInstance();

            Assert.IsNull(emptyEnumHelper);
        }

        [TestMethod]
        public void VerifyAlphabeticalNames()
        {
            var enumHelper = EnumHelper<UserStatus>.GetInstance();

            var firstName = enumHelper.AlphabeticalNames.First();

            Assert.AreEqual(firstName, "Any");
        }

        [TestMethod]
        public void VerifyEnumHelperRoundTripping()
        {
            var enumHelper = EnumHelper<UserStatus>.GetInstance();

            var roundTripped = enumHelper.Names.Contains(Enum.GetName(typeof(UserStatus), UserStatus.Married));

            Assert.IsTrue(roundTripped);
        }

        [TestMethod]
        public void VerifyNamesAreAlphabetialNames()
        {
            var enumHelper = EnumHelper<UserStatus>.GetInstance();

            var countIsSame = enumHelper.Names.Count() == enumHelper.Names.Intersect(enumHelper.AlphabeticalNames).Count();

            Assert.IsTrue(countIsSame);
        }
    }
}

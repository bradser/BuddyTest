using System;
using System.Linq;
using Buddy;
using BuddyTest;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHarness
{
    [TestClass]
    public class EnumHelperTests : SilverlightTest
    {
        [TestMethod]
        public void OnlyWorksOnEnums()
        {
            var emptyEnumHelper = EnumHelper<BuddyClient>.GetInstance(); // use any random non-enum type

            Assert.IsNull(emptyEnumHelper);
        }

        [TestMethod]
        public void VerifyAlphabeticalNames()
        {
            var enumHelper = EnumHelper<UserStatus>.GetInstance();

            var firstName = enumHelper.AlphabeticalNames.First(); // TODO: use linq to verify each name

            Assert.AreEqual(firstName, "Any");
        }

        [TestMethod]
        public void VerifyRoundTripping()
        {
            var enumHelper = EnumHelper<UserStatus>.GetInstance();

            var roundTripped = DoRoundTrip(enumHelper, UserStatus.Any);
            Assert.IsTrue(roundTripped);

            roundTripped = DoRoundTrip(enumHelper, UserStatus.Widowed);
            Assert.IsTrue(roundTripped);
        }

        private bool DoRoundTrip(EnumHelper<UserStatus> enumHelper, UserStatus status)
        {
            return enumHelper.Names.Contains(Enum.GetName(typeof(UserStatus), status));
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

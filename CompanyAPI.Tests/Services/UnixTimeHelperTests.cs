using System;
using CompanyAPI.Services;
using Xunit;

namespace CompanyAPI.Tests.Services
{
    public class UnixTimeHelperTests
    {
        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly DateTime dtTest = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const long utTest = 1577836800;

        [Fact]
        private void TestInverse()
        {
            long unixNow = UnixTimeHelper.CurrentUnixTime();
            DateTime dtNow = UnixTimeHelper.FromUnixTime(unixNow);
            Assert.Equal(unixNow, UnixTimeHelper.ToUnixTime(dtNow));
        }

        [Fact]
        private void TestEpoch()
        {
            Assert.Equal(0, UnixTimeHelper.ToUnixTime(epoch));
        }

        [Fact]
        private void TestToUnixTime()
        {
            Assert.Equal(utTest, UnixTimeHelper.ToUnixTime(dtTest));
        }

        [Fact]
        private void TestFromUnixTime()
        {
            Assert.Equal(dtTest, UnixTimeHelper.FromUnixTime(utTest));
        }
    }
}
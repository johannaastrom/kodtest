namespace TollFeeCalculator.Tests
{
    public class TollCalculatorTest
    {
        private readonly TollCalculator _sut;

        public TollCalculatorTest()
        {
            _sut = new TollCalculator();
        }

        [Fact]
        public void GetDailyTollFee_NullVehicle_ReturnsZero()
        {
            var dates = new[] { new DateTime(2013, 1, 2, 8, 0, 0) };

            var result = TollCalculator.GetDailyTollFee(null, dates);

            Assert.Equal(0, result);
        }

        [Fact]
        public void GetDailyTollFee_NullDates_ReturnsZero()
        {
            var vehicle = new Car();

            var result = TollCalculator.GetDailyTollFee(vehicle, null);

            Assert.Equal(0, result);
        }

        [Fact]
        public void GetDailyTollFee_TollFreeVehicle_ReturnsZero()
        {
            var dates = new[] { new DateTime(2013, 1, 2, 7, 0, 0) };

            var result = TollCalculator.GetDailyTollFee(new Motorbike(), dates);

            Assert.Equal(0, result);
        }

        [Fact]
        public void GetDailyTollFee_SinglePass_ReturnsCorrectFee()
        {
            var dates = new[] { new DateTime(2013, 1, 2, 7, 15, 0) };

            var result = TollCalculator.GetDailyTollFee(new Car(), dates);

            Assert.Equal(18, result);
        }

        [Fact]
        public void GetDailyTollFee_ExceedsMaxFee_ReturnsMaxFee()
        {
            var dates = new[]
            {
                new DateTime(2013, 1, 2, 6, 0, 0),
                new DateTime(2013, 1, 2, 8, 0, 0),
                new DateTime(2013, 1, 2, 10, 0, 0),
                new DateTime(2013, 1, 2, 12, 0, 0),
                new DateTime(2013, 1, 2, 14, 0, 0),
                new DateTime(2013, 1, 2, 16, 0, 0),
                new DateTime(2013, 1, 2, 18, 0, 0)
            };

            var result = TollCalculator.GetDailyTollFee(new Car(), dates);

            Assert.Equal(60, result);
        }

        [Fact]
        public void GetDailyTollFee_MultiplePassesWithin60Minutes_ChargesHighestFee()
        {
            var dates = new[]
            {
                new DateTime(2013, 1, 2, 7, 0, 0),
                new DateTime(2013, 1, 2, 7, 30, 0)
            };

            var result = TollCalculator.GetDailyTollFee(new Car(), dates);

            Assert.Equal(18, result);
        }

        [Fact]
        public void GetDailyTollFee_PassesOver60MinutesApart_ChargesBothFees()
        {
            var dates = new[]
            {
                new DateTime(2013, 1, 2, 7, 0, 0),
                new DateTime(2013, 1, 2, 8, 5, 0)
            };

            var result = TollCalculator.GetDailyTollFee(new Car(), dates);

            Assert.Equal(31, result);
        }

        [Theory]
        [InlineData(6, 0, 8)]
        [InlineData(6, 29, 8)]
        [InlineData(6, 30, 13)]
        [InlineData(6, 59, 13)]
        [InlineData(7, 0, 18)]
        [InlineData(7, 59, 18)]
        [InlineData(8, 0, 13)]
        [InlineData(8, 29, 13)]
        [InlineData(8, 30, 8)]
        [InlineData(14, 59, 8)]
        [InlineData(15, 0, 13)]
        [InlineData(15, 29, 13)]
        [InlineData(15, 30, 18)]
        [InlineData(16, 0, 18)]
        [InlineData(16, 59, 18)]
        [InlineData(17, 0, 13)]
        [InlineData(17, 59, 13)]
        [InlineData(18, 0, 8)]
        [InlineData(18, 29, 8)]
        [InlineData(18, 30, 0)]
        [InlineData(5, 59, 0)]
        [InlineData(19, 0, 0)]
        public void GetDailyTollFee_VariousTimeSlots_ReturnsCorrectFee(int hour, int minute, int expectedFee)
        {
            var dates = new[] { new DateTime(2013, 1, 2, hour, minute, 0) };

            var result = TollCalculator.GetDailyTollFee(new Car(), dates);

            Assert.Equal(expectedFee, result);
        }

        [Theory]
        [InlineData(2013, 1, 5)]
        [InlineData(2013, 1, 6)]
        [InlineData(2013, 1, 1)]
        [InlineData(2013, 7, 15)]
        [InlineData(2013, 12, 24)]
        [InlineData(2013, 12, 25)]
        [InlineData(2013, 12, 31)]
        public void GetDailyTollFee_TollFreeDate_ReturnsZero(int year, int month, int day)
        {
            var dates = new[] { new DateTime(year, month, day, 7, 0, 0) };

            var result = TollCalculator.GetDailyTollFee(new Car(), dates);

            Assert.Equal(0, result);
        }
    }
}
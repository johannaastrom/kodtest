namespace TollFeeCalculator
{
    public class TollCalculator
    {

        /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total toll fee for that day
         */

        private const int MaxFee = 60;

        public static int GetDailyTollFee(IVehicle vehicle, DateTime[] dates)
        {
            if (vehicle is null || dates is null || dates.Length is 0)
                return 0;

            var orderedDates = dates.OrderBy(d => d).ToArray();
            var intervalStart = orderedDates[0];
            var totalFee = 0;
            var maxIntervalFee = 0;

            foreach (var date in orderedDates)
            {
                int currentFee = GetSinglePassFee(date, vehicle);
                var minutesSinceIntervalStart = (date - intervalStart).TotalMinutes;

                if (minutesSinceIntervalStart <= 60)
                {
                    maxIntervalFee = Math.Max(maxIntervalFee, currentFee);
                }
                else
                {
                    totalFee += maxIntervalFee;
                    intervalStart = date;
                    maxIntervalFee = currentFee;
                }
            }

            totalFee += maxIntervalFee;

            return Math.Min(totalFee, MaxFee);
        }

        private static int GetSinglePassFee(DateTime date, IVehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle))
                return 0;

            return (date.Hour, date.Minute) switch
            {
                (6, >= 0 and <= 29) => 8,
                (6, >= 30 and <= 59) => 13,
                (7, _) => 18,
                (8, >= 0 and <= 29) => 13,
                (8, >= 30 and <= 59) => 8,
                (>= 9 and <= 14, _) => 8,
                (15, >= 0 and <= 29) => 13,
                (15, >= 30 and <= 59) or (16, _) => 18,
                (17, _) => 13,
                (18, >= 0 and <= 29) => 8,
                _ => 0
            };
        }

        private static bool IsTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle is null)
                return false;

            return Enum.TryParse<TollFreeVehicles>(vehicle.GetVehicleType(), out _);
        }

        private static bool IsTollFreeDate(DateTime date)
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return true;

            if (date.Year is not 2013)
                return false;

            return (date.Month, date.Day) switch
            {
                (1, 1) => true,
                (3, 28) or (3, 29) => true,
                (4, 1) or (4, 30) => true,
                (5, 1) or (5, 8) or (5, 9) => true,
                (6, 5) or (6, 6) or (6, 21) => true,
                (7, _) => true,
                (11, 1) => true,
                (12, 24) or (12, 25) or (12, 26) or (12, 31) => true,
                _ => false
            };
        }

        private enum TollFreeVehicles
        {
            Motorbike = 0,
            Tractor = 1,
            Emergency = 2,
            Diplomat = 3,
            Foreign = 4,
            Military = 5
        }
    }
}
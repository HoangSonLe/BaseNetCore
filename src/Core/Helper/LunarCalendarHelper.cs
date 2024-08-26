using Core.CommonModels.BaseModels;
using System.Globalization;

namespace Core.Helper
{
    public class LunarCalendarHelper
    {
        private static readonly ChineseLunisolarCalendar chineseCalendar = new ChineseLunisolarCalendar();
        /// <summary>
        /// Date is a input lunar date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsValidLunarDate(DateTime date)
        {
            try
            {
                // Convert the Gregorian date to a lunar date
                int lunarYear = date.Year;
                int lunarMonth = date.Month;
                int lunarDay = date.Day;
                int leapMonth = chineseCalendar.GetLeapMonth(lunarYear);

                // Check if the month is a leap month
                bool isLeapMonth = leapMonth > 0 && lunarMonth == leapMonth;

                // Attempt to create the same date in the Gregorian calendar using the lunar date components
                DateTime convertedBackDate = chineseCalendar.ToDateTime(lunarYear, lunarMonth, lunarDay, 0, 0, 0, 0, isLeapMonth ? 1 : 0);

                // If the converted back date is the same as the original date, it's valid
                return date.Date == convertedBackDate.Date;
            }
            catch
            {
                return false; // If any exception occurs, the date is not valid
            }
        }
        public static DateTime LunarToGregorian(LunarDate lunarDate)
        {
            return chineseCalendar.ToDateTime(lunarDate.Year, lunarDate.Month, lunarDate.Day, 0, 0, 0, 0, lunarDate.IsLeapMonth ? 1 : 0);
        }
        public static LunarDate GregorianToLunar(DateTime gregorianDate)
        {
            int year = chineseCalendar.GetYear(gregorianDate);
            int month = chineseCalendar.GetMonth(gregorianDate);
            int day = chineseCalendar.GetDayOfMonth(gregorianDate);
            bool isLeapMonth = chineseCalendar.IsLeapMonth(year, month);
            return new LunarDate(year, month, day, isLeapMonth);
        }
        private static DateTime GetLunarDate(int year, int month, int day)
        {
            // This method should return the corresponding solar date of the given lunar date.
            // For demonstration purposes, we'll just return the current date.
            // You need to replace this logic with actual lunar to solar date conversion.

            // Here’s a pseudo-logic assuming we have a way to convert a lunar date to solar date
            return chineseCalendar.ToDateTime(year, month, day, 0, 0, 0, 0); // Returns the corresponding solar date
        }
        public static LunarDate GetNearestLunarDate(int targetLunarDay, int targetLunarMonth)
        {
            DateTime currentDate = DateTime.Now;

            // Get the current lunar date
            int currentLunarYear = chineseCalendar.GetYear(currentDate);
            int currentLunarMonth = chineseCalendar.GetMonth(currentDate);
            int currentLunarDay = chineseCalendar.GetDayOfMonth(currentDate);

            // Calculate the next lunar date
            DateTime nextLunarDate = GetLunarDate(currentLunarYear, targetLunarMonth, targetLunarDay);
            DateTime previousLunarDate = GetLunarDate(currentLunarYear - (targetLunarMonth < currentLunarMonth || targetLunarMonth == currentLunarMonth && targetLunarDay < currentLunarDay ? 1 : 0), targetLunarMonth, targetLunarDay);

            // Check which date is closer to the current date
            TimeSpan timeToNext = nextLunarDate - currentDate;
            TimeSpan timeToPrevious = currentDate - previousLunarDate;

            return timeToNext < timeToPrevious ? GregorianToLunar(nextLunarDate) : GregorianToLunar(previousLunarDate);
        }

        public static LunarDate CalculateLunarAnniversaryFromCurrentDate(LunarDate originalDate)
        {
            // Get the current date
            DateTime currentGregorianDate = DateTime.Now;

            // Convert the current Gregorian date to the corresponding lunar date
            LunarDate currentLunarDate = GregorianToLunar(currentGregorianDate);

            // Calculate the anniversary year based on the original lunar date
            int anniversaryYear = currentLunarDate.Year + (currentLunarDate.Year >= originalDate.Year ? 0 : 1);

            // Return the anniversary lunar date
            return new LunarDate(anniversaryYear, originalDate.Month, originalDate.Day, originalDate.IsLeapMonth);
        }
        public static int CompareLunarDatesInDays(LunarDate lunarDate1, LunarDate lunarDate2)
        {
            DateTime gregorianDate1 = LunarToGregorian(lunarDate1);
            DateTime gregorianDate2 = LunarToGregorian(lunarDate2);

            // Calculate the difference in days
            TimeSpan difference = gregorianDate1 - gregorianDate2;
            return (int)difference.TotalDays;
        }
    }
}

namespace Core.CommonModels.BaseModels
{
    public class LunarDate : IComparable<LunarDate>

    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public bool IsLeapMonth { get; set; }

        public LunarDate(int year, int month, int day, bool isLeapMonth)
        {
            Year = year;
            Month = month;
            Day = day;
            IsLeapMonth = isLeapMonth;
        }

        public override string ToString()
        {
            return $"{(IsLeapMonth ? "Leap " : "")}Year: {Year}, Month: {Month}, Day: {Day}";
        }

        public int CompareTo(LunarDate other)
        {
            if (other == null) return 1;

            // Compare years
            if (Year != other.Year) return Year.CompareTo(other.Year);

            // Compare months
            if (Month != other.Month) return Month.CompareTo(other.Month);

            // Compare leap month status
            if (IsLeapMonth != other.IsLeapMonth) return IsLeapMonth ? 1 : -1;

            // Compare days
            return Day.CompareTo(other.Day);
        }
    }
}

namespace Infrastructure.Elastic.Models
{
    public class FlightInfo
    {
        public string FlightNum { get; set; }
        public string DestCountry { get; set; }
        public string OriginWeather { get; set; }
        public string OriginCityName { get; set; }
        public double AvgTicketPrice { get; set; }
        public double DistanceMiles { get; set; }
        public bool FlightDelay { get; set; }
        public string DestWeather { get; set; }
        public string Dest { get; set; }
        public string FlightDelayType { get; set; }
        public string OriginCountry { get; set; }
        public int DayOfWeek { get; set; }
        public double DistanceKilometers { get; set; }
        public DateTime Timestamp { get; set; }
        public Location DestLocation { get; set; }
        public string DestAirportID { get; set; }
        public string Carrier { get; set; }
        public bool Cancelled { get; set; }
        public double FlightTimeMin { get; set; }
        public string Origin { get; set; }
        public Location OriginLocation { get; set; }
        public string DestRegion { get; set; }
        public string OriginAirportID { get; set; }
        public string OriginRegion { get; set; }
        public string DestCityName { get; set; }
        public double FlightTimeHour { get; set; }
        public int FlightDelayMin { get; set; }
    }

    public class Location
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

}

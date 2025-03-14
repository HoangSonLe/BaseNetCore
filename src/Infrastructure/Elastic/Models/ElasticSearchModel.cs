namespace Infrastructure.Elastic.Models
{

    public class Shard
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public class Total
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

    public class Source : FlightInfo
    {

    }

    public class Hit
    {
        public string _index { get; set; }
        public string _id { get; set; }
        public int _version { get; set; }
        public object _score { get; set; }
        public List<string> _ignored { get; set; }
        public Source _source { get; set; }
        public List<long> sort { get; set; }
    }

    public class Hits
    {
        public Total total { get; set; }
        public object max_score { get; set; }
        public List<Hit> hits { get; set; }
    }
    public class LastAppearance
    {
        public double value { get; set; }
        public DateTime value_as_string { get; set; }
    }
    public class Aggregations
    {
        public GroupByDevice group_by_device { get; set; }
    }
    public class Bucket
    {
        public string key { get; set; }
        public int doc_count { get; set; }
        public GroupByVehicle group_by_vehicle { get; set; }
        public LastAppearance last_appearance { get; set; }
        public LastDocument last_document { get; set; }
        public FirstDocument first_document { get; set; }
    }
    public class LastDocument
    {
        public Hits hits { get; set; }
    }
    public class FirstDocument
    {
        public Hits hits { get; set; }
    }
    public class GroupByDevice
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }

    public class GroupByVehicle
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }
    public class Root
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shard _shards { get; set; }
        public Hits hits { get; set; }
        public Aggregations aggregations { get; set; }
    }
}

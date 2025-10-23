namespace NetBuild.Domain.Entities
{
    public class Schedule
    {
        public string TimeZone { get; set; }
        public Dictionary<int, List<TimeRange>> DaySchedules { get; set; }
    }

    public class TimeRange
    {
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }
}

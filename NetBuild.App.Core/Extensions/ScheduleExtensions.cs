using NetBuild.App.Core.ApiModel;

namespace NetBuild.App.Core.Extensions
{
    public static class ScheduleExtensions
    {
        public static bool IsValid(this Schedule schedule) {
            if (schedule == null || schedule.DaySchedules.Keys.Any(x => x < 0 || x > 6))
            {
                return false;
            }

            foreach (var daySchedule in schedule.DaySchedules.Values) {
                if (!ValidateTimeRanges(daySchedule)) {
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateTimeRanges(List<TimeRange> timeRanges)
        {
            try
            {
                var times = timeRanges.Select(x => new
                {
                    OpenTime = TimeOnly.ParseExact(x.OpenTime, "HH:mm"),
                    CloseTime = TimeOnly.ParseExact(x.CloseTime, "HH:mm")
                }
                ).ToList();
                times.Sort((a, b) => a.OpenTime.CompareTo(b.OpenTime));

                // Each individual range is valid
                foreach(var timeRange in times)
                {
                    if (timeRange.OpenTime.CompareTo(timeRange.CloseTime) >= 0) {
                        return false;
                    }
                }

                // Ranges don't overlap
                for(var i = 0; i < times.Count() - 1; i++) {
                    if (times[i].CloseTime.CompareTo(times[i+1].OpenTime) >= 0)
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

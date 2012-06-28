namespace SDammann.Utils.Phone.Shell {
    using System;


    public sealed class TimeSegment {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public TimeSegment()
                : this(8, 22) {
        }

        public TimeSegment(int startHour, int endHour) {
            this.StartTime = TimeSpan.FromHours(startHour);
            this.EndTime = TimeSpan.FromHours(endHour);
        }
    }
}
using System;

namespace ParkingManagement.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime DateUtc()
        {
            return DateTime.UtcNow.Date;
        }

        public DateTime DateUtc(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        public DateTime DateUtc(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public DateTime DateTimeUtc()
        {
            return DateTime.UtcNow;
        }

        public DateTime DateTimeUtc(int year, int month, int day, int hour, int minute, int second)
        {
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }

        public DateTime DateTimeUtc(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);
        }

        public DateTime DateTimeUtc(string date)
        {
            return DateTimeUtc(DateTime.Parse(date));
        }
    }
}

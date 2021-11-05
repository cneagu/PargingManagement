using System;

namespace ParkingManagement.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime DateUtc();
        DateTime DateUtc(int year, int month, int day);
        DateTime DateUtc(DateTime date);
        DateTime DateTimeUtc();
        DateTime DateTimeUtc(int year, int month, int day, int hour, int minute, int second);
        DateTime DateTimeUtc(DateTime date);
        DateTime DateTimeUtc(string date);
    }
}

using System;

namespace ParkingManagement.WebClient.Api.Models.Home
{
    public class DateInput
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public enum DateInputStatus
    {
        None = 0,
        Success = 1,
        NoDate = 2,
    }
}

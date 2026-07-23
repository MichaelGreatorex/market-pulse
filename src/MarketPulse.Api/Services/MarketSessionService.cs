using MarketPulse.Api.DTOs.Dashboard;

public class MarketSessionService
{
    public MarketStatusDto GetStatus()
    {
        var eastern = TimeZoneInfo.FindSystemTimeZoneById(
            "Eastern Standard Time");

        var now = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            eastern);

        var isWeekday =
            now.DayOfWeek >= DayOfWeek.Monday &&
            now.DayOfWeek <= DayOfWeek.Friday;

        var open =
            now.TimeOfDay >= new TimeSpan(9, 30, 0) &&
            now.TimeOfDay < new TimeSpan(16, 0, 0);

        var isOpen = isWeekday && open;

        return new MarketStatusDto
        {
            Country = "United States",
            Flag = "🇺🇸",
            Exchange = "NASDAQ",
            IsOpen = isOpen,
            NextEvent = isOpen
                ? "Closes 16:00 ET"
                : "Opens 09:30 ET"
        };
    }
}
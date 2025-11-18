using Microsoft.Maui.Storage;

namespace WhereISSit.Services;

public static class TimeFormat
{
    private const string TimeFormatKey = "selected_time_format";

    public static void Set(string format)
    {
        Preferences.Set(TimeFormatKey, format);
    }

    public static string Get()
    {
        return Preferences.Get(TimeFormatKey, "24h");
    }
}
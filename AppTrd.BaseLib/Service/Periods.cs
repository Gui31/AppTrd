namespace AppTrd.BaseLib.Service
{
    public enum Periods
    {
        Undefined = -1,
        OneSecond = 1, // SECOND
        OneMinute = 60, // MINUTE 
        TwoMinutes = 120, // MINUTE_2
        ThreeMinutes = 180, // MINUTE_3
        FiveMinutes = 300, // MINUTE_5
        TenMinutes = 600, // MINUTE_10
        FifteenMinutes = 900, // MINUTE_15
        ThirtYMinutes = 1800, // MINUTE_30
        OneHour = 3600, // HOUR
        TwoHours = 7200, // HOUR_2
        ThreeHours = 10800, // HOUR_3
        FourHours = 14400, // HOUR_4
        OneDay = 86400, // DAY
        OneWeek = 604800, // WEEK
        OneMonth = 2419200, // MONTH
    }
}
using System;

namespace dnSR_Coding
{
    public static class EventManager
    {
        public static Action<WeatherPreset> OnApplyingWeatherPreset;
        public static Action<Enums.Cursor_RelatedAction> OnCursorHover;
    }
}
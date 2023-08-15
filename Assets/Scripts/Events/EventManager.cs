using System;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    public static class EventManager
    {
        public static Action<WeatherPreset> OnApplyingWeatherPreset;
        public static Action<Enums.Cursor_SelectionType> OnCursorHover;
    }
}
using System;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    public static class EventManager
    {
        public readonly static CstmEvent<WeatherPreset> OnApplyingWeatherPreset = 
            new CstmEvent<WeatherPreset>();

        public readonly static CstmEvent<Enums.Cursor_SelectionType> OnCursorHover = 
            new CstmEvent<Enums.Cursor_SelectionType>();

        public readonly static CstmEvent OnCstmEventTestWithoutArgs = new CstmEvent();

        public readonly static CstmEvent<Enums.Cursor_SelectionType> OnCstmEventTestWithArgs = 
            new CstmEvent<Enums.Cursor_SelectionType>();
    }
}
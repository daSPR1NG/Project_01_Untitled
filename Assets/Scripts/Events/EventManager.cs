using System;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    public static class EventManager
    {
        public readonly static CstmEvent<WeatherPreset> WeatherSystem_OnApplyingWeatherPreset = 
            new CstmEvent<WeatherPreset>();

        public readonly static CstmEvent<Enums.Cursor_SelectionType> Selectable_OnCursorHover = 
            new CstmEvent<Enums.Cursor_SelectionType>();


        // Example ----------------

        //public readonly static CstmEvent OnCstmEventTestWithoutArgs = new CstmEvent();

        //public readonly static CstmEvent<Enums.Cursor_SelectionType> OnCstmEventTestWithArgs = 
        //    new CstmEvent<Enums.Cursor_SelectionType>();
    }
}
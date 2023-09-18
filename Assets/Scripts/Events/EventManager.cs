using System;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    public static class EventManager
    {
        #region Weather related

        public readonly static CstmEvent<WeatherPreset> WeatherSystem_OnApplyingWeatherPreset = 
            new CstmEvent<WeatherPreset>();

        public readonly static CstmEvent<WeatherEntity> WeatherEntity_OnRegistration = new CstmEvent<WeatherEntity>();
        public readonly static CstmEvent<WeatherEntity> WeatherEntity_OnUnregistration = new CstmEvent<WeatherEntity>();

        #endregion

        #region Cursor related

        public readonly static CstmEvent<Enums.Cursor_SelectionType> Selectable_OnCursorHover = 
            new CstmEvent<Enums.Cursor_SelectionType>();

        #endregion

        // Example ----------------

        //public readonly static CstmEvent OnCstmEventTestWithoutArgs = new CstmEvent();

        //public readonly static CstmEvent<Enums.Cursor_SelectionType> OnCstmEventTestWithArgs = 
        //    new CstmEvent<Enums.Cursor_SelectionType>();
    }
}
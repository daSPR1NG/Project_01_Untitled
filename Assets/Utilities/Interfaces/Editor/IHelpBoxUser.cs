using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> IHelpBoxUser description <summary>
    public interface IHelpBoxUser
    {
        public Rect? HelpBoxRect { get; set; }
        public float HelpBoxHeight { get; set; }

        public class IHelpBoxUserExtension
        {
            /// <summary>
            /// This extension is used to set the height once 
            /// to prevent the editor GUI to continually add the height to the GetPropertyHeight.
            /// </summary>
            /// <param name="helpBoxUser">The user.</param>
            /// <param name="newValue">The value you want to assign to helpBoxUser.Height.</param>
            public static void InitHelpBoxHeight( IHelpBoxUser helpBoxUser, float newValue )
            {
                if ( helpBoxUser.HelpBoxHeight != 0 ) { return; }

                helpBoxUser.HelpBoxHeight = newValue;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace HeliumDreams.Tools
{
    /// <summary>
    /// Uses a float to represent Mass. Can be accessed using many different Units of Mass.
    /// Will display in the inspector with choice of units
    /// </summary>
    [Serializable]
    public class Mass
    {
        /// <summary>
        /// Mass in Kilograms
        /// </summary>
        /// Used as default since its what a Unity Ridgidbody knows and loves
        public float kilograms = 0.0f;
        /// <summary>
        /// Mass in Metric Tonnes
        /// </summary>
        public float Tonnes
        {
            get
            {
                return kilograms * conversionsToKilograms["t."];
            }
            set
            {
                kilograms = value / conversionsToKilograms["t."];
            }
        }
        /// <summary>
        /// Mass in Grams
        /// </summary>
        public float Grams
        {
            get
            {
                return kilograms * conversionsToKilograms["g."];
            }
            set
            {
                kilograms = value / conversionsToKilograms["g."];
            }
        }
        /// <summary>
        /// Mass in Pounds
        /// </summary>
        public float Pounds
        {
            get
            {
                return kilograms * conversionsToKilograms["lb"];
            }
            set
            {
                kilograms = value / conversionsToKilograms["lb"];
            }
        }
        /// <summary>
        /// Mass in Ounces
        /// </summary>
        public float Ounces
        {
            get
            {
                return kilograms * conversionsToKilograms["oz"];
            }
            set
            {
                kilograms = value / conversionsToKilograms["oz"];
            }
        }

        #region Editor
        //Used by the custom property drawer to keep track of which unit to use in the editor
#if UNITY_EDITOR
        public string lastUnit = "kg";
#endif
        //Dictionry of all the conversation values between the unit and kilograms
        //All codes must be two characters at the moment for the custom property drawer
        //Used by the custom property drawer and all the public properties
        public static Dictionary<string, float> conversionsToKilograms = new Dictionary<string, float>
        {
            {"kg",1.0f},
            {"t.",1000.0f},
            {"g.", 0.001f},
            {"lb",0.453592f},
            {"oz", 0.0283495f}
        };
        #endregion

        #region Conversion
        //This stuff will allow you to treat this Type as a float directly, accessing the kilogram property
        //This means you can use Rigidbody.Mass = Mass instead of Rigidbody.Mass = Mass.kilograms
        public Mass()
        {
            kilograms = 0;
#if UNITY_EDITOR
            lastUnit = "kg";
#endif
        }
        public Mass(float f)
        {
            kilograms = f;
#if UNITY_EDITOR
            lastUnit = "kg";
#endif
        }

        public static implicit operator float(Mass w) => w.kilograms;
        public static explicit operator Mass(float f) => new Mass(f);

        #endregion
    }
}

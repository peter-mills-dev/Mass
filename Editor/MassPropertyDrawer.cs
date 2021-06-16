using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeliumDreams.Tools.Editor
{
    /// <summary>
    /// Custom Propery Drawer for Mass. Allows switching of displayed units and on the fly conversion
    /// </summary>
    [CustomPropertyDrawer(typeof(Mass))]
    public class MassPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //Grab the current unit this property is expecting to display
            string unitToUse = property.FindPropertyRelative("lastUnit").stringValue;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var rect = new Rect(position.position, Vector2.one * 20);

            if(EditorGUI.DropdownButton(
                rect, 
                new GUIContent(GetTexture()), 
                FocusType.Keyboard, 
                new GUIStyle() { fixedWidth = 50f, border = new RectOffset(1,1,1,1)}))
            {
                GenericMenu menu = new GenericMenu();

                foreach (var item in Mass.conversionsToKilograms)
                {
                    menu.AddItem(new GUIContent(item.Key),
                    (item.Key == unitToUse), () => SetUnit(property, item.Key));
                }

                menu.ShowAsContext();
            }

            position.position += Vector2.right * 5;

            //Will hold the new value of the field in its value, and the units to apply in its key
            KeyValuePair<string, float> value;

            value = HandleWeightValuePropertyDrawer(position, property, Mass.conversionsToKilograms[unitToUse], unitToUse);

            //Set the value of Mass to the new value with the correct conversion. 
            property.FindPropertyRelative("kilograms").floatValue = value.Value * Mass.conversionsToKilograms[value.Key];

            /*
            //NOTE: Uncomment this and typing in a unit will automatically change the displayed units. 
            //At the moment, if lastUnit = "kg" and you type "1 t." the final display would be "1000 kg"
            //Uncomment this, and you will change lastUnit to "t." and the final display would be "1 t."
            if (value.Key != unitToUse)
            {
                SetUnit(property, value.Key);
            }
            */
        }

        //Draws a text field and handles getting the right units in and out of it
        private static KeyValuePair<string, float> HandleWeightValuePropertyDrawer(Rect position, SerializedProperty property, float conversionFactor, string units)
        {
           
            //Get the starting value
            float value = property.FindPropertyRelative("kilograms").floatValue / conversionFactor;

            //Set the default numbers we want to return here, we only edit them later if needed
            float returnFloat = value;
            string returnString = units;

            //Get a new value from the text field, add the units onto it so that users know what unit mode they are in
            string newValue = EditorGUI.TextField(position, value.ToString() + " " + units);

            //Check to make sure there are enough characters to actually contain a unit
            if (newValue.Length > 2)
            {
                //This is where the hardcoded unit length is used. 
                var suffix = newValue.Substring(newValue.Length - 2, 2);

                //If the suffix matches the units we came in with, we are all good, just shorten the newValue to remove the units
                if (suffix == units)
                {
                    newValue = newValue.Substring(0, newValue.Length - 2);
                }
                //If the suffix is any other key in the conversion dictionary, we want to save that suffix to return and still cut the suffix from the newValue
                else if (Mass.conversionsToKilograms.ContainsKey(suffix))
                {
                    returnString = suffix;
                    newValue = newValue.Substring(0, newValue.Length - 2);
                }
            }

            //Try and turn the newValue into a float (if it had a suffix it should have been removed)
            if(float.TryParse(newValue, out value))
            {
                //Only save to the return value if it was succesful. Junk being typed it will result in the number not changing
                returnFloat = value;
            }

            KeyValuePair<string, float> returnValue = new KeyValuePair<string, float>(returnString, returnFloat);
            
            return returnValue;
        }

        private void SetUnit(SerializedProperty property, string value)
        {
            var propRelative = property.FindPropertyRelative("lastUnit");
            propRelative.stringValue = value;
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// This is using a hardcoded reference to a texture in the Unity.UI package.
        /// If you dont have it, clicking the spot will still work, but it'll be blank
        /// </summary>
        /// <returns></returns>
        private Texture GetTexture()
        {
            var textures = Resources.FindObjectsOfTypeAll(typeof(Texture))
                .Where(t => t.name.ToLower().Contains("verticallayoutgroup"))
                .Cast<Texture>().ToList();

            if (textures.Count > 0)
            {
                return textures[0];
            }
            else
            {
                return null;
            }
        }
    }
}

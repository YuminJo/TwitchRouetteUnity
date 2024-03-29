﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HSVPicker
{
    [RequireComponent(typeof(TMP_InputField))]
    [DefaultExecutionOrder(10)]
    public class HexColorField : MonoBehaviour
    {
        public ColorPicker hsvpicker;

        public bool displayAlpha;

        public TMP_InputField hexInputField;

        public Image RouetteImg;
        

        private void Start()
        {
            hexInputField = GetComponent<TMP_InputField>();

            // Add listeners to keep text (and color) up to date
            hexInputField.onEndEdit.AddListener(UpdateColor);
            hsvpicker.onValueChanged.AddListener(UpdateHex);
            UpdateHex(hsvpicker.CurrentColor);
        }

        private void OnDestroy()
        {
            hexInputField.onValueChanged.RemoveListener(UpdateColor);
            hsvpicker.onValueChanged.RemoveListener(UpdateHex);
        }

        private void UpdateHex(Color newColor)
        {
            hexInputField.text = ColorToHex(newColor);
        }

        public void UpdateColor(string newHex)
        {
            Color color;
            if (!newHex.StartsWith("#"))
                newHex = "#"+newHex;
            if (ColorUtility.TryParseHtmlString(newHex, out color))
            {
                hsvpicker.CurrentColor = color;
                Camera.main.backgroundColor = color;
                RouetteImg.color = color;
            }
            else
                Debug.Log("hex value is in the wrong format, valid formats are: #RGB, #RGBA, #RRGGBB and #RRGGBBAA (# is optional)");
        }

        private string ColorToHex(Color32 color)
        {
            return displayAlpha
                ? string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a)
                : string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
        }
    }
}
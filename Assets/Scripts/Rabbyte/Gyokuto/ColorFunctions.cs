using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class LuaMethods
{
    public static Color GetColorByString(string color)
    {
        Color selectedColor;
        switch (color.ToLower())
        {
            case "blue":
                selectedColor = Color.blue;
                break;
            case "black":
                selectedColor = Color.black;
                break;
            case "cyan":
                selectedColor = Color.cyan;
                break;
            case "gray":
                selectedColor = Color.gray;
                break;
            case "green":
                selectedColor = Color.green;
                break;
            case "grey":
                selectedColor = Color.grey;
                break;
            case "magenta":
                selectedColor = Color.magenta;
                break;
            case "red":
                selectedColor = Color.red;
                break;
            case "white":
                selectedColor = Color.white;
                break;
            case "yellow":
                selectedColor = Color.yellow;
                break;
            default:
                Color newColor;
                if (ColorUtility.TryParseHtmlString(color, out newColor))
                {
                    selectedColor = newColor;
                }
                else
                    selectedColor = Color.white;

                break;
        }

        return selectedColor;
    }
}

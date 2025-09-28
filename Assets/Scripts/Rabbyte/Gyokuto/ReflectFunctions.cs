using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Rabbyte.Gyotoku;

public static partial class LuaMethods
{
    readonly static Dictionary<string, string> keyCodes = new Dictionary<string, string>()
    {
        {"x", "transform.position.x"},
        {"y", "transform.position.y" },
        {"z", "transform.poistion.z" },
        {"alpha", "color.a" }
    };

    readonly static Dictionary<string, string> altKeyCodes = new Dictionary<string, string>()
    {
        {"x", "GetComponent<RectTransform>().anchoredPosition.x"},
        {"y", "GetComponent<RectTransform>().anchoredPosition.y" },
        {"alpha", "color.a" }
    };

#if NET_4_6
    /// <summary>
    /// Get the value of an object via string
    /// </summary>
    /// <param name="property">The property name</param>
    /// <returns></returns>
    public static dynamic GetProperty(string property)
    {
        return null;
    }
    /// <summary>
    /// Sets the value of an object via string
    /// </summary>
    /// <param name="property">The property name</param>
    /// <param name="value">The new value</param>
    public static void SetProperty(string property, dynamic value)
    {

    }
#endif
}

public static partial class LuaMethods
{
    readonly static Dictionary<string, dynamic> globals = new Dictionary<string, dynamic>()
    {
        //Camera Properties
        { "Shake",  (Action<float, float, GameObject>)ShakeScreen },
        { "CameraShake",  (Action<float, float>)ShakeCamera },
        //Reflect Properties
        { "GetProperty", (Func<string, dynamic>)GetProperty },
        { "SetProperty", (Action<string, dynamic>)SetProperty },
        //CharacterProperties
        { "SetExpression", (Action<string, string>)SetExpressionToCharacter },
    };
    static LuaMethods()
    {
        LuaFunctions.AddGlobalList(globals);
    }

    public static void Load()
    {

    }
}
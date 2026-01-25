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

    public static void Read(string message)
    {
        Debug.Log(message);
    }
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
        { "DebugLog", (Action<string>)Read},
        //CharacterProperties
        { "SetExpression", (Action<string, string>)SetExpressionToCharacter },
    };
    static LuaMethods()
    {
        
    }

    public static void Load()
    {
        LuaFunctions.AddGlobalList(globals);
    }

    public static void AddGlobal(string name, dynamic dyn)
    {
        if (!globals.ContainsKey(name))
            globals.Add(name, dyn);
    }
    public static void AddGlobal(GlobalDictionary dic)
    {
        if (!globals.ContainsKey(dic.name))
            globals.Add(dic.name, dic.dyn);
    }
    public static void AddGlobal(List<GlobalDictionary> dics)
    {
        foreach(GlobalDictionary dic in dics)
            if (!globals.ContainsKey(dic.name))
                globals.Add(dic.name, dic.dyn);
    }
    public static void AddGlobal(Dictionary<string, dynamic> newGlobals)
    {
        foreach(KeyValuePair<string, dynamic> global in newGlobals)
            if (!globals.ContainsKey(global.Key))
                globals.Add(global.Key, global.Value);
    }
}

public struct GlobalDictionary
{
    public string name;
    public dynamic dyn;
}
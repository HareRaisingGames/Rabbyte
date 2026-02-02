using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Rabbyte.Gyotoku;

public static partial class LuaMethods
{
    static string _typeName;
    static dynamic _typeInstance;
    
    public static void SetInstance(dynamic instance)
    {
        _typeInstance = instance;
        _typeName = _typeInstance.GetType().Name;
    }
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
    static Type GetTypeAnywhere(string typeName)
    {
        // First, try the basic GetType which works for types in the current or mscorlib assembly
        Type type = Type.GetType(typeName);
        if (type != null) return type;

        // If not found, search all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null; // Type not found in any assembly
    }

    static Type GetTypeFromName(string typeName)
    {
        Type t = Type.GetType(typeName);
        if (t != null) return t;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach(Type type in assembly.GetTypes())
            {
                if (type.Name == typeName)
                {
                    return type;
                }
                    
            }
        }
        return null;
    }

    /// <summary>
    /// A technique on retrieving both the appropriate info and the outgoing instance
    /// </summary>
    /// <param name="property">The property name</param>
    /// <param name="instance">The output instance - when the variable is about to be recieved, it'll look for that specific one</param>
    /// <returns></returns>
    static dynamic GetPropertyType(string property, out dynamic instance)
    {
        Type type = GetTypeAnywhere(_typeName);
        instance = _typeInstance;

        FieldInfo field = type.GetField(property, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if(field != null)
            return field;

        PropertyInfo prop = type.GetProperty(property, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (prop != null)
            return prop;

        string[] propertyBreak = property.Split(".");
        for(int i = 0; i < propertyBreak.Length; i++)
        {
            /*if (propertyBreak[i].Contains("GetComponent"))
            {
                string typeName = propertyBreak[i].Replace("GetComponent", "")
                                                  .Replace("<", "")
                                                  .Replace(">", "")
                                                  .Replace("(", "")
                                                  .Replace(")", "")
                                                  .Replace("typeof", "");
                Type t = GetTypeFromName(typeName);
                if(t != null)
                {
                    //Debug.Log(t);
                    MethodInfo methodInfo = typeof(Component).GetMethod("GetComponent", new Type[] { });
                    MethodInfo specificGetComponent = methodInfo.MakeGenericMethod(t);
                    if (i >= propertyBreak.Length - 1) return specificGetComponent;
                    Debug.Log(instance);
                    //Component component = specificGetComponent.Invoke(instance.gameObject, null) as Component;
                    Debug.Log("Hi!");
                    //Debug.Log(specificGetComponent);
                    //Debug.Log("Testing!");
                    //Component component = instance.GetComponent(t);
                    //if (i >= propertyBreak.Length - 1) return component;
                    //instance = component;
                    //Debug.Log(component != null);
                }
            }
            else
            {*/
                field = type.GetField(propertyBreak[i], BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                prop = type.GetProperty(propertyBreak[i], BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                if (field != null || prop != null)
                {
                    if (field != null)
                    {
                        type = field.FieldType;
                        if (i >= propertyBreak.Length - 1) return field;
                        instance = (dynamic)field.GetValue(instance);
                    }
                    else if (prop != null)
                    {
                        type = prop.PropertyType;
                        if (i >= propertyBreak.Length - 1) return prop;
                        instance = (dynamic)prop.GetValue(instance);
                    }
                }
                else
                    break;
            }
        //}
            
        return null;
    }

    /// <summary>
    /// Get the value of an object via string
    /// </summary>
    /// <param name="property">The property name</param>
    /// <returns></returns>
    public static dynamic GetProperty(string property)
    {
        //Find the type object that this will be called out
        dynamic instance;
        try
        {
            dynamic field = GetPropertyType(property, out instance);
            if (field is FieldInfo || field is PropertyInfo)
                return (dynamic)field.GetValue(instance);
            else if (field is MethodInfo)
                return field.Invoke(instance, null);
        }
        catch(Exception ex)
        {
            Debug.LogError("The following property does not exist");
            return null;
        }
        return null;

    }
    /// <summary>
    /// Sets the value of an object via string
    /// </summary>
    /// <param name="property">The property name</param>
    /// <param name="value">The new value</param>
    public static void SetProperty(string property, dynamic value)
    {
        dynamic output;
        try
        {
            dynamic field = GetPropertyType(property, out output);

            //Check if the object is a vector property
            bool vector = property.EndsWith(".x") || property.EndsWith(".y") || property.EndsWith(".z");
            if (vector)
            {
                string filtered = property.Remove(property.Length - 2);

                dynamic tOutput;
                dynamic tField = GetPropertyType(filtered, out tOutput);

                //Grab the last two variables from the vector
                string check = $"{property[property.Length - 2]}{property[property.Length - 1]}";

            //Check if it uses FieldInfo or PropertyInfo
            bool accepted = false;

            if (tField is FieldInfo) 
                accepted = tField.FieldType == typeof(Vector3) || tField.FieldType == typeof(Vector2);
            else if (tField is PropertyInfo) 
                accepted = tField.PropertyType == typeof(Vector3) || tField.PropertyType == typeof(Vector2);

                if (accepted)
                {
                    switch (check)
                    {
                        case ".x":
                            tField.SetValue(tOutput, new Vector3(value, GetProperty($"{filtered}.y"), GetProperty($"{filtered}.z")));
                            break;
                        case ".y":
                            tField.SetValue(tOutput, new Vector3(GetProperty($"{filtered}.x"), value, GetProperty($"{filtered}.z")));
                            break;
                        case ".z":
                            tField.SetValue(tOutput, new Vector3(GetProperty($"{filtered}.x"), GetProperty($"{filtered}.y"), value));
                            break;
                    }
                }
                else
                    field.SetValue(output, value);
            }
            else
                field.SetValue(output, value);
        }
        catch (Exception ex)
        {
            Debug.LogError("Invalid casting or parameter does not exist");
        }


    }
#endif

    static void Read(object message)
    {
        Debug.Log(message);
    }

    static void Error(object message)
    {
        Debug.LogError(message);
    }

    static void Warning(object message)
    {
        Debug.LogWarning(message);
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
        { "GetColor", (Func<string, Color>)GetColorByString},
        //Debug Properties
        { "DebugLog", (Action<object>)Read},
        { "DebugError", (Action<object>)Error},
        { "DebugWarning", (Action<object>)Warning},
        //Character Properties
        { "SetExpression", (Action<string, string>)SetExpressionToCharacter },
        { "GetExpression", (Func<string, string>)GetExpressionFromCharacter }
    };
    static LuaMethods()
    {
        
    }

    public static void Load()
    {
        LuaFunctions.AddGlobalList(globals);
    }
    /// <summary>
    /// Add a global via string and value
    /// </summary>
    /// <param name="name">The string tag</param>
    /// <param name="dyn">The dynamic value</param>
    public static void AddGlobal(string name, dynamic dyn)
    {
        if (!globals.ContainsKey(name))
            globals.Add(name, dyn);
        else
            globals[name] = dyn;
    }
    public static void AddGlobal(GlobalDictionary dic)
    {
        if (!globals.ContainsKey(dic.name))
            globals.Add(dic.name, dic.dyn);
        else
            globals[dic.name] = dic.dyn;
    }
    public static void AddGlobal(List<GlobalDictionary> dics)
    {
        foreach (GlobalDictionary dic in dics)
            if (!globals.ContainsKey(dic.name))
                globals.Add(dic.name, dic.dyn);
            else
                globals[dic.name] = dic.dyn;
    }
    public static void AddGlobal(Dictionary<string, dynamic> newGlobals)
    {
        foreach (KeyValuePair<string, dynamic> global in newGlobals)
            if (!globals.ContainsKey(global.Key))
                globals.Add(global.Key, global.Value);
            else
                globals[global.Key] = global.Value;
    }
}

public struct GlobalDictionary
{
    public string name;
    public dynamic dyn;
}
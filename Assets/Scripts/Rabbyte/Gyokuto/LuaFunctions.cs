using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using System;
using System.Linq;

namespace Rabbyte.Gyotoku
{
    public static class LuaFunctions
    {
        static SimpleSBDFile _file = new SimpleSBDFile();
        public static SimpleSBDFile dialogueFile
        {
            set
            {
                _file = value;
                script = new Script();
            }
        }

        static Script script;

        public static void DoShake(GameObject obj)
        {
            //UserData.RegisterAssembly(typeof(GameObject).Assembly);
            string code = @"
                Shake(2, 2, obj)
            ";
            script.Globals["obj"] = obj;
            script.DoString(code);
        }

        /// <summary>
        /// Calls every time the line starts
        /// </summary>
        public static void OnLineStart()
        {
            //Insert();
            string code = @$"
                function onLineStart()
                    {_file.curLine.onStart}
                end
            ";
            //Debug.Log("Beginning");
            //Debug.Log(code);
            script.DoString(code);
            script.Call(script.Globals["onLineStart"]);
        }

        /// <summary>
        /// Calls every time the line ends
        /// </summary>
        public static void OnLineEnd()
        {
            //Insert();
            string code = @$"
                function onLineEnd()
                    {_file.curLine.onEnd}
                end
            ";
            //Debug.Log("End");
            Debug.Log(_file.onEnd);
            Debug.Log(code);
            script.DoString(code);
            script.Call(script.Globals["onLineEnd"]);
        }

        /// <summary>
        /// Calls at a specific interval of the line
        /// </summary>
        /// <param name="i">The interval of the line</param>
        public static void OnLineInterval(int i)
        {
            //Insert();
            string code = @$"
                function onLineInterval(num)
                    {_file.curLine.onWord}
                end
            ";
            script.Globals["num"] = i;
            script.DoString(code);
            script.Call(script.Globals["onLineInterval"]);
        }

        ///<summary>
        /// These are global functions that a user can access via these functions below
        /// </summary>

        #region Meta
        static Dictionary<string, dynamic> globals = new Dictionary<string, dynamic>()
        {
            { "Shake",  (Action<float, float, GameObject>)LuaMethods.ShakeScreen }
        };
        static void AddGlobals()
        {
            foreach(KeyValuePair<string, dynamic> global in globals)
            {
                script.Globals[global.Key] = global.Value;
            }
        }
#if NET_4_6
        /// <summary>
        /// Adds a global parameter to MoonSharp: WARNING - API Level must be set to .NET Framework 
        /// </summary>
        /// <param name="name">The string name for the global parameter</param>
        /// <param name="value">The value of the global</param>
        public static void AddGlobal(string name, dynamic value)
        {
            if (!globals.ContainsKey(name))
                globals.Add(name, value);
            else
                globals[name] = value;
            AddGlobals();
        }

        public static void AddGlobalList(Dictionary<string, dynamic> list)
        {
            foreach(KeyValuePair<string, dynamic> obj in list)
            {
                if (!globals.ContainsKey(obj.Key))
                    globals.Add(obj.Key, obj.Value);
                else
                    globals[obj.Key] = obj.Value;
            }
            AddGlobals();
        }
#endif

        static List<Type> types = new List<Type>() {
            typeof(MonoBehaviour),
            typeof(GameObject),
            typeof(Transform),
            typeof(SimpleSBDFile)
        };

        /// <summary>
        /// Adds a class type to MoonSharp and registers that type
        /// </summary>
        /// <param name="t">The new type</param>
        public static void AddTypeToRegisteration(Type t)
        {
            if (!types.Contains(t)) types.Add(t);
            foreach (Type type in types)
                UserData.RegisterType(type);
        }

        /// <summary>
        /// Adds a list of class types MoonSharp and filters out any duplicate classes.
        /// </summary>
        /// <param name="tList">A list of types</param>
        public static void AddTypesToRegisteration(List<Type> tList)
        {
            List<Type> selfMerge = tList.Union(tList).ToList();
            List<Type> uniqueList = selfMerge.Union(types).ToList();
            types.AddRange(uniqueList);
            foreach (Type type in types)
                UserData.RegisterType(type);
        }

#endregion

        static LuaFunctions()
        {
            Insert();
        }

        static void Insert()
        {
            script = new Script();
            foreach (Type type in types)
                UserData.RegisterType(type);
            AddGlobals();
            LuaMethods.Load();
        }
    }
}


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
            string code = @$"
                function onLineStart()
                    {_file.onStart}
                end
            ";
            script.DoString(code);
            script.Call(script.Globals["onLineStart"]);
        }

        /// <summary>
        /// Calls every time the line ends
        /// </summary>
        public static void OnLineEnd()
        {
            string code = @$"
                function onLineEnd()
                    {_file.onEnd}
                end
            ";
            script.DoString(code);
            script.Call(script.Globals["onLineEnd"]);
        }

        /// <summary>
        /// Calls at a specific interval of the line
        /// </summary>
        /// <param name="i">The interval of the line</param>
        public static void OnLineInterval(int i)
        {
            string code = @$"
                function onLineInterval(num)
                    {_file.onWord}
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
            { "Shake",  (Action<float, float, GameObject>)ShakeScreen }
        };
        static void AddGlobals()
        {
            foreach(KeyValuePair<string, dynamic> global in globals)
            {
                script.Globals[global.Key] = global.Value;
            }
        }

        /// <summary>
        /// Adds a global parameter to MoonSharp: WARNING - API Level must be set to .NET Framework 
        /// </summary>
        /// <param name="name">The string name for the global parameter</param>
        /// <param name="value">The value of the global</param>
#if NET_4_6
        public static void AddGlobal(string name, dynamic value)
        {

            bool containsValue()
            {
                foreach (KeyValuePair<string, dynamic> global in globals)
                {

                    if (global.Value.Equals(value))
                        return true;
                    
                }
                return false;
            };

            if (!globals.ContainsKey(name) && !containsValue())
                globals.Add(name, value);
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
            script = new Script();
            foreach(Type type in types)
                UserData.RegisterType(type);
            AddGlobals();
        }

        public static IEnumerator Shake(float duration, float magnitude, GameObject obj)
        {
            bool ui = false;
            Vector3 originalPos = obj.transform.localPosition;
            if(obj.GetComponent<RectTransform>() != null)
            {
                ui = true;
                originalPos = obj.GetComponent<RectTransform>().anchoredPosition;
            }

            float elapsed = 0.0f;

            while(elapsed < duration)
            {
                if(ui)
                {
                    float offsetX = Mathf.Sin(Time.time * UnityEngine.Random.Range(-1f, 1f)) * magnitude;
                    float offsetY = Mathf.Cos(Time.time * UnityEngine.Random.Range(-1f, 1f)) * magnitude; // Slightly different speed for y-axis

                    obj.GetComponent<RectTransform>().anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0);
                }
                else
                {
                    float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                    float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

                    obj.transform.localPosition = new Vector3(x, y, originalPos.z);
                }

                

                elapsed += Time.deltaTime;

                yield return null;
            }

            if(ui)
                obj.GetComponent<RectTransform>().anchoredPosition = originalPos;
            else
                obj.transform.localPosition = originalPos;
        }

        public static void ShakeScreen(float duration, float magnitude, GameObject obj)
        {
            IEnumerator shake = Shake(duration, magnitude, obj);
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>().StartCoroutine(shake);
            //while (shake.MoveNext());
        }
    }
}


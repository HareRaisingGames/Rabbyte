using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class LuaMethods
{
    //For multiple shakes
    static Dictionary<GameObject, Vector3> originShakePositions = new Dictionary<GameObject, Vector3>();
    static IEnumerator Shake(float duration, float magnitude, GameObject obj)
    {
        bool ui = false;
        if (!originShakePositions.ContainsKey(obj))
            originShakePositions.Add(obj, obj.transform.localPosition);

        if (obj.GetComponent<RectTransform>() != null)
        {
            ui = true;
            if (originShakePositions.ContainsKey(obj))
                originShakePositions[obj] = obj.GetComponent<RectTransform>().anchoredPosition;
        }

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            if (ui)
            {
                float offsetX = Mathf.Sin(Time.time * Random.Range(-1f, 1f)) * magnitude;
                float offsetY = Mathf.Cos(Time.time * Random.Range(-1f, 1f)) * magnitude; // Slightly different speed for y-axis
                if (originShakePositions.ContainsKey(obj))
                    obj.GetComponent<RectTransform>().anchoredPosition = originShakePositions[obj] + new Vector3(offsetX, offsetY, 0);
            }
            else
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                if (originShakePositions.ContainsKey(obj))
                    obj.transform.localPosition = new Vector3(x, y, originShakePositions[obj].z);
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        if (originShakePositions.ContainsKey(obj))
            if (ui)
                obj.GetComponent<RectTransform>().anchoredPosition = originShakePositions[obj];
            else
                obj.transform.localPosition = originShakePositions[obj];

        originShakePositions.Remove(obj);


    }

    public static void ShakeScreen(float duration, float magnitude, GameObject obj)
    {
        IEnumerator shake = Shake(duration, magnitude, obj);
        UnityEngine.Object.FindObjectOfType<MonoBehaviour>().StartCoroutine(shake);
        //while (shake.MoveNext());
    }

    public static void ShakeCamera(float duration, float magnitude)
    {
        IEnumerator shake = Shake(duration, magnitude, Camera.main.gameObject);
        UnityEngine.Object.FindObjectOfType<MonoBehaviour>().StartCoroutine(shake);
        //while (shake.MoveNext());
    }
}

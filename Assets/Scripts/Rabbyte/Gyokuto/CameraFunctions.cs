using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class LuaMethods
{
    public static IEnumerator Shake(float duration, float magnitude, GameObject obj)
    {
        bool ui = false;
        Vector3 originalPos = obj.transform.localPosition;
        if (obj.GetComponent<RectTransform>() != null)
        {
            ui = true;
            originalPos = obj.GetComponent<RectTransform>().anchoredPosition;
        }

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            if (ui)
            {
                float offsetX = Mathf.Sin(Time.time * Random.Range(-1f, 1f)) * magnitude;
                float offsetY = Mathf.Cos(Time.time * Random.Range(-1f, 1f)) * magnitude; // Slightly different speed for y-axis

                obj.GetComponent<RectTransform>().anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            }
            else
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                obj.transform.localPosition = new Vector3(x, y, originalPos.z);
            }



            elapsed += Time.deltaTime;

            yield return null;
        }

        if (ui)
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

    public static void ShakeCamera(float duration, float magnitude)
    {
        IEnumerator shake = Shake(duration, magnitude, Camera.main.gameObject);
        UnityEngine.Object.FindObjectOfType<MonoBehaviour>().StartCoroutine(shake);
        //while (shake.MoveNext());
    }
}

public static partial class LuaMethods
{

}

using UnityEditor;
using UnityEngine;
using Rabbyte;

public class CloseHandler
{
    #region Editor Properties
    #if UNITY_EDITOR
    /// This method is called when the script is loaded or a recompile occurs
    [InitializeOnLoadMethod]
    private static void OnInitialize()
    {
        // Subscribe to the 'quitting' event
        EditorApplication.quitting += OnEditorQuitting;
    }

    // This method will be called when the Editor is quitting
    private static void OnEditorQuitting()
    {
        Debug.Log("Unity Editor is quitting. Performing cleanup tasks...");
        StarbornFileHandler.ClearCache();
        // Add any specific actions or cleanup logic here,
        // such as saving data, releasing resources, etc.
    }
    #endif
    #endregion

    #region Runtime Properties
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnRuntimeInitialize()
    {
        Application.quitting += onRuntimeQutting;
    }

    private static void onRuntimeQutting()
    {
        StarbornFileHandler.ClearCache();
    }
    #endregion
}

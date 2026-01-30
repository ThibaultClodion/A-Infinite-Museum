using UnityEditor;
using UnityEngine;

public class QuickFunctionTest : MonoBehaviour
{
    [Header("Function to Test")]
    public MonoBehaviour targetScript;

    [Header("Method Name")]
    public string methodName;

    public void ExecuteTestFunction()
    {
        if (targetScript == null)
        {
            Debug.LogError("Target script is not assigned!");
            return;
        }

        if (string.IsNullOrEmpty(methodName))
        {
            Debug.LogError("Method name is not specified!");
            return;
        }

        var method = targetScript.GetType().GetMethod(methodName, 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);

        if (method == null)
        {
            Debug.LogError($"Method '{methodName}' not found on {targetScript.GetType().Name}!");
            return;
        }

        try
        {
            Debug.Log($"Executing {targetScript.GetType().Name}.{methodName}()...");
            method.Invoke(targetScript, null);
            Debug.Log("Execution completed.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing {methodName}: {e.Message}\n{e.StackTrace}");
        }
    }

    public void ListAvailableMethods()
    {
        if (targetScript == null)
        {
            Debug.LogError("Target script is not assigned!");
            return;
        }

        var methods = targetScript.GetType().GetMethods(
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance | 
            System.Reflection.BindingFlags.DeclaredOnly);

        Debug.Log($"Available methods on {targetScript.GetType().Name}:");
        foreach (var method in methods)
        {
            Debug.Log($"  - {method.Name}");
        }
    }
}

[CustomEditor(typeof(QuickFunctionTest))]
public class QuickFunctionTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuickFunctionTest script = (QuickFunctionTest)target;

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Execute Test Function", GUILayout.Height(40)))
        {
            script.ExecuteTestFunction();
        }

        if (GUILayout.Button("List Available Methods", GUILayout.Height(30)))
        {
            script.ListAvailableMethods();
        }
    }
}
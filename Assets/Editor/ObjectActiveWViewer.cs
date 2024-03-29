using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ObjectActiveViewer
{
    private const int WIDTH = 16;

    static ObjectActiveViewer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }

    private static void OnGUI(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject == null) return;

        var position = selectionRect;

        position.x = position.xMax - WIDTH+20;
        position.width = WIDTH;

        var newActive = GUI.Toggle(position, gameObject.activeSelf, string.Empty);

        if (newActive == gameObject.activeSelf) return;

        gameObject.SetActive(newActive);
    }
}
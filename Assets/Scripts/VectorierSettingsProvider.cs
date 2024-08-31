using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

// -=-=-=- //

public class VectorierSettingsProvider : SettingsProvider
{
    private bool useShortcutLaunch;

    public VectorierSettingsProvider(string path, SettingsScope scopes, IEnumerable<string>? keywords = null) : base(path, scopes, keywords)
    {
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        // pass
    }

    public override void OnGUI(string searchContext)
    {
        EditorGUILayout.BeginVertical(new GUIStyle
        {
            fixedHeight = 100F,
            stretchWidth = true,
            padding = new RectOffset(0, 0, 15, 0)
        });


         var roomsDirectory = VectorierSettings.RoomsDirectory;
        MakeTextField(ref roomsDirectory, "Rooms File Directory", "Rooms file location directory");

        EditorGUILayout.EndVertical();
        EditorPrefs.SetString(VectorierSettings.RoomsDirectoryKey, roomsDirectory);
    }

    private void MakeTextField(ref string? value, string textFieldlabel, string tooltip)
    {
        value ??= "";
        var guiContent = new GUIContent(textFieldlabel, tooltip);
        value = EditorGUILayout.TextField(guiContent, value);
    }

    private bool MakeToggle(bool value, string toggleLabel, string tooltip)
    {
        var guiContent = new GUIContent(toggleLabel, tooltip);
        return EditorGUILayout.Toggle(guiContent, value);
    }

    [SettingsProvider]
    public static SettingsProvider CreateVectorierSettingsProvider()
    {
        return new VectorierSettingsProvider("Project/Vectorier", SettingsScope.Project);
    }
}
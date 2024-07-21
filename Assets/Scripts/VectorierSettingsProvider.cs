using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

// -=-=-=- //

public class VectorierSettingsProvider : SettingsProvider
{
    public VectorierSettingsProvider(string path, SettingsScope scopes, IEnumerable<string>? keywords = null) : base(path, scopes, keywords)
    {
        // pass
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

        var gameDirectory = VectorierSettings.GameDirectory;
        MakeTextField(ref gameDirectory, "Game directory", "Game location directory / Steam URL");

        EditorGUILayout.EndVertical();
        EditorPrefs.SetString(VectorierSettings.GameDirectoryKey, gameDirectory);
    }

    private void MakeTextField(ref string? value, string textFieldlabel, string tooltip)
    {
        value ??= "";
        var guiContent = new GUIContent(textFieldlabel, tooltip);
        value = EditorGUILayout.TextField(guiContent, value);
    }

    [SettingsProvider]
    public static SettingsProvider CreateVectorierSettingsProvider()
    {
        return new VectorierSettingsProvider("Project/Vectorier", SettingsScope.Project);
    }
}
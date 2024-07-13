
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

public class VectorierSettingsProvider : SettingsProvider
{

    private const string GameExecutableToolTip = "If this field is empty, then it will use the Steam run game path (steam://rungameid/248970).";

    public VectorierSettingsProvider(string path, SettingsScope scopes, IEnumerable<string>? keywords = null) : base(path, scopes, keywords)
    {
    }



    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
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
        MakeTextField(ref gameDirectory, "Game Directory", "The directory where the game executable is located.");

        EditorGUILayout.Space(5F);

        var gameExecutablePath = VectorierSettings.GameExecutablePath;
        MakeTextField(ref gameExecutablePath, "Game Executable Path", GameExecutableToolTip);

        EditorGUILayout.EndVertical();

        EditorPrefs.SetString(VectorierSettings.GameDirectoryKey, gameDirectory);
        EditorPrefs.SetString(VectorierSettings.GameExecutablePathKey, gameExecutablePath);

    }

    private void MakeTextField(ref string? value, string textFieldlabel, string tooltip)
    {
        value ??= string.Empty;
        var guiContent = new GUIContent(textFieldlabel, tooltip);
        value = EditorGUILayout.TextField(guiContent, value);
    }

    [SettingsProvider]
    public static SettingsProvider CreateVectorierSettingsProvider()
    {
        return new VectorierSettingsProvider("Project/Vectorier", SettingsScope.Project);
    }


}

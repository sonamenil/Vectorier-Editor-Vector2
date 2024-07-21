using UnityEditor;

#nullable enable

// -=-=-=- //

public static class VectorierSettings
{
    public const string SettingsPath = "Assets/Settings/VectorierSettings.asset";

    internal const string GameDirectoryKey = "VectorierSettings.GameDirectory";
    public static string? GameDirectory => EditorPrefs.GetString(GameDirectoryKey, "");
}
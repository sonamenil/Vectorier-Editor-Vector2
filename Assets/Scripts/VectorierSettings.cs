using UnityEditor;

#nullable enable

// -=-=-=- //

public static class VectorierSettings
{
    public const string SettingsPath = "Assets/Settings/VectorierSettings.asset";

    internal const string RoomsDirectoryKey = "VectorierSettings.RoomsDirectory";

    public static string? RoomsDirectory => EditorPrefs.GetString(RoomsDirectoryKey, "");
}
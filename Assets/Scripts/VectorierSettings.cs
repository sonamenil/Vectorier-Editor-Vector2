#nullable enable
using System.IO;
using UnityEditor;
public static class VectorierSettings
{

    public const string SettingsPath = "Assets/Settings/VectorierSettings.asset";
    private const string GameExecutableName = "Vector.exe";

    internal const string GameDirectoryKey = "VectorierSettings.GameDirectory";
    internal const string GameExecutablePathKey = "VectorierSettings.GameExecutablePath";

    public static string? GameDirectory => EditorPrefs.GetString(GameDirectoryKey, "");

    public static string? GameExecutablePath => EditorPrefs.GetString(GameExecutablePathKey, "");









}

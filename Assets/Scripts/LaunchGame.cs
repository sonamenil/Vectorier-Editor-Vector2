using System.ComponentModel;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace DefaultNamespace
{
    public class LaunchGame : MonoBehaviour
    {
        private const string SteamRunGamePath = "steam://rungameid/248970";

        // This prevents the game from running after just building the map.
        private static bool shouldLaunchGame;

        static LaunchGame()
        {
            BuildMap.MapBuilt += OnMapBuilt;
        }


        private static void OnMapBuilt()
        {
            RunGame();
        }

        [MenuItem("Vectorier/Launch/Run Game %#R")]
        private static void RunGame()
        {

            if (!shouldLaunchGame)
                return;
            var gameExecutablePath = VectorierSettings.GameExecutablePath ?? SteamRunGamePath;

            try
            {
                var gameProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = gameExecutablePath
                    }
                };

                gameProcess.Start();
                gameProcess.WaitForExit();
            }
            catch (Win32Exception) // This exception is thrown when the game executable is not found.
            {
                Debug.LogError($"Cannot find game executable at path: {VectorierSettings.GameDirectory}!");
            }
        }



        [MenuItem("Vectorier/Launch/Build and Run Game %#&R")]
        public static void BuildAndRun()
        {
            shouldLaunchGame = true;
            BuildMap.Build();



        }
    }
}

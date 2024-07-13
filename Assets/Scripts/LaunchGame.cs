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


            var gameExecutablePath = VectorierSettings.GameExecutablePath ?? SteamRunGamePath;

            if (string.IsNullOrEmpty(gameExecutablePath))
            {
                Debug.LogWarning("Game executable path is not set! Please set it in the Project setting.");
                return;
            }

            try
            {
                var gameProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = gameExecutablePath
                    },
                    EnableRaisingEvents = true
                };

                gameProcess.Exited += (sender, args) =>
                {
                    Debug.Log("Game exited.");
                    shouldLaunchGame = false; // Reset flag after the game exits
                };

                gameProcess.Start();
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
            BuildMap.IsBuildForRunGame = true; // Set the flag before building
            BuildMap.Build();

        }


    }
}

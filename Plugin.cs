using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using FailBetter.Core;
using Sunless.Game.ApplicationProviders;

namespace AllShipsAllSlots;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal sealed class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();


    private const string CONFIG_FILENAME = "AllShipsAllSlots_config.ini";

    private static bool unlockForward = true;
    private static bool unlockAFT = true;

    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Logger.LogInfo("\n           _ _  _____ _     _                    _ _  _____ _       _       \n     /\\   | | |/ ____| |   (_)             /\\   | | |/ ____| |     | |      \n    /  \\  | | | (___ | |__  _ _ __  ___   /  \\  | | | (___ | | ___ | |_ ___ \n   / /\\ \\ | | |\\___ \\| '_ \\| | '_ \\/ __| / /\\ \\ | | |\\___ \\| |/ _ \\| __/ __|\n  / ____ \\| | |____) | | | | | |_) \\__ \\/ ____ \\| | |____) | | (_) | |_\\__ \\\n /_/    \\_\\_|_|_____/|_| |_|_| .__/|___/_/    \\_\\_|_|_____/|_|\\___/ \\__|___/\n                             | |                                            \n                             |_|                                            \n");
        Instance = this;

        LoadConfig(); // Load config

        // Initialize Harmony
        Harmony.CreateAndPatchAll(typeof(ShipyardProviderPatch));
    }

    private void LoadConfig(bool loadDefault = false)
    {
        string[] lines;
        if (File.Exists(CONFIG_FILENAME) && !loadDefault)
        {
            lines = File.ReadAllLines(CONFIG_FILENAME);
        }
        else
        {
            Logger.LogWarning("Config not found or corrupt, using default values.");
            string file = ReadTextResource(GetEmbeddedPath() + CONFIG_FILENAME); // Get the default config from the embedded resources
            lines = file.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries); // Split the file into lines
        }

        var optionsDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var line in lines)
            {
                if (line.Contains('=')) // Check if the line contains an '=' character so it's a valid config line
                {
                    // Remove all spaces from the line and split it at the first occurrence of '=' into two parts
                    string[] keyValue = line.Replace(" ", "").Split(['='], 2);
                    optionsDict[keyValue[0]] = keyValue[1]; // Add the key and value to the dictionary
                }
            }


            unlockForward = bool.Parse(optionsDict["unlockforward"]);
            unlockAFT = bool.Parse(optionsDict["unlockaft"]);
            Logger.LogInfo($"Config loaded - Forward slots: {unlockForward}, AFT slots: {unlockAFT}");
        }
        catch (Exception)
        {
            LoadConfig( /*loadDefault =*/ true); // Load config with default values
        }
    }

    public static string GetEmbeddedPath(string folderName = "") // Get the path of embedded resources
    {
        string projectName = Assembly.GetExecutingAssembly().GetName().Name;
        string fullPath = $"{projectName}.{folderName}";
        return fullPath;
    }

    public static string ReadTextResource(string fullResourceName)
    {
        using (Stream stream = Assembly.GetManifestResourceStream(fullResourceName))
        {
            if (stream == null)
            {
                Instance.Logger.LogWarning("Tried to get resource that doesn't exist: " + fullResourceName);
                return null; // Return null if the embedded resource doesn't exist
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd(); // Read and return the embedded resource
        }
    }

    [HarmonyPatch(typeof(ShipyardProvider))]
    private static class ShipyardProviderPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("UpdateCombatSlots")]
        private static bool PrefixStart(ShipyardProvider __instance, Quality newShip)
        {
            // Whether the ship has the slot originally
            bool hasForward = WellKnownQualityProvider.ShipsWithForwardSlots.Contains(newShip);
            bool hasAft = WellKnownQualityProvider.ShipsWithAftSlots.Contains(newShip);

            // Whether it should have it based on the config
            int shouldHaveForward = unlockForward ? 1 : 0;
            int shouldHaveAft = unlockAFT ? 1 : 0;

            __instance.CurrentCharacter.AcquireQualityAtExplicitLevel(
                WellKnownQualityProvider.Forward,
                hasForward ? 1 : shouldHaveForward);

            __instance.CurrentCharacter.AcquireQualityAtExplicitLevel(
                WellKnownQualityProvider.Aft,
                hasAft ? 1 : shouldHaveAft);

            return false; // Don't run the original function
        }
    }
}



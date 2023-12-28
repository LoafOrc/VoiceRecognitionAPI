using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Speech.Recognition;
using VoiceRecognitionAPI.Patches;

namespace VoiceRecognitionAPI {

    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin {
        public const string modGUID = "me.loaforc.voicerecognitionapi";
        public const string modName = "VoiceRecognitionAPI";
        public const string modVersion = "1.0.0";

        private static readonly Harmony harmony = new Harmony(modGUID);
        internal static Plugin instance;
        internal static ManualLogSource logger;

        internal static ConfigEntry<bool> LOG_SPEECH;
        internal static ConfigEntry<bool> LOG_IMPORT;

        internal static bool RECOGNITION_SETUP = false;

        void Awake() {
            if (instance == null) instance = this; // Signleton
            else return; // Make sure nothing else gets loaded.
            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            LOG_SPEECH = Config.Bind(
                "Logging",
                "LogRecognitionResults",
                false,
                "Does " + modName + " print out its results from voice recognition. Can be useful to see if something is wrong with voice detection."
            );

            LOG_IMPORT = Config.Bind(
                "Logging",
                "LogAssemblyResolve",
                true,
                "Does " + modName + " print out its status on importing libraries. Can be useful if " + modName + " is failing to load."
            );

            // Make sure instead of failing we load the System.Speech Library from the embedded resources.
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                if(LOG_IMPORT.Value)
                    logger.LogInfo("Importing " + args.Name);

                String resourceName = modName + "." + new AssemblyName(args.Name).Name + ".dll";
                if (LOG_IMPORT.Value)
                    logger.LogInfo("Located at: " + resourceName);

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                    if (LOG_IMPORT.Value)
                        logger.LogInfo("Found file! Length: " + stream.Length);

                    byte[] assemblyData = new byte[stream.Length];

                    stream.Read(assemblyData, 0, assemblyData.Length);

                    try {
                        Assembly loaded = Assembly.Load(assemblyData);
                        if(LOG_IMPORT.Value)
                            logger.LogInfo($"Loaded {loaded.FullName}");

                        return loaded;
                    } catch (Exception ex) {
                        logger.LogError("Failed to load assembly: " + ex.Message + "\n" + ex.ToString());
                        return null;
                    }
                }

            };

            logger.LogInfo("Patching game...");
            harmony.PatchAll(typeof(GameNetworkManagerPatch));

            logger.LogInfo(modName + ":" + modVersion + " has succesfully loaded!");
        }

        internal void SetupEngine() {
            logger.LogInfo("Setting up the recognition engine.");
            new SpeechHandler();

            RECOGNITION_SETUP = true;
        }
    }
}

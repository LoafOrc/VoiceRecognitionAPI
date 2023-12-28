using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace VoiceRecognitionAPI.Patches {
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void setupRecognitionEngine() {
            Plugin.instance.SetupEngine();
        }
    }
}

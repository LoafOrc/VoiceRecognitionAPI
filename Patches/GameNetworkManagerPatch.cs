using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace VoiceRecognitionAPI.Patches {
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch {
        [HarmonyPostfix, HarmonyPatch("Start")]
        internal static void SetupRecognitionEngine() {
            new SpeechHandler();
        }
    }
}

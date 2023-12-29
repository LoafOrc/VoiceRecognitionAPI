using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRecognitionAPI {
    public static class Voice {
        public const float DEFAULT_MIN_CONFIDENCE = .2f;

        internal static event EventHandler<VoiceRecognitionEventArgs> VoiceRecognitionFinishedEvent = (__, args) => {
            if (Plugin.LOG_SPEECH.Value)
                Plugin.logger.LogInfo("Recognized: \"" + args.Message + "\" with a confidence of " + args.Confidence);
        };

        internal static List<string> phrases = new List<string>();

        public static void ListenForPhrase(string phrase, Action<string> callback) {
            ListenForPhrase(phrase, DEFAULT_MIN_CONFIDENCE, callback);
        }

        public static void ListenForPhrase(string phrase, float minConfidence, Action<string> callback) {
            ListenForPhrases(new string[] { phrase }, minConfidence, callback);
        }

        public static void ListenForPhrases(string[] phrases, Action<string> callback) {
            ListenForPhrases(phrases, DEFAULT_MIN_CONFIDENCE, callback);
        }

        public static void ListenForPhrases(string[] phrases, float minConfidence, Action<string> callback) {
            if(Plugin.RECOGNITION_SETUP) {
                throw new VoiceRecognitionEngineAlreadyStarted("The voice recognition engine was already started. If you are a developer, Make sure to setup your voice recognition patterns in Awake().");
            }

            Plugin.logger.LogInfo(phrases);
            Voice.phrases.AddRange(phrases);
            VoiceRecognitionFinishedEvent += (__, args) => {
                if (phrases.Contains(args.Message) && args.Confidence >= minConfidence) {
                    callback.Invoke(args.Message);
                }
            };
        }

        internal static void VoiceRecognition(RecognizeCompletedEventArgs e) {
            VoiceRecognitionEventArgs args = new VoiceRecognitionEventArgs();

            args.Message = e.Result.Text; args.Confidence = e.Result.Confidence;

            try {
                VoiceRecognitionFinishedEvent.Invoke(Plugin.instance, args);
            } catch(Exception ex) {
                Plugin.logger.LogError("Something failed to do something " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public class VoiceRecognitionEventArgs : EventArgs {
            public string Message;
            public float Confidence;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;

namespace VoiceRecognitionAPI {
    public static class Voice {
        public const float DEFAULT_MIN_CONFIDENCE = .7f;
        public static bool RECOGNITION_SETUP { get; internal set; }


        internal static event EventHandler<VoiceRecognitionEventArgs> VoiceRecognitionFinishedEvent = (__, args) => {
            VoicePlugin.logger.LogDebug("Recognized: \"" + args.Message + "\" with a confidence of " + args.Confidence);
        };

        internal static List<string> phrases = new List<string>();

        public static EventHandler<VoiceRecognitionEventArgs> RegisterCustomHandler(EventHandler<VoiceRecognitionEventArgs> callback) {
            VoiceRecognitionFinishedEvent += callback;
            return callback;
        }

        public static EventHandler<VoiceRecognitionEventArgs> ListenForPhrase(string phrase, Action<string> callback, float minConfidence = DEFAULT_MIN_CONFIDENCE) {
            return ListenForPhrases(new string[] { phrase }, callback, minConfidence);
        }
        
        public static void RegisterPhrases(string[] phrases) {
            Voice.phrases.AddRange(phrases);
        }

        public static EventHandler<VoiceRecognitionEventArgs> ListenForPhrases(string[] phrases, Action<string> callback, float minConfidence = DEFAULT_MIN_CONFIDENCE) {
            if(RECOGNITION_SETUP) {
                throw new VoiceRecognitionEngineAlreadyStarted("The voice recognition engine was already started. If you are a developer, Make sure to setup your voice recognition patterns in Awake().");
            }

            RegisterPhrases(phrases);
            EventHandler<VoiceRecognitionEventArgs> recCallback = (__, args) => {
                if (phrases.Contains(args.Message) && args.Confidence >= minConfidence) {
                    callback.Invoke(args.Message!);
                }
            };
            VoiceRecognitionFinishedEvent += recCallback;
            return recCallback;
        }

        public static void StopListeningForPhrase(EventHandler<VoiceRecognitionEventArgs> callback) {
            VoiceRecognitionFinishedEvent -= callback;
        }

        internal static void VoiceRecognition(RecognizeCompletedEventArgs e) {
            VoiceRecognitionEventArgs args = new VoiceRecognitionEventArgs();

            args.Message = e.Result.Text; args.Confidence = e.Result.Confidence;

            try {
                VoiceRecognitionFinishedEvent.Invoke(VoicePlugin.instance, args);
            } catch(Exception ex) {
                VoicePlugin.logger.LogError("Something failed to do something " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public class VoiceRecognitionEventArgs : EventArgs {
            public string Message;
            public float Confidence;
        }
    }
}

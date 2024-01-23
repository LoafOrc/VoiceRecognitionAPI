using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;

namespace VoiceRecognitionAPI {
    public static class Voice {
        public const float DEFAULT_MIN_CONFIDENCE = .2f;

        internal static event EventHandler<VoiceRecognitionEventArgs> VoiceRecognitionFinishedEvent = (__, args) => {
            if (VoicePlugin.LOG_SPEECH.Value)
                VoicePlugin.logger.LogInfo("Recognized: \"" + args.Message + "\" with a confidence of " + args.Confidence);
        };

        internal static List<string> phrases = new List<string>();

        public static EventHandler<VoiceRecognitionEventArgs> CustomListenForPhrases(string[] phrases, EventHandler<VoiceRecognitionEventArgs> callback) {
            Voice.phrases.AddRange(phrases);
            EventHandler<VoiceRecognitionEventArgs> wrapped = (__, args) => {
                if (phrases.Contains(args.Message)) {
                    callback.Invoke(__, args);
                }
            };
            VoiceRecognitionFinishedEvent += wrapped;
            return wrapped;
        }

        public static EventHandler<VoiceRecognitionEventArgs> ListenForPhrase(string phrase, Action<string> callback) {
            return ListenForPhrase(phrase, DEFAULT_MIN_CONFIDENCE, callback);
        }

        public static EventHandler<VoiceRecognitionEventArgs> ListenForPhrase(string phrase, float minConfidence, Action<string> callback) {
            return ListenForPhrases(new string[] { phrase }, minConfidence, callback);
        }

        public static EventHandler<VoiceRecognitionEventArgs> ListenForPhrases(string[] phrases, Action<string> callback) {
            return ListenForPhrases(phrases, DEFAULT_MIN_CONFIDENCE, callback);
        }

        public static EventHandler<VoiceRecognitionEventArgs> ListenForPhrases(string[] phrases, float minConfidence, Action<string> callback) {
            if(VoicePlugin.RECOGNITION_SETUP) {
                throw new VoiceRecognitionEngineAlreadyStarted("The voice recognition engine was already started. If you are a developer, Make sure to setup your voice recognition patterns in Awake().");
            }

            Voice.phrases.AddRange(phrases);
            EventHandler<VoiceRecognitionEventArgs> recCallback = (__, args) => {
                if (phrases.Contains(args.Message) && args.Confidence >= minConfidence) {
                    callback.Invoke(args.Message);
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

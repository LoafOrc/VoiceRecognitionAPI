using System;
using System.Speech.Recognition;
using UnityEngine;

namespace VoiceRecognitionAPI {
    public class SpeechHandler {

        // Now what I'm about to do might seem crazy. But!, it must be done otherwise an error is thrown!
        // Basically it tries to load SpeechRecognitionEngine before the main mod has the chance to load
        // it correctly, which means without this hack the mod would not start.
        private static object recognition;

        internal static SpeechHandler instance;


        public SpeechHandler() {
            if (instance == null) instance = this;
            else return;

            recognition = new SpeechRecognitionEngine();
            SpeechRecognitionEngine casted = (SpeechRecognitionEngine)recognition;
            casted.SetInputToDefaultAudioDevice();
            
            foreach(string phrase in Voice.phrases) {
                Plugin.logger.LogInfo(phrase);
            }

            Plugin.logger.LogInfo("Phrases used for voice recognition: " + Voice.phrases);
            casted.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(Voice.phrases.ToArray()))));
            casted.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(RecognizeCompletedHandler);
            casted.RecognizeAsync();
            Plugin.logger.LogInfo("Began listenting");
        }

        void RecognizeCompletedHandler(object sender, RecognizeCompletedEventArgs e) {
            Plugin.logger.LogInfo("afh");
            ((SpeechRecognitionEngine)recognition).RecognizeAsync();
            if (e.Error != null) {
                Plugin.logger.LogError("An erroror occured during recognition: " + e.Error);
                return;
            }
            if (e.InitialSilenceTimeout || e.BabbleTimeout) {
                return;
            }
            if (e.Result != null && Plugin.LOG_SPEECH.Value) {
                Voice.VoiceRecognition(e);
            } else if (Plugin.LOG_SPEECH.Value) {
                Plugin.logger.LogInfo("No result.");
            }
        }
    }


}

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

            VoicePlugin.logger.LogInfo("Setting up the recognition engine.");

            recognition = new SpeechRecognitionEngine();
            SpeechRecognitionEngine casted = (SpeechRecognitionEngine)recognition;
            casted.SetInputToDefaultAudioDevice();
            
            foreach(string phrase in Voice.phrases) {
                VoicePlugin.logger.LogDebug("registering phrase: " + phrase);
            }

            GrammarBuilder grammarBuilder = new GrammarBuilder(new Choices(Voice.phrases.ToArray()));
            grammarBuilder.Culture = casted.RecognizerInfo.Culture;

            casted.LoadGrammar(new Grammar(grammarBuilder));
            casted.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(RecognizeCompletedHandler);
            casted.RecognizeAsync();
            VoicePlugin.logger.LogInfo("Speech Recognition Engine is Ready to Go!!");
        }

        void RecognizeCompletedHandler(object sender, RecognizeCompletedEventArgs e) {
            VoicePlugin.logger.LogDebug("Speech Engine event fired.");
            ((SpeechRecognitionEngine)recognition).RecognizeAsync();
            if (e.Error != null) {
                VoicePlugin.logger.LogError("An error occured during recognition: " + e.Error);
                return;
            }
            if (e.InitialSilenceTimeout || e.BabbleTimeout) {
                VoicePlugin.logger.LogWarning("babble timeout");
                return;
            }
            if (e.Result != null) {
                Voice.VoiceRecognition(e);
            } else {
                VoicePlugin.logger.LogDebug("No result.");
            }
        }
    }


}

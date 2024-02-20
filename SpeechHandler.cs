using System;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using UnityEngine;

namespace VoiceRecognitionAPI {
    internal class SpeechHandler {

        // Now what I'm about to do might seem crazy. But!, it must be done otherwise an error is thrown!
        // Basically it tries to load SpeechRecognitionEngine before the main mod has the chance to load
        // it correctly, which means without this hack the mod would not start.
        internal object recognition;

        public static SpeechHandler instance { get; private set; }


        internal SpeechHandler() {
            if (instance == null) instance = this;
            else return;

            if(Voice.phrases.Count == 0) {
                VoicePlugin.logger.LogWarning("this is awkward, no mods registered any voice phrases. Cancelling creating the speech recognition engine!");
                instance = null;
                return;
            }

            VoicePlugin.logger.LogInfo("Setting up the recognition engine.");

            recognition = new SpeechRecognitionEngine();
                SpeechRecognitionEngine casted = (SpeechRecognitionEngine)recognition;
            try {
                casted.SetInputToDefaultAudioDevice();
            } catch (Exception e) when(e is PlatformNotSupportedException || e is COMException) {
                VoicePlugin.logger.LogError("Failed create recognition engine. This is most likely due to your language not supporting Microsoft's speech recognition!\n" + e);

                instance = null;
                return;
            }
            
            foreach(string phrase in Voice.phrases) {
                VoicePlugin.logger.LogDebug("registering phrase: " + phrase);
            }

            GrammarBuilder grammarBuilder = new GrammarBuilder(new Choices(Voice.phrases.ToArray())) {
                Culture = casted.RecognizerInfo.Culture
            };

            VoicePlugin.logger.LogInfo("Almost done setting up..");
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
                VoicePlugin.logger.LogDebug("babble timeout");
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

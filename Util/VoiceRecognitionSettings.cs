using LethalSettings.UI;
using LethalSettings.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceRecognitionAPI.Util {
    internal class VoiceRecognitionSettings {
        internal static LabelComponent? successMessage;
        private static bool testingRecogntion = false;

        internal static void Init() {
            successMessage = new LabelComponent {
                Text = "",
                Alignment = TMPro.TextAlignmentOptions.Left,
                FontSize = 14,
            };

            ModMenu.RegisterMod(new ModMenu.ModSettingsConfig {
                Name = "VoiceRecognitionAPI",
                Id = VoicePlugin.modGUID,
                Description = "Allows you to test out voice recognition!",
                MenuComponents = new MenuComponent[] {
                    new ButtonComponent {
                        Text = "Test Voice Recognition",
                        OnClick = (self) => {
                            successMessage.Text = "Please say \"I love the company!\"";
                            testingRecogntion = true;
                        }
                    },
                    successMessage
                }
            });

            Voice.ListenForPhrase("i love the company", (message) => {
                if (testingRecogntion) {
                    testingRecogntion = false;
                    successMessage.Text = "The company thanks you! (Your voice recognition is working)";
                }
            });
        }
    }
}

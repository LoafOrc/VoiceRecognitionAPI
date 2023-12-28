using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRecognitionAPI {
    public class VoiceRecognitionEngineAlreadyStarted : Exception {
        public VoiceRecognitionEngineAlreadyStarted() : base() { }
        public VoiceRecognitionEngineAlreadyStarted(string message) : base(message) { }
    }
}

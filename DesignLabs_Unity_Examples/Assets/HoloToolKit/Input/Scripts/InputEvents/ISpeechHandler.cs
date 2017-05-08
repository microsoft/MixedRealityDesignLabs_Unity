using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to speech recognition.
    /// </summary>
    public interface ISpeechHandler : IEventSystemHandler
    {
        void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData);
    }
}
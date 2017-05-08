using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a tap.
    /// </summary>
    public class InputClickedEventData : InputEventData
    {
        /// <summary>
        /// Number of taps that triggered the event.
        /// </summary>
        public int TapCount { get; private set; }

        public InputClickedEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, int tapCount)
        {
            BaseInitialize(inputSource, sourceId);
            TapCount = tapCount;
        }
    }
}
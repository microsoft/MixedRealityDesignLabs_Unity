using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to source state changes, such as when an input source is detected or lost.
    /// </summary>
    public interface ISourceStateHandler : IEventSystemHandler
    {
        void OnSourceDetected(SourceStateEventData eventData);
        void OnSourceLost(SourceStateEventData eventData);
    }
}
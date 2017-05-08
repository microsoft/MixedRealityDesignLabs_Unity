using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to manipulation gestures.
    /// </summary>
    public interface IManipulationHandler : IEventSystemHandler
    {
        void OnManipulationStarted(ManipulationEventData eventData);
        void OnManipulationUpdated(ManipulationEventData eventData);
        void OnManipulationCompleted(ManipulationEventData eventData);
        void OnManipulationCanceled(ManipulationEventData eventData);
    }
}

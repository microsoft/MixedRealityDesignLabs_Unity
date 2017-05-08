using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to simple click input.
    /// </summary>
    public interface IInputClickHandler : IEventSystemHandler
    {
        void OnInputClicked(InputClickedEventData eventData);
    }
}
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to simple pointer-like input.
    /// </summary>
    public interface IInputHandler : IEventSystemHandler
    {
        void OnInputUp(InputEventData eventData);
        void OnInputDown(InputEventData eventData);
    }
}
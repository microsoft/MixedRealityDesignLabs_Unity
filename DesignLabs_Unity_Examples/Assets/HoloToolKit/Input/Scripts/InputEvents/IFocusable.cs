using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to focus enter/exit.
    /// </summary>
    public interface IFocusable : IEventSystemHandler
    {
        void OnFocusEnter();
        void OnFocusExit();
    }
}

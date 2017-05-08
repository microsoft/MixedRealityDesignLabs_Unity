using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that has a source id. 
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        public InputEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId)
        {
            BaseInitialize(inputSource, sourceId);
        }
    }
}
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class InputHandleCallbackFX : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        private ParticleSystem particles = null;

        private void Start()
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            particles.transform.position = GazeManager.Instance.HitPosition;
            particles.Emit(60);
        }
    }
}
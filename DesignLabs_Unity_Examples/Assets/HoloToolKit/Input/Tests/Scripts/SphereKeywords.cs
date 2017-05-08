using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    [RequireComponent(typeof(Renderer))]
    public class SphereKeywords : MonoBehaviour, ISpeechHandler
    {
        private Material cachedMaterial;

        private void Awake()
        {
            cachedMaterial = GetComponent<Renderer>().material;
        }

        public void ChangeColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    cachedMaterial.SetColor("_Color", Color.red);
                    break;
                case "blue":
                    cachedMaterial.SetColor("_Color", Color.blue);
                    break;
                case "green":
                    cachedMaterial.SetColor("_Color", Color.green);
                    break;
            }
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            ChangeColor(eventData.RecognizedText);
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }
    }
}
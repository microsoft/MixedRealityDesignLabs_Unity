using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class DisplayKeywords : MonoBehaviour
    {
        public KeywordManager keywordManager;
        public Text textPanel;

        void Start()
        {
            if (keywordManager == null || textPanel == null)
            {
                Debug.Log("Please check the variables in the Inspector on DisplayKeywords.cs on" + name + ".");
                return;
            }

            textPanel.text = "Try saying:\n";
            foreach (KeywordManager.KeywordAndResponse k in keywordManager.KeywordsAndResponses)
            {
                textPanel.text += k.Keyword + "\n";
            }
        }
    }
}
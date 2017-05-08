using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Editor Tools/Clear PlayerPrefs")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteAll();
    }
}
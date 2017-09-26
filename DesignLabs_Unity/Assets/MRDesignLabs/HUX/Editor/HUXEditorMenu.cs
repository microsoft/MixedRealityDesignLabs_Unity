//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// HUX Editor Menu Class.
/// Contains all functions used withing the HUX dropdown.  Should be broken apart if
/// this class gets overloaded and becomes unmanagable.
/// </summary>
public class HUXEditorMenu : MonoBehaviour
{
    #region HUX
    /// <summary>
    /// Path to the Hololens prefab
    /// </summary>
    protected const string HololensPath = "Assets/MRDesignLab/HUX/Prefabs/Interface/HoloLens.prefab";

    protected const string INT_LAYER = "Interaction";
    protected const string ACT_LAYER = "Activation";

    [MenuItem("HUX/Add Input Controls", false, 1)]
	public static void AddInputControls()
	{
		HUXInputEditing.RemoveInputControl("Horizontal", 2);
		HUXInputEditing.RemoveInputControl("Vertical", 2);
		HUXInputEditing.RemoveInputControl("Fire1", 2);
		HUXInputEditing.RemoveInputControl("Fire2", 2);

		for (int index = 0; index < HuxInputList.ALL_CONTROLS.Length; index++)
		{
			HUXInputEditing.AddInputControl(HuxInputList.ALL_CONTROLS[index]);
		}
	}

    /// <summary>
    /// Init scene removes the main camera if found and adds the Hololens.
    /// </summary>
    [MenuItem("HUX/Interface/HoloLens", false, 1)]
    public static void AddHololensInterface()
    {
        // Remove the Main Camera if found.
        GameObject _mainCam = GameObject.FindWithTag("MainCamera");
        if (_mainCam != null)
            Object.DestroyImmediate(_mainCam);

        // Remove old interface if it exists
        Veil _mainVeil = GameObject.FindObjectOfType<Veil>();
        if (_mainVeil != null)
            Object.DestroyImmediate(_mainVeil.gameObject);

        // Add the Hololens Prefab
        GameObject _hl = HUXEditorUtils.AddToScene(HololensPath, false);

        if (Selection.activeGameObject)
            _hl.transform.parent = Selection.activeGameObject.transform;
    }
    #endregion

    #region Collections
    [MenuItem("HUX/Create Collection", false, 2)]
    public static void CreateCollection()
    {
        GameObject _gameObject = new GameObject("Collection");
        _gameObject.AddComponent<HUX.Collections.ObjectCollection>();

        foreach (GameObject _go in Selection.gameObjects)
        {
            _go.transform.parent = _gameObject.transform;
        }
    }
    #endregion

    #region Utility
    protected const string MessageBoxPath = "Assets/MRDesignLab/HUX/Prefabs/Dialogs/MessageBox.prefab";

    [MenuItem("HUX/Create Sequencer", false, 2)]
    public static void CreateSequencer()
    {
        GameObject _gameObject = new GameObject("Sequencer");
        HUX.Utility.Sequencer _sequencer = _gameObject.AddComponent<HUX.Utility.Sequencer>();
        _sequencer.MessageBoxPrefab = AssetDatabase.LoadAssetAtPath<HUX.Dialogs.MessageBox>(MessageBoxPath);
    }

    [MenuItem("HUX/Startup Check")]
    public static void StartupCheck()
    {
        StartupChecks.ForceCheck();
        EditorWindow window = EditorWindow.GetWindow<StartupChecksWindow>(false, "Startup Check", true);
        window.minSize = new Vector2(425, 450);
    }
    #endregion

    #region Buttons
    protected const string SpriteButtonPath = "Assets/MRDesignLab/HUX/Prefabs/Buttons/SpriteButton.prefab";
    protected const string MeshButtonPath = "Assets/MRDesignLab/HUX/Prefabs/Buttons/MeshButton.prefab";
    protected const string ObjectButtonPath = "Assets/MRDesignLab/HUX/Prefabs/Buttons/ObjectButton.prefab";
    protected const string CompoundSquareButtonPath = "Assets/MRDesignLab/HUX/Prefabs/Buttons/SquareButton.prefab";
    protected const string CompoundRectangleButtonPath = "Assets/MRDesignLab/HUX/Prefabs/Buttons/RectangleButton.prefab";
    protected const string CompoundCircleButtonPath = "Assets/MRDesignLab/HUX/Prefabs/Buttons/CircleButton.prefab";

    [MenuItem("HUX/Buttons/Add Sprite Button", false, 20)]
    public static void CreateSpriteButton()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(SpriteButtonPath, typeof(GameObject));
        GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);

        if (Selection.activeGameObject)
        {
            clone.transform.position = Selection.activeGameObject.transform.position;
            clone.transform.rotation = Selection.activeGameObject.transform.rotation;
            clone.transform.parent = Selection.activeGameObject.transform;
        }
    }


    [MenuItem("HUX/Buttons/Add Mesh Button", false, 20)]
    public static void CreateMeshButton()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(MeshButtonPath, typeof(GameObject));
        GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);

        if (Selection.activeGameObject)
        {
            clone.transform.position = Selection.activeGameObject.transform.position;
            clone.transform.rotation = Selection.activeGameObject.transform.rotation;
            clone.transform.parent = Selection.activeGameObject.transform;
        }
    }


    [MenuItem("HUX/Buttons/Add Object Button", false, 20)]
    public static void CreateObjectButton()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(ObjectButtonPath, typeof(GameObject));
        GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);

        if (Selection.activeGameObject)
        {
            clone.transform.position = Selection.activeGameObject.transform.position;
            clone.transform.rotation = Selection.activeGameObject.transform.rotation;
            clone.transform.parent = Selection.activeGameObject.transform;
        }
    }

    [MenuItem("HUX/Buttons/Add Compound Button (Square)", false, 20)]
    public static void CreateCompoundSquareButton()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(CompoundSquareButtonPath, typeof(GameObject));
        GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        clone.name = clone.name;

        if (Selection.activeGameObject)
        {
            clone.transform.position = Selection.activeGameObject.transform.position;
            clone.transform.rotation = Selection.activeGameObject.transform.rotation;
            clone.transform.parent = Selection.activeGameObject.transform;
        }
    }

    [MenuItem("HUX/Buttons/Add Compound Button (Rectangle)", false, 20)]
    public static void CreateCompoundRectangleButton()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(CompoundRectangleButtonPath, typeof(GameObject));
        GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        clone.name = clone.name;

        if (Selection.activeGameObject)
        {
            clone.transform.position = Selection.activeGameObject.transform.position;
            clone.transform.rotation = Selection.activeGameObject.transform.rotation;
            clone.transform.parent = Selection.activeGameObject.transform;
        }
    }

    [MenuItem("HUX/Buttons/Add Compound Button (Circle)", false, 20)]
    public static void CreateCompoundCircleButton()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(CompoundCircleButtonPath, typeof(GameObject));
        GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        clone.name = clone.name;

        if (Selection.activeGameObject)
        {
            clone.transform.position = Selection.activeGameObject.transform.position;
            clone.transform.rotation = Selection.activeGameObject.transform.rotation;
            clone.transform.parent = Selection.activeGameObject.transform;
        }
    }

    #endregion

    #region Receivers
    [MenuItem("HUX/Receiver/Connect Receiver", false, 22)]
    public static void ConnectReceiver()
    {
        HUX.Receivers.InteractionReceiver _receiver = null;
        List<HUX.Buttons.Button> _buttons = new List<HUX.Buttons.Button>();

        foreach (GameObject _go in Selection.gameObjects)
        {
            HUX.Buttons.Button _button = _go.GetComponent<HUX.Buttons.Button>();
            if (_button != null)
                _buttons.Add(_button);

            HUX.Receivers.InteractionReceiver _newReceiver = _go.GetComponent<HUX.Receivers.InteractionReceiver>();
            if (_newReceiver != null)
            {
                if (_receiver == null)
                {
                    _receiver = _newReceiver;
                }
                else
                {
                    Debug.LogWarning("[HUX] More than one Receiver Found!! Using first Receiver Found.");
                }
            }
        }

        if (_receiver != null && _buttons.Count > 0)
        {
            foreach (HUX.Buttons.Button _button in _buttons)
            {
                _receiver.RegisterInteractible(_button.gameObject);
            }
        }
    }

    [MenuItem("HUX/Receiver/Disconnect Receiver", false, 22)]
    public static void DisconnectReceiver()
    {
        HUX.Receivers.InteractionReceiver _receiver = null;
        List<HUX.Buttons.Button> _buttons = new List<HUX.Buttons.Button>();

        foreach (GameObject _go in Selection.gameObjects)
        {
            HUX.Buttons.Button _button = _go.GetComponent<HUX.Buttons.Button>();
            if (_button != null)
                _buttons.Add(_button);

            HUX.Receivers.InteractionReceiver _newReceiver = _go.GetComponent<HUX.Receivers.InteractionReceiver>();
            if (_newReceiver != null)
            {
                if (_receiver == null)
                {
                    _receiver = _newReceiver;
                }
                else
                {
                    Debug.LogWarning("[HUX] More than one Receiver Found!! Using first Receiver Found.");
                }
            }
        }

        if (_receiver != null)
        {
            if(_buttons.Count > 0)
            {
                foreach (HUX.Buttons.Button _button in _buttons)
                {
                    _receiver.RemoveInteractible(_button.gameObject);
                }
            }
            else
            {

            }
        }
    }

    [MenuItem("HUX/Receiver/Add Toggle Receiver", false, 30)]
    public static void CreateToggleReceiver()
    {
        GameObject _gameObject = new GameObject("ToggleActiveReceiver");
        HUX.Receivers.ToggleActiveReceiver _receiver = _gameObject.AddComponent<HUX.Receivers.ToggleActiveReceiver>();

        if (Selection.activeGameObject)
        {
            HUX.Buttons.Button _button = Selection.activeGameObject.GetComponent<HUX.Buttons.Button>();
            if (_button != null)
            {
                _receiver.RegisterInteractible(_button.gameObject);
            }
        }
    }

    [MenuItem("HUX/Receiver/Add Slideshow Receiver", false, 30)]
    public static void CreateSlideshowReceiver()
    {
        GameObject _gameObject = new GameObject("SlideshowReceiver");
        HUX.Receivers.SlideshowReceiver _receiver = _gameObject.AddComponent<HUX.Receivers.SlideshowReceiver>();

        if (Selection.activeGameObject)
        {
            HUX.Buttons.Button _button = Selection.activeGameObject.GetComponent<HUX.Buttons.Button>();
            if (_button != null)
            {
                _receiver.RegisterInteractible(_button.gameObject);
            }
        }
    }

    [MenuItem("HUX/Receiver/Add Speech Reciever", false, 30)]
    public static void CreateSpeechReveiver()
    {
        if (Selection.activeGameObject != null)
        {
            Selection.activeGameObject.AddComponent<SpeechReciever>();
        }
    }

    #endregion

    #region Cursors
    protected const string SpriteCursorPath = "Assets/MRDesignLab/HUX/Prefabs/Cursors/SpriteCursor.prefab";
    protected const string MeshCursorPath = "Assets/MRDesignLab/HUX/Prefabs/Cursors/MeshCursor.prefab";
    protected const string AnimCursorPath = "Assets/MRDesignLab/HUX/Prefabs/Cursors/AnimCursor.prefab";

    [MenuItem("HUX/Cursors/Add Sprite Cursor", false, 30)]
    public static void CreateSpriteCursor()
    {
        // Cursors are not child objects
        Object prefab = AssetDatabase.LoadAssetAtPath(SpriteCursorPath, typeof(GameObject));
        GameObject clone = Instantiate(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);
    }

    [MenuItem("HUX/Cursors/Add Mesh Cursor", false, 30)]
    public static void CreateMeshCursor()
    {
        // Cursors are not child objects
        Object prefab = AssetDatabase.LoadAssetAtPath(MeshCursorPath, typeof(GameObject));
        GameObject clone = Instantiate(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);
    }

    [MenuItem("HUX/Cursors/Add Anim Cursor", false, 30)]
    public static void CreateAnimCursor()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(AnimCursorPath, typeof(GameObject));
        GameObject clone = Instantiate(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);
    }
    #endregion

    #region Spatial
    /*protected const string TagAlongSolverPath = "Assets/MRDesignLab/HUX/Prefabs/Spatial/TagAlongSolver.prefab";
    protected const string BodyLockSolverPath = "Assets/MRDesignLab/HUX/Prefabs/Spatial/BodyLockSolver.prefab";
    protected const string RadialViewSolverPath = "Assets/MRDesignLab/HUX/Prefabs/Spatial/RadialViewSolver.prefab";

    [MenuItem("HUX/Spatial/Add Tag Along Solver", false, 30)]
    public static void CreateTagAlongSolver()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(TagAlongSolverPath, typeof(GameObject));
        GameObject clone = Instantiate(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);

        foreach (GameObject _go in Selection.gameObjects)
        {
            _go.transform.parent = clone.transform;
        }
    }

    [MenuItem("HUX/Spatial/Add Body Lock Solver", false, 30)]
    public static void CreateBodyLockSolver()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(BodyLockSolverPath, typeof(GameObject));
        GameObject clone = Instantiate(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);

        foreach (GameObject _go in Selection.gameObjects)
        {
            _go.transform.parent = clone.transform;
        }
    }

    [MenuItem("HUX/Spatial/Add Radial View Solver", false, 30)]
    public static void CreateRadialViewSolver()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath(RadialViewSolverPath, typeof(GameObject));
        GameObject clone = Instantiate(prefab) as GameObject;
        clone.name = clone.name.Substring(0, clone.name.Length - 7);

        foreach (GameObject _go in Selection.gameObjects)
        {
            _go.transform.parent = clone.transform;
        }
    }*/

    #endregion
}

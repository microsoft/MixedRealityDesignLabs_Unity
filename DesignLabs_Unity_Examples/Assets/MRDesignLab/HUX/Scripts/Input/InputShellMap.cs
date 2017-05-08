//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
#if UNITY_WINRT && !UNITY_EDITOR
#define USE_WINRT
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HUX.Utility;
using System.Reflection;
using HUX;
using UnityEngine.VR;
using HUX.Focus;

/// <summary>
/// InputShellMap is responsible for reading input from input sources and feeding it into InputShell.
/// It performs the switching logic so that a particular input source can be "activated" for use and
/// take precedence over 
/// 
/// A brief overview of the flow of input:
/// Input APIs -> InputSourceBase -> InputShellMap -> InputShell -> HUX
/// 
/// </summary>
public class InputShellMap : Singleton<InputShellMap>
{

    // Store a reference to the InputShell instance for convenient access
    InputShell inputShell;

    // InputSources gathers all the input sources
    public InputSources inputSources = new InputSources();

    // Helper class to handle deciding when to switch between the types of input
    public InputSwitchLogic inputSwitchLogic = new InputSwitchLogic();

    // Input states: Handle scaling for sources -> sematics
    // Hands
    public Vector2ControlState stateHandZoom = new Vector2ControlState();

    // Mouse
    public Vector2ControlState stateMouseWheelZoom = new Vector2ControlState(new Vector2(0, 1f / 40f));

    // 6dof
    public Vector2ControlState stateSixDOFZoom = new Vector2ControlState();

    // controller
    public Vector2ControlState stateLeftJoyScroll = new Vector2ControlState(new Vector2(-0.1f, -0.1f));
    public Vector2ControlState stateLeftJoyTranslate = new Vector2ControlState(new Vector2(1f, 1f));
    public Vector2ControlState stateTrigZoom = new Vector2ControlState(new Vector2(1f, 0.01f));
    public Vector2ControlState stateRightJoyRotate = new Vector2ControlState(new Vector2(0.75f, 0.75f));
    public Vector2ControlState statePadTranslate = new Vector2ControlState();
    public Vector2ControlState statePadCardinal = new Vector2ControlState();
    public Vector2ControlState stateLeftJoyCardinal = new Vector2ControlState();

    // Targeting scroll gesture
    public Vector2ControlState stateTargetScroll = new Vector2ControlState(false);

    #region Debug controls

    // If true, the gamepad will be used to control a world cursor
    [DebugTunable]
    public bool GamepadCursor = false;

    #endregion

    /// <summary>
    /// Awake and init components
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        inputSources.Init(gameObject);
        inputSwitchLogic.Init(inputSources);

        // I don't like hooking everything up with just events.  There could be too many situations where
        // a chain of subscribers is called all from one entry point, and the order of execution isn't very
        // clear.  A polling method would reduce these situations.  The only downside is that it's somewhat
        // convenient having events, especially if the input source is updating faster than the polling rate.
        // Input sources can provide events if they don't aggregate their data.  InputShell can provide events
        // since it's convenient to use at the app level.  A downside with using polling is that dependencies
        // between sources can also get complicated and make update order unclear...
    }

    /// <summary>
    /// Additional init, ensures that InputShell has instance allocated
    /// </summary>
    void Start()
    {
        inputShell = InputShell.Instance;

        // Shell control states don't need to generate events
        inputShell.ZoomVector.GenerateButtonEvents = false;
        inputShell.ScrollVector.GenerateButtonEvents = false;
    }

    /// <summary>
    /// MonoBehavior update.  Updates the InputSources and InputSwitchLogic components,
    /// updates all input states, and then applies the input to the shell.  Finally,
    /// updates InputShell.
    /// </summary>
    void Update()
    {
        // Apply debug tunes
        inputSources.gamepad.IsEnabled = !GamepadCursor;
        inputSources.worldCursorGamepad.IsEnabled = GamepadCursor;

        // Update all InputSources (manually, so we can control update order)
        inputSources.Update();

        // Update the input switching logic
        inputSwitchLogic.Update();

        // Update input states
        UpdateStates();

        // Now apply to the shell and dev input states
        ApplyInputToShell();
        ApplyInputToDev();

        // Update shell input, synchronously.  This will allow InputShell to do any necessary logic before reporting input events
        inputShell._Update();
    }

    /// <summary>
    /// Maps input from sources and states to the shell input interface
    /// </summary>
    void ApplyInputToShell()
    {
        InputShell shell = inputShell;

        // Update select button
        bool selectButton = inputSwitchLogic.GetCurrentSelectPressed();
        shell.SelectButton.ApplyState(selectButton);

        // Update menu button
        bool menuButton = inputSwitchLogic.GetAnyMenuPressed();
        shell.MenuButton.ApplyState(menuButton);

        // Update the scroll vector
        shell.ScrollVector.AddState(inputSources.touch6D.touchState);
        if (inputSources.gamepad.IsActiveTargetingSource() && !inputSources.gamepad.JoystickDragging)
        {
            shell.ScrollVector.AddState(stateLeftJoyScroll);
        }
        shell.ScrollVector.AddState(stateTargetScroll);
        shell.ScrollVector.FinalizeState();

        // Update the zoom vector
        shell.ZoomVector.AddState(stateHandZoom);
        shell.ZoomVector.AddState(stateSixDOFZoom);
        shell.ZoomVector.AddState(inputSources.editor.dragControl);
        shell.ZoomVector.AddState(stateMouseWheelZoom);
        shell.ZoomVector.AddState(stateTrigZoom);
        shell.ZoomVector.FinalizeState();

        // Update the cardinal input vector
        shell.CardinalVector.AddState(stateLeftJoyCardinal);
        shell.CardinalVector.AddState(statePadCardinal);
        shell.CardinalVector.FinalizeState();

        // Update targeting ray
        shell.TargetOrigin = inputSwitchLogic.GetTargetOrigin();
        shell.TargetOrientation = inputSwitchLogic.GetTargetOrientation();
        shell.TargetingReady = inputSwitchLogic.GetTargetingReady();
    }

    /// <summary>
    /// Maps input from sources and states to the dev input interface
    /// </summary>
    void ApplyInputToDev()
    {
        InputDev dev = InputDev.Instance;

#if UNITY_EDITOR
        // Update virtual cam input
        dev.VirtualCamMovementToggle.ApplyState(inputSources.gamepad.xButtonPressed);

        if (dev.VirtualCamMovementToggle.pressed)
        {
            dev.VirtualCamTranslate.AddState(false, inputSources.gamepad.leftJoyVector);
        }

        dev.VirtualCamTranslate.AddState(false, GetDevCameraVector(true));
        dev.VirtualCamTranslate.FinalizeState();

        if (dev.VirtualCamMovementToggle.pressed)
        {
            dev.VirtualCamVertAndRoll.AddState(false, new Vector2(-inputSources.gamepad.trigVector.x + inputSources.gamepad.trigVector.y, 0));
        }
        dev.VirtualCamVertAndRoll.AddState(false, GetDevCameraVector(false));
        dev.VirtualCamVertAndRoll.FinalizeState();

        dev.VirtualCamRotate.AddState(stateRightJoyRotate);
        dev.VirtualCamRotate.FinalizeState();
#endif
    }

    Vector2 GetDevCameraVector(bool translate)
    {
        Vector2 vec = Vector2.zero;
        if (translate)
        {
            vec.x += (InputSourceKeyboard.GetKey(KeyCode.A) ? -1f : 0) + (InputSourceKeyboard.GetKey(KeyCode.D) ? 1f : 0);
            vec.y += (InputSourceKeyboard.GetKey(KeyCode.S) ? -1f : 0) + (InputSourceKeyboard.GetKey(KeyCode.W) ? 1f : 0);
        }
        else
        {
            vec.x += (InputSourceKeyboard.GetKey(KeyCode.E) ? 1f : 0) + (InputSourceKeyboard.GetKey(KeyCode.Q) ? -1f : 0);
        }
        return vec;
    }

    /// <summary>
    /// Updates all control states.  See InputControlState.cs.
    /// </summary>
    void UpdateStates()
    {
        // Hand zoom gesture
        stateHandZoom.ApplyState(inputSources.hands.dragControl);

        // Mouse wheel zoom
        stateMouseWheelZoom.AddState(false, new Vector2(0, inputSources.editor.mouseSource.ScrollWheelDelta));
        stateMouseWheelZoom.AddState(false, new Vector2(0, inputSources.worldCursorMouse.mouseSource.ScrollWheelDelta));
        stateMouseWheelZoom.FinalizeState();

        // SixDOF zoom gesture
        stateSixDOFZoom.ApplyDelta(false, new Vector2(0, inputSources.touch6D.dragControl.delta.y));

        // Controller input maps to scrolling, zooming, and freecam input
        stateLeftJoyScroll.ApplyDelta(false, inputSources.gamepad.leftJoyVector);
        stateLeftJoyTranslate.ApplyDelta(false, inputSources.gamepad.leftJoyVector);
        stateTrigZoom.ApplyDelta(false, new Vector2(0, -inputSources.gamepad.trigVector.x + inputSources.gamepad.trigVector.y));
        stateRightJoyRotate.ApplyDelta(false, inputSources.gamepad.rightJoyVector);
        statePadTranslate.ApplyDelta(false, inputSources.gamepad.padVector);
        statePadCardinal.ApplyDelta(false, inputSources.gamepad.padVector);

        // Joystick cardinal state
        Vector2 joyPos = Vector2.zero;
        if (inputSources.gamepadCardinal.IsActiveTargetingSource())
        {
            float mag = 0.7f;
            if (inputSources.gamepad.leftJoyVector.sqrMagnitude > mag * mag)
            {
                joyPos = inputSources.gamepad.leftJoyVector;
                // Quantize
                joyPos.x = (float)Mathf.RoundToInt(5f * joyPos.x) / 5f;
                joyPos.y = (float)Mathf.RoundToInt(5f * joyPos.y) / 5f;
            }
        }
        stateLeftJoyCardinal.ApplyDelta(false, joyPos);

        // Update drag gesture
		InputSourceBase curSource = inputSwitchLogic.CurrentTargetingSource as InputSourceBase;

		if (curSource != null && curSource.IsManipulating())
        {
            stateTargetScroll.ApplyPos(true, curSource.GetManipulationPlaneProjection());
            //Debug.Log("scroll state delta: " + stateTargetScroll.delta);
        }
        else
        {
            stateTargetScroll.ApplyPos(false, Vector2.zero);
            //Debug.Log("scroll (done) state delta: " + stateTargetScroll.delta);
        }
    }

    /// <summary>
    /// Returns true if a hold gesture can be completed.  Hold gestures cannot be completed if the user
    /// is currently interacting with something.
    /// </summary>
    public bool CanCompleteHoldGesture()
    {
		return !InputShell.Instance.IsAnyManipulating();
    }
}

/// <summary>
/// InputSwitchLogic holds a list of all targeting input sources and provides very simple logic for
/// activating and switching between inputs, and getting input from the current input.
/// InputSwitchLogic is instantiated and used by InputShellMap.
/// </summary>
[System.Serializable]
public class InputSwitchLogic
{
    // Hold pointer to InputSources for convenient access
    InputSources inputSources;

    // The current targetin source
    public ITargetingInputSource CurrentTargetingSource;

    // List of all input sources which implement ITargetingInputSource
    public List<ITargetingInputSource> TargetingSources = new List<ITargetingInputSource>();

    // The same list, but InputSourceBase pointers
    public List<InputSourceBase> TargetingSourceBases = new List<InputSourceBase>();

    // Print to debug log when sources are activated or deactivated
    public bool debugPrint = false;

    /// <summary>
    /// Initialization.  Stores all input sources from InputSources which implement ITargetingInputSource
    /// </summary>
    /// <param name="_inputSources"></param>
    public void Init(InputSources _inputSources)
    {
        inputSources = _inputSources;

        foreach (InputSourceBase source in inputSources.sources)
        {
            //if( source.GetType().IsAssignableFrom(typeof(ITargetingInputSource)))
            if (source is ITargetingInputSource)
            {
                TargetingSources.Add(source as ITargetingInputSource);
                TargetingSourceBases.Add(source);
            }
        }

        if (debugPrint)
        {
            Debug.Log("TargetingSources: " + TargetingSources.Count);
        }

        CurrentTargetingSource = inputSources.hands;
    }

    /// <summary>
    /// Returns true if the current input source has select pressed.
    /// </summary>
    public bool GetCurrentSelectPressed()
    {
        if (CurrentTargetingSource != null)
        {
            return CurrentTargetingSource.IsSelectPressed();
        }
        return false;
    }

    /// <summary>
    /// Returns true if any enabled input source has menu pressed
    /// </summary>
    public bool GetAnyMenuPressed()
    {
        foreach (InputSourceBase source in inputSources.sources)
        {
            if (source.IsEnabled)
            {
                ITargetingInputSource targetSource = source as ITargetingInputSource;
                if (targetSource != null && targetSource.IsMenuPressed())
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if the specified source is the active targeting source
    /// </summary>
    public bool IsTargetingSourceActive(InputSourceBase source)
    {
        return CurrentTargetingSource == source as ITargetingInputSource;
    }

    /// <summary>
    /// Returns the origin of the current targeting source
    /// </summary>
    public Vector3 GetTargetOrigin()
    {
        if (CurrentTargetingSource != null)
        {
            return CurrentTargetingSource.GetTargetOrigin();
        }
        return Veil.Instance.HeadTransform.position;
    }

    /// <summary>
    /// Returns the rotation of the current targeting source
    /// </summary>
    public Quaternion GetTargetOrientation()
    {
        if (CurrentTargetingSource != null)
        {
            return CurrentTargetingSource.GetTargetRotation();
        }
        return Veil.Instance.HeadTransform.rotation;
    }

    /// <summary>
    /// Returns true if the current targeting source is in the 'ready' state
    /// </summary>
    public bool GetTargetingReady()
    {
        if (CurrentTargetingSource != null)
        {
            return CurrentTargetingSource.IsReady();
        }
        return false;
    }

    /// <summary>
    /// Gets the focuser tied to the current active input source.
    /// </summary>
    /// <returns></returns>
    public AFocuser GetFocuserForCurrentTargetingSource()
    {
        AFocuser focuser = null;
        if (FocusManager.Instance != null)
        {
            for (int index = 0; index < FocusManager.Instance.Focusers.Length; index++)
            {
                InputSourceFocuser sourceFocuser = FocusManager.Instance.Focusers[index] as InputSourceFocuser;
                if (sourceFocuser != null && sourceFocuser.TargetingInputSource == CurrentTargetingSource)
                {
                    focuser = FocusManager.Instance.Focusers[index];
                    break;
                }
            }

            //If we haven't found a specific focuser use the gaze.
            if (focuser == null)
            {
                focuser = FocusManager.Instance.GazeFocuser;
            }
        }

        return focuser;
    }

    /// <summary>
    /// Checks all enabled input sources and activates a new source if requested
    /// </summary>
    public void Update()
    {
        for (int i = 0; i < TargetingSources.Count; ++i)
        {
            ITargetingInputSource targetSource = TargetingSources[i];
            if (TargetingSourceBases[i].IsEnabled)
            {
                if (targetSource != CurrentTargetingSource && targetSource.ShouldActivate())
                {
                    ActivateTargetingSource(targetSource);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Deactivates the current source and activateds the new source
    /// </summary>
    public void ActivateTargetingSource(ITargetingInputSource targetSource)
    {
        if (CurrentTargetingSource != null && CurrentTargetingSource != targetSource)
        {
            CurrentTargetingSource.OnActivate(false);
        }

        CurrentTargetingSource = targetSource;

        if (CurrentTargetingSource != null)
        {
            if (debugPrint)
            {
                Debug.Log("Activating source " + CurrentTargetingSource);
            }

            CurrentTargetingSource.OnActivate(true);
        }
    }
}

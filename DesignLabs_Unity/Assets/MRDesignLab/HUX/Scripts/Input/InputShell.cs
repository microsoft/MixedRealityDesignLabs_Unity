//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Utility;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InputShell is the output source of input for the rest of the application.
/// The shell scripts and demo should only use input recieved through InputShell,
/// so that when input methods are added or changed, they may not need to change.
/// 
/// Currently the input is abstracted to just Select and Menu buttons, Scroll and
/// Zoom 2d inputs, a Cardinal direction input, and a Targeting ray.
/// </summary>
public class InputShell : Singleton<InputShell>
{
    public event System.Action<InputSourceBase, bool> OnTargetSourceSelectChanged = null;
    public event System.Action<InputSourceBase, bool> OnTargetSourceMenuChanged = null;

    // Buttons for select, and menu
    public ButtonControlState SelectButton = new ButtonControlState();
    public ButtonControlState MenuButton = new ButtonControlState();

    // Scroll and zoom input
    public Vector2ControlState ScrollVector = new Vector2ControlState();
    public Vector2ControlState ZoomVector = new Vector2ControlState();

    // Cardinal input
    public Vector2ControlState CardinalVector = new Vector2ControlState();

    // For targeting in the shell
    public Vector3 TargetOrigin;
    public Quaternion TargetOrientation;
    public Vector3 TargetDirection
    {
        get
        {
            return TargetOrientation * Vector3.forward;
        }
    }
    // Basically show the ready cursor.  I.e. hand ready? Or replace this with a check for any active targeting source?
    public bool TargetingReady;

    // World cursor prefab
    public WorldCursor WorldCursorPrefab;

    // Reference to the world cursor
    public WorldCursor worldCursor;

    /// <summary>
    /// Initialization.  Creates the world cursor, gets the beam controller, and other things associated
    /// with input.
    /// </summary>
    void Start()
    {
        // Create world cursor instance
        if (WorldCursorPrefab != null)
        {
            worldCursor = Instantiate<WorldCursor>(WorldCursorPrefab);
        }

        List<InputSourceBase> inputSources = InputShellMap.Instance.inputSwitchLogic.TargetingSourceBases;
        for (int i = 0; i < inputSources.Count; i++)
        {
            ITargetingInputSource targetSource = inputSources[i] as ITargetingInputSource;
            if (targetSource != null)
            {
                targetSource.OnSelectChanged += DoTargetSourceSelectChanged;
                targetSource.OnMenuChanged += OnTargetSourceMenuChanged;
            }
        }
    }

    void OnDestroy()
    {
        List<InputSourceBase> inputSources = InputShellMap.Instance.inputSwitchLogic.TargetingSourceBases;
        for (int i = 0; i < inputSources.Count; i++)
        {
            ITargetingInputSource targetSource = inputSources as ITargetingInputSource;
            if (targetSource != null)
            {
                targetSource.OnSelectChanged -= DoTargetSourceSelectChanged;
                targetSource.OnMenuChanged -= OnTargetSourceMenuChanged;
            }
        }
    }

    private void DoTargetSourceSelectChanged(InputSourceBase inputSource, bool newState)
    {
        OnTargetSourceSelectChanged(inputSource, newState);
    }

    /// <summary>
    /// Manual update function, invoked by InputShellMap, after all input controls have been updated
    /// </summary>
    public void _Update()
    {
            // Fire all queued actions now that state is fully updated
            ControlStateBase.FireActions();

            // Beam activation logic
            InputSources inputSources = InputSources.Instance;
            bool beamActive = inputSources.touch6D.IsActiveTargetingSource();
            beamActive |= inputSources.hands.IsActiveTargetingSource() && ((ITargetingInputSource)inputSources.hands).IsSelectPressed();

    #if UNITY_EDITOR
            beamActive = true;
    #endif

        // beamController.SetBeamActive(beamActive);
    }

    /// <summary>
    /// Returns true if something is being updated
    /// </summary>
    public bool IsAnyManipulating() 
    {
        bool manipulating = false;

		InputSourceBase[] sources = InputShellMap.Instance.inputSources.sources;

		for (int index = 0; index < sources.Length && !manipulating; index++)
		{
			InputSourceBase source = sources[index];
			manipulating = (source != null && source.IsManipulating());
		}

		return manipulating;
    }

    /// <summary>
    /// Intersect the targeting ray with a plane centered on the specified transform
    /// </summary>
    public bool GetTargetPoint(Transform frame, out Vector3 targetPoint)
    {
        Plane p = new Plane(frame.forward, frame.position);
        return GetTargetPoint(p, out targetPoint);
    }

    /// <summary>
    /// Intersects the targeting ray with the specified plane
    /// </summary>
    public bool GetTargetPoint(Plane plane, out Vector3 targetPoint)
    {
        Ray targetingRay = new Ray(TargetOrigin, TargetDirection);

        float enter = -1f;
        if (plane.Raycast(targetingRay, out enter))
        {
            targetPoint = targetingRay.origin + targetingRay.direction * enter;
            return true;
        }
        targetPoint = Vector3.zero;
        return false;
    }
}


//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// PriorityAction wraps two actions: a first action, and a standard action.  This can
/// be used to guarantee execution of actions, when subscription order is not known.
/// 
/// Operators +/- are overridden to operate on the standard action, and the first
/// action can be accessed explicitly when desired.
/// Call the PriorityAction using Fire(), which calls the first action first, followed
/// by the standard action.
/// </summary>
public class PriorityAction
{
	// The action to be called first
	public System.Action firstAction;

	// The action called after firstAction
	System.Action action;

	/// <summary>
	/// Adds an action to the standard action
	/// </summary>
	public static PriorityAction operator +(PriorityAction pact, System.Action act)
	{
		pact.action += act;
		return pact;
	}

	/// <summary>
	/// Removes an action from the standard action
	/// </summary>
	public static PriorityAction operator -(PriorityAction pact, System.Action act)
	{
		pact.action -= act;
		return pact;
	}

	/// <summary>
	/// Call firstAction first, then the standard action
	/// </summary>
	public void Fire()
	{
		if (firstAction != null)
		{
			firstAction();
		}
		if (action != null)
		{
			action();
		}
	}
}

/// <summary>
/// ControlStateBase is the base class for input control states.  An input control state
/// is used to track the state of an input and group that with actions which can be
/// called when the input control changes.
/// </summary>
public class ControlStateBase
{
	/// <summary>
	/// A single list of actions that are queued up to all be fired after input states are updated
	/// </summary>
	static public List<PriorityAction> actions = new List<PriorityAction>();

	/// <summary>
	/// Adds an action to the end of the global queue
	/// </summary>
	protected void QueueAction(PriorityAction action)
	{
		actions.Add(action);
	}

	/// <summary>
	/// Adds an action to the end of the global queue, at the specified offset
	/// </summary>
	protected void QueueAction(PriorityAction action, int endOffset)
	{
		if (endOffset == 0)
		{
			QueueAction(action);
		}
		else
		{
			actions.Insert(actions.Count + endOffset, action);
		}
	}

	/// <summary>
	/// Fires the actions in the global queue, and empties it
	/// </summary>
	static public void FireActions()
	{
		foreach (PriorityAction action in actions)
		{
			action.Fire();
		}
		actions.Clear();
	}
}

/// <summary>
/// ButtonControlState represents a boolean button state.  It stores the current state,
/// the previous state, and an action for OnChanged.  It also has accessors for Changed,
/// PressedOnce, and ReleasedOnce.
/// </summary>
[System.Serializable]
public class ButtonControlState : ControlStateBase
{
	// Is pressed
	public bool pressed;

	// Was pressed
	public bool prevPressed;

	// Action called when pressed changes
	public PriorityAction OnChanged = new PriorityAction();
	//public PriorityAction OnPressed = new PriorityAction();
	//public PriorityAction OnReleased = new PriorityAction();

	/// <summary>
	/// ApplyState updates the pressed state and calls OnChanged if pressed changes
	/// </summary>
	/// <param name="_pressed"></param>
	public void ApplyState(bool _pressed)
	{
		prevPressed = pressed;
		bool changed = pressed != _pressed;
		pressed = _pressed;
		if (changed)
		{
			QueueAction(OnChanged);
			//if (pressed)
			//{
			//	QueueAction(OnPressed);
			//}
			//else
			//{
			//	QueueAction(OnReleased);
			//}
		}
	}

	// Changed returns true if the state is different from the previous state
	public bool Changed
	{
		get
		{
			return pressed != prevPressed;
		}
	}

	// PressedOnce returns true for one update if pressed
	public bool PressedOnce
	{
		get
		{
			return pressed && Changed;
		}
	}

	// PressedOnce returns true for one update when released
	public bool ReleasedOnce
	{
		get
		{
			return !pressed && Changed;
		}
	}
}

/// <summary>
/// Vector2ControlState represents a 2-axis input, such as a joystick or touchpad.  Each axis is treated
/// separately, so this can be used to combine two single axis inputs as well, such as triggers.
/// Position and delta are tracked, button events can automatically be generated, input scale is
/// configurable, and events for OnMove and OnChanged.
/// </summary>
[System.Serializable]
public class Vector2ControlState : ControlStateBase
{
	public Vector2 pos;     // Current position
	public Vector2 delta;   // Position delta

	// If the input has no pressed/released events, the events can be generated based on the 2d input value
	public bool GenerateButtonEvents = true;
	
	// Wait this long before setting the button state to released
	public float GenerateButtonReleaseDelay = 0.25f;

	// Scale the incoming input by this
	public Vector2 InputScale = Vector2.one;

	// A button state is also associated with this control, for 
	public ButtonControlState buttonState = new ButtonControlState();

	// When automatically generating events, a timer can be used for generating the release event
	float releaseTimer;

	// Keep track of the pre-scaled position for delta calculation
	Vector2 preScaledPos;

	// Track the state when multiple inputs are combined during one update
	Vector2 tempDelta;
	bool tempPressed;

	// Called when there is a non-zero delta (since it should be applied)
	public PriorityAction OnMove = new PriorityAction();
	
	// Called when the delta changes at all
	public PriorityAction OnChanged = new PriorityAction();

	/// <summary>
	/// Constructor.  If generateEvents is true, button events will be called based on 2d input state.
	/// </summary>
	public Vector2ControlState(bool generateEvents = true)
	{
		GenerateButtonEvents = generateEvents;
	}

	/// <summary>
	/// Constructor, with configurable scale parameter
	/// </summary>
	public Vector2ControlState(Vector2 scale, bool generateEvents = true)
	{
		InputScale = scale;
		GenerateButtonEvents = generateEvents;
	}

	// --------------------------------------------------------------------------------------------------------
	// We can use this class to:
	// 1) Track position or delta
	// 2) Represent an output control with begin/update/end events that automatically fire
	// * Multiple input delta can be aggregated over a frame, but not position (positions must be selected,
	//   or convert themselves and be applied as deltas)

	// Functions for #1:

	/// <summary>
	/// Finalizes the control with input delta (position increments), calls events
	/// </summary>
	public void ApplyDelta(bool pressed, Vector2 newDelta)
	{
		FinalizeState(pressed, newDelta);
	}

	/// <summary>
	/// Finalizes the control with input position (delta set to difference from last pos), calls events
	/// </summary>
	public void ApplyPos(bool pressed, Vector2 newPos)
	{
		Vector2 del = newPos - preScaledPos;
		preScaledPos = newPos;

		// We want to use state tracking to get position deltas, so ignore the onset delta here
		if (pressed && !buttonState.pressed)
		{
			del = Vector2.zero;
		}

		FinalizeState(pressed, del);
	}

	/// <summary>
	/// Finalizes the control with another control state (applying the state's delta as delta), calls events
	/// </summary>
	public void ApplyState(Vector2ControlState state)
	{
		FinalizeState(state.buttonState.pressed, state.delta);
	}

	// Functions for #2:

	/// <summary>
	/// Adds delta to tracked state
	/// </summary>
	public void AddState(bool pressed, Vector2 addDelta)
	{
		tempPressed |= pressed;
		tempDelta += addDelta;
	}

	/// <summary>
	/// Adds state to tracked state
	/// </summary>
	/// <param name="state"></param>
	public void AddState(Vector2ControlState state)
	{
		AddState(state.buttonState.pressed, state.delta);
	}

	/// <summary>
	/// FinalizeState will apply and reset the tracked state, calling events
	/// </summary>
	public void FinalizeState()
	{
		FinalizeState(tempPressed, tempDelta);
		tempDelta = Vector2.zero;
		tempPressed = false;
	}

	/// <summary>
	/// FinalizeState will apply the input delta and call events
	/// </summary>
	public void FinalizeState(bool newPressed, Vector2 newDelta)
	{
		// Convert properly to handle different input/output delta vs position cases
		newDelta = Vector2.Scale(newDelta, InputScale);

		bool changed = delta != newDelta;

		// Apply the new delta
		delta = newDelta;
		pos += delta;

		bool notZero = delta != Vector2.zero;
		bool isMoving = notZero;

		// Do we need to generate pressed event?
		if (GenerateButtonEvents)
		{
			newPressed = isMoving;

			// Do the release timer.  If currently moving, keep the timer ready to go
			if (isMoving)
			{
				releaseTimer = GenerateButtonReleaseDelay;
			}
			else if (buttonState.pressed)
			{
				// If not moving, but the button state is still pressed, update the timer.
				releaseTimer -= Time.deltaTime;

				// If GenerateButtonReleaseDelay hasn't elapsed yet, artificially set pressed to true
				if (releaseTimer > 0)
				{
					newPressed = true;
				}
			}
		}

		// Is this the right place to do this?
		if (!newPressed)
		{
			delta = Vector2.zero;
			pos = Vector2.zero;
			// Do not reset preScaledPos!  If it's being used to generate button events, then it's going to re-fire!
			//preScaledPos = Vector2.zero;
			isMoving = false;
		}

		bool wasPressed = buttonState.pressed;

		// Make sure vec is up to date when event fires
		buttonState.ApplyState(newPressed);

		// Fire move event if the value isn't 0,0, since it's a delta
		// Actually, don't fire the move event if it was released
		if (isMoving /*&& pressed*/)
		{
			// Since we want the end event to happen after the last update, we can insert it just in front if pressed is false
			QueueAction(OnMove, (!newPressed && (newPressed != wasPressed)) ? -1 : 0);
		}

		// Fire event if it changed
		if (changed)
		{
			QueueAction(OnChanged, (!newPressed && (newPressed != wasPressed)) ? -1 : 0);
		}

	}
}

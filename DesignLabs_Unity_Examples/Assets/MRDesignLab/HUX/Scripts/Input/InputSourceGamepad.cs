//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX;
using System;

public class InputSourceGamepad : InputSourceVirtual, ITargetingInputSource
{
	public event Action<InputSourceBase, bool> OnSelectChanged = delegate { };
	public event Action<InputSourceBase, bool> OnMenuChanged = delegate { };

	public event Action<InputSourceGamepad, bool> OnStartButtonChanged = delegate { };
	public event Action<InputSourceGamepad, bool> OnAButtonChanged = delegate { };
	public event Action<InputSourceGamepad, bool> OnBButtonChanged = delegate { };
	public event Action<InputSourceGamepad, bool> OnXButtonChanged = delegate { };
	public event Action<InputSourceGamepad, bool> OnYButtonChanged = delegate { };

	public event Action<InputSourceGamepad, InputSourceGamepadBase> OnGamepadSourceChanged = delegate { };

	public InputSourceGamepadBase gamepadSource;

	[DebugTunable]
	public bool JoystickDragging = true;
	[DebugTunable]
	public bool AbsoluteJoystickDrag = false;
	[DebugTunable]
	public Vector2 dragTargetRangeDegrees = new Vector2(15f, -15f);
	[DebugTunable]
	public Vector2 dragSpeed = new Vector2(15f, -15f);

	bool useJoyAdjustedDir;
	Quaternion dragStartHeadRotation = Quaternion.identity;
	Quaternion adjustedTargetRot = Quaternion.identity;

	private bool m_Select;
	private bool m_Menu;

	/*public ButtonControlState startButtonState = new ButtonControlState();
	public ButtonControlState aButtonState = new ButtonControlState();
	public ButtonControlState bButtonState = new ButtonControlState();
	public ButtonControlState xButtonState = new ButtonControlState();
	public ButtonControlState yButtonState = new ButtonControlState();*/

	void Awake()
	{
		gamepadSource = gameObject.AddComponent<InputSourceGamepadBase>();
	}

	public bool startButtonPressed
	{
		get
		{
			return gamepadSource.startButtonState.pressed;
		}
	}
	public bool aButtonPressed
	{
		get
		{
			return gamepadSource.aButtonState.pressed;
		}
	}
	public bool bButtonPressed
	{
		get
		{
			return gamepadSource.bButtonState.pressed;
		}
	}
	public bool xButtonPressed
	{
		get
		{
			return gamepadSource.xButtonState.pressed;
		}
	}
	public bool yButtonPressed
	{
		get
		{
			return gamepadSource.yButtonState.pressed;
		}
	}
	public Vector2 leftJoyVector
	{
		get
		{
			return gamepadSource.leftJoyVector;
		}
	}
	public Vector2 rightJoyVector
	{
		get
		{
			return gamepadSource.rightJoyVector;
		}
	}
	public Vector2 trigVector
	{
		get
		{
			return gamepadSource.trigVector;
		}
	}
	public Vector2 padVector
	{
		get
		{
			return gamepadSource.padVector;
		}
	}

	void SelectGamepadSource(InputSourceGamepadBase source)
	{
		if (gamepadSource != null)
		{
			gamepadSource.startButtonState.OnChanged -= OnStartEvent;
			gamepadSource.aButtonState.OnChanged -= OnSelectEvent;
			gamepadSource.bButtonState.OnChanged -= OnBButtonEvent;
			gamepadSource.xButtonState.OnChanged -= OnXButtonEvent;
			gamepadSource.yButtonState.OnChanged -= OnYButtonEvent;
		}

		gamepadSource = source;

		if (gamepadSource != null)
		{
			gamepadSource.startButtonState.OnChanged += OnStartEvent;
			gamepadSource.aButtonState.OnChanged += OnSelectEvent;
			gamepadSource.bButtonState.OnChanged += OnBButtonEvent;
			gamepadSource.xButtonState.OnChanged += OnXButtonEvent;
			gamepadSource.yButtonState.OnChanged += OnYButtonEvent;
		}

		OnGamepadSourceChanged(this, gamepadSource);
	}

	private void OnStartEvent()
	{
		OnStartButtonChanged(this, gamepadSource.startButtonState.pressed);
	}

	void OnSelectEvent()
	{
		bool pressed = gamepadSource.aButtonState.pressed;
		this.OnAButtonChanged(this, pressed);
		if (pressed)
		{
			if (JoystickDragging)
			{
				dragStartHeadRotation = Veil.Instance.HeadTransform.rotation;
				adjustedTargetRot = AbsoluteJoystickDrag ? Quaternion.identity : dragStartHeadRotation;

				useJoyAdjustedDir = true;
			}
		}
		else
		{
			useJoyAdjustedDir = false;
		}
	}

	private void OnBButtonEvent()
	{
		this.OnBButtonChanged(this, gamepadSource.bButtonState.pressed);
	}

	private void OnXButtonEvent()
	{
		this.OnXButtonChanged(this, gamepadSource.xButtonState.pressed);
	}

	private void OnYButtonEvent()
	{
		this.OnYButtonChanged(this, gamepadSource.yButtonState.pressed);
	}

	public override void _Update()
	{
		// Select whichever input is present first
		if (!gamepadSource.IsPresent())
		{
			InputSources inputSources = InputSources.Instance;
			if (inputSources.netGamepad.IsPresent())
			{
				SelectGamepadSource(inputSources.netGamepad);
			}
			else if (inputSources.hidGamepad.IsPresent())
			{
				SelectGamepadSource(inputSources.hidGamepad);
			}
			else if (inputSources.unityGamepad.IsPresent())
			{
				SelectGamepadSource(inputSources.unityGamepad);
			}
		}
		else
		{
			if (useJoyAdjustedDir)
			{
				Vector2 joy = gamepadSource.leftJoyVector;
				if (AbsoluteJoystickDrag)
				{
					adjustedTargetRot = buildWorldTargetRot(joy);
				}
				else
				{
					adjustedTargetRot *= buildLocalTargetVector(joy, dragSpeed * Time.deltaTime);
				}
			}

			/*aButtonState.ApplyState(gamepadSource.aButtonState.pressed);
			bButtonState.ApplyState(gamepadSource.bButtonState.pressed);
			xButtonState.ApplyState(gamepadSource.xButtonState.pressed);
			yButtonState.ApplyState(gamepadSource.yButtonState.pressed);
			startButtonState.ApplyState(gamepadSource.startButtonState.pressed);*/
		}

		bool prev = m_Select;
		m_Select = (this as ITargetingInputSource).IsSelectPressed();
		if (prev != m_Select)
		{
			OnSelectChanged(this, m_Select);
		}

		prev = m_Menu;
		m_Menu = (this as ITargetingInputSource).IsMenuPressed();
		if (prev != m_Menu)
		{
			OnMenuChanged(this, m_Menu);
		}

		base._Update();
	}

	Quaternion buildLocalTargetVector(Vector2 input, Vector2 scale)
	{
		return Quaternion.Euler(input.y * scale.y, input.x * scale.x, 0);
	}

	Quaternion buildWorldTargetRot(Vector2 nav)
	{
		return dragStartHeadRotation * buildLocalTargetVector(nav, dragTargetRangeDegrees);
	}

	// Targeting source interface
	bool ITargetingInputSource.ShouldActivate()
	{
		return aButtonPressed;// && InputSources.Instance.gamepadCardinal.CanDeactivate();
	}
	Vector3 ITargetingInputSource.GetTargetOrigin()
	{
		return Veil.Instance.HeadTransform.position;
	}
	Quaternion ITargetingInputSource.GetTargetRotation()
	{
		if (useJoyAdjustedDir)
		{
			return adjustedTargetRot;
		}

		return Veil.Instance.HeadTransform.rotation;
	}

	bool ITargetingInputSource.IsSelectPressed()
	{
		return aButtonPressed;
	}

	bool ITargetingInputSource.IsMenuPressed()
	{
		return startButtonPressed;
	}

	void ITargetingInputSource.OnActivate(bool activated)
	{
	}

	bool ITargetingInputSource.IsReady()
	{
		return true;
	}

	bool ITargetingInputSource.IsTargetingActive()
	{
		return true;
	}
}

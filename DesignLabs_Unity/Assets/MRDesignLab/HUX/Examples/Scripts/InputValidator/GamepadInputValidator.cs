//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;

namespace HUX.InputValidator
{
	public class GamepadInputValidator : AInputValidator
	{
		#region Editor Variables
		[SerializeField]
		private TextItem m_SourceName;

		[SerializeField]
		private TextItem m_DPadHorizontal;

		[SerializeField]
		private TextItem m_DPadVertical;

		[SerializeField]
		private TextItem m_LeftTrigger;

		[SerializeField]
		private TextItem m_RightTrigger;

		[SerializeField]
		private TextItem m_LeftStickX;

		[SerializeField]
		private TextItem m_LeftStickY;

		[SerializeField]
		private TextItem m_RightStickX;

		[SerializeField]
		private TextItem m_RightStickY;

		#endregion

		//---------------------------------------------------------------------
		#region Variables

		private Vector2 m_PrevLeftJoy;
		private Vector2 m_PrevRightJoy;
		private Vector2 m_PrevTriggers;
		private Vector2 m_PrevDPad;

		#endregion

		//---------------------------------------------------------------------

		#region AInputValidator Functions
		protected override void AddInputListeners()
		{
			InputSources.Instance.gamepad.OnStartButtonChanged += OnStartButtonChanged;
			InputSources.Instance.gamepad.OnAButtonChanged += OnAButtonChanged;
			InputSources.Instance.gamepad.OnBButtonChanged += OnBButtonChanged;
			InputSources.Instance.gamepad.OnXButtonChanged += OnXButtonChanged;
			InputSources.Instance.gamepad.OnYButtonChanged += OnYButtonChanged;
			InputSources.Instance.gamepad.OnGamepadSourceChanged += OnSourceChanged;
		}

		protected override void RemoveInputListeners()
		{
			InputSources.Instance.gamepad.OnStartButtonChanged -= OnStartButtonChanged;
			InputSources.Instance.gamepad.OnAButtonChanged -= OnAButtonChanged;
			InputSources.Instance.gamepad.OnBButtonChanged -= OnBButtonChanged;
			InputSources.Instance.gamepad.OnXButtonChanged -= OnXButtonChanged;
			InputSources.Instance.gamepad.OnYButtonChanged -= OnYButtonChanged;
			InputSources.Instance.gamepad.OnGamepadSourceChanged -= OnSourceChanged;
		}
		#endregion

		//---------------------------------------------------------------------

		#region Unity Methods
		protected void Update()
		{
			Vector2 leftJoy = InputSources.Instance.gamepad.leftJoyVector;
			Vector2 rightJoy = InputSources.Instance.gamepad.rightJoyVector;
			Vector2 triggers = InputSources.Instance.gamepad.trigVector;
			Vector2 dPad = InputSources.Instance.gamepad.padVector;

			if (m_PrevLeftJoy.x != leftJoy.x)
			{
				SetAxisMessage(m_LeftStickX, "LStickX", leftJoy.x);
			}

			if (m_PrevLeftJoy.y != leftJoy.y)
			{
				SetAxisMessage(m_LeftStickY, "LStickY", leftJoy.y);
			}

			if (m_PrevRightJoy.x != rightJoy.x)
			{
				SetAxisMessage(m_RightStickX, "RStickX", rightJoy.x);
			}

			if (m_PrevRightJoy.y != rightJoy.y)
			{
				SetAxisMessage(m_RightStickY, "RStickY", rightJoy.y);
			}

			if (m_PrevTriggers.x != triggers.x)
			{
				SetAxisMessage(m_LeftTrigger, "LTrigger", triggers.x);
			}

			if (m_PrevTriggers.y != triggers.y)
			{
				SetAxisMessage(m_RightTrigger, "RTrigger", triggers.y);
			}

			if (m_PrevDPad.x != dPad.x)
			{
				SetAxisMessage(m_DPadHorizontal, "DPadHoriz", dPad.x);
			}

			if (m_PrevDPad.y != dPad.y)
			{
				SetAxisMessage(m_DPadVertical, "DPadVert", dPad.y);
			}

			m_PrevLeftJoy = leftJoy;
			m_PrevRightJoy = rightJoy;
			m_PrevTriggers = triggers;
			m_PrevDPad = dPad;
		}
		#endregion

		//--------------------------------------------------------------------
		private void SetAxisMessage(TextItem textItem, string axisName, float axisValue)
		{
			if (textItem != null)
			{
				textItem.SetText(axisName + ": " + axisValue.ToString("F3"));
				textItem.Highlight();
			}
		}

		//---------------------------------------------------------------------

		#region Listeners
		private void OnYButtonChanged(InputSourceGamepad arg1, bool newState)
		{
			string state = newState ? "Pressed" : "Released";
			AddMessage(string.Format("Y Button {0}", state));
		}

		private void OnXButtonChanged(InputSourceGamepad arg1, bool newState)
		{
			string state = newState ? "Pressed" : "Released";
			AddMessage(string.Format("X Button {0}", state));
		}

		private void OnBButtonChanged(InputSourceGamepad arg1, bool newState)
		{
			string state = newState ? "Pressed" : "Released";
			AddMessage(string.Format("B Button {0}", state));
		}

		private void OnAButtonChanged(InputSourceGamepad arg1, bool newState)
		{
			string state = newState ? "Pressed" : "Released";
			AddMessage(string.Format("A Button {0}", state));
		}

		private void OnStartButtonChanged(InputSourceGamepad arg1, bool newState)
		{
			string state = newState ? "Pressed" : "Released";
			AddMessage(string.Format("Start Button {0}", state));
		}

		private void OnSourceChanged(InputSourceGamepad arg1, InputSourceGamepadBase newSource)
		{
			if (newSource != null)
			{
				m_SourceName.SetText(newSource.GetType().ToString());
			}
			else
			{
				m_SourceName.SetText("No Gamepad Source");
			}

			m_SourceName.Highlight();
		}

		#endregion
	}
}

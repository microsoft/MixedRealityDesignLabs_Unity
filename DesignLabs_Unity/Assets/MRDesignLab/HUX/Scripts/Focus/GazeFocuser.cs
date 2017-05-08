//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Focus
{
	public class GazeFocuser : AFocuser
	{
		#region Private Variables

		private Vector3 m_GazeOrigin;
		private Vector3 m_GazeDirection;
		private Quaternion m_GazeRotation;
		private HeadGazeStabilizer m_GazeStabilizer;

		#endregion

		public override bool CanInteract
		{
			get
			{
				return false;
			}
		}

		public override bool IsInteractionReady
		{
			get
			{
				return false;
			}
		}

		public override bool IsSelectPressed
		{
			get
			{
				return false;
			}
		}

		public override Vector3 TargetDirection
		{
			get
			{
				return m_GazeDirection;
			}
		}

		public override Quaternion TargetOrientation
		{
			get
			{
				return m_GazeRotation;
			}
		}

		public override Vector3 TargetOrigin
		{
			get
			{
				return m_GazeOrigin;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			if (Camera.main != null)
			{
				m_GazeOrigin = Camera.main.transform.position;
				m_GazeDirection = Camera.main.transform.forward;
				m_GazeRotation = Camera.main.transform.rotation;
			}

			m_GazeStabilizer = this.GetComponent<HeadGazeStabilizer>();
		}

		protected override void OnPreRaycast()
		{
			if (Camera.main != null)
			{
				m_GazeOrigin = Camera.main.transform.position;
				m_GazeDirection = Camera.main.transform.forward;
				m_GazeRotation = Camera.main.transform.rotation;

				if (m_GazeStabilizer != null)
				{
					m_GazeStabilizer.UpdateHeadStability(m_GazeOrigin, Camera.main.transform.rotation);
					m_GazeOrigin = m_GazeStabilizer.StableHeadPosition;
				}
			}
		}
	}
}

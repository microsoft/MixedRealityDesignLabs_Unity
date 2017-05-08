//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections.Generic;

#if !UNITY_EDITOR && UNITY_WSA
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif

//#if UNITY_EDITOR || UNITY_WSA
using UnityEngine;
//#endif

namespace HidControllerInput
{
//#if !UNITY_EDITOR && !UNITY_WSA
//	public struct Vector2
//	{
//		public float x, y;

//		public Vector2(float _x = 0, float _y = 0)
//		{
//			x = _x;
//			y = _y;
//		}

//		public Vector2 normalized
//		{
//			get
//			{
//				float len = (float)Math.Sqrt((float)(x * x + y * y));
//				if (len > 0)
//				{
//					return new Vector2(x / len, y / len);
//				}
//				return Vector2.zero;
//			}
//		}
//		public static Vector2 zero = new Vector2(0, 0);
//	}
//#endif

	public class DeviceState
	{
		public string DeviceId;

#if !UNITY_EDITOR && UNITY_WSA
		public HidDevice Device;
#endif

        public int updateCount;

		public enum EGamepadType
		{
			None,
			X360_USB,
			PS4DS4_BT,
			EIGP20_BT
		}
		public EGamepadType gamepadType = EGamepadType.None;

		public Vector2 LeftStick, RightStick, Triggers, DPad;
		public int DPadNum;

		public bool A, B, X, Y;
		public bool leftShoulder, rightShoulder, leftTrigger, rightTrigger;
		public bool start, back, logo, leftStickClick, rightStickClick;

		// PS controller
		public bool BigPad;

		public static int GetControllerCount()
		{
#if !UNITY_EDITOR && UNITY_WSA
			lock (DeviceConnect.states)
			{
				return DeviceConnect.states.Count;
			}
#else
            return 0;
#endif
		}

		public static DeviceState GetState(int index)
		{
			DeviceState retState = new DeviceState();

#if !UNITY_EDITOR && UNITY_WSA
			lock (DeviceConnect.states)
			{
				if (index >= 0 && index < DeviceConnect.states.Count)
				{
					lock(DeviceConnect.states[index])
					{
						retState.AssignFrom(DeviceConnect.states[index], false);
					}
				}
			}
#endif

            return retState;
		}

		public void AssignFrom(DeviceState st, bool controlsOnly)
		{
			LeftStick = st.LeftStick;
			RightStick = st.RightStick;
			Triggers = st.Triggers;
			DPad = st.DPad;
			DPadNum = st.DPadNum;

			A = st.A;
			B = st.B;
			X = st.X;
			Y = st.Y;
			leftShoulder = st.leftShoulder;
			rightShoulder = st.rightShoulder;
			leftTrigger = st.leftTrigger;
			rightTrigger = st.rightTrigger;
			start = st.start;
			back = st.back;
			logo = st.logo;
			leftStickClick = st.leftStickClick;
			rightStickClick = st.rightStickClick;
			BigPad = st.BigPad;

			if (!controlsOnly)
			{
				gamepadType = st.gamepadType;
				updateCount = st.updateCount;

				//DeviceId = st.DeviceId;
				//Device = st.Device;
			}
		}

		public static int[] GetNumericControls(EGamepadType type)
		{
			switch (type)
			{
				case EGamepadType.PS4DS4_BT:
					return new int[] { 48, 49, 50, 51, 52, 53, 57 };
				case EGamepadType.X360_USB:
					return new int[] { 48, 49, 50, 51, 52, 57 };
				case EGamepadType.EIGP20_BT:
					return new int[] { 48, 49, 51, 52, 57 };
				default:
					break;
			}
			return new int[0];
		}

		public static EGamepadType GetGamepadType(int vid, int pid)
		{
			if (vid == 0x54c && pid == 0x5c4)
			{
				return EGamepadType.PS4DS4_BT;
			}
			else if (vid == 0x45e && pid == 0x028e)
			{
				return EGamepadType.X360_USB;
			}
			else if (vid == 0x04e8 && pid == 0xa000)
			{
				return EGamepadType.EIGP20_BT;
			}
			return EGamepadType.None;
		}

		float processJoy(float val, float range)
		{
			return val / (0.5f * range) - 1f;
		}
		float processTrigger(float val, float range)
		{
			return val / range;
		}

		// Converts dpad num (like 8 hours on a clock) to vec2 dir.  Offset is from 0 = noon = straight up
		public static Vector2 dirNumToVec(ref int dir, int offset, int zero)
		{
			if (dir == zero)
			{
				return Vector2.zero;
			}

			dir = (dir - offset) % 8;

			Vector2 v = Vector2.zero;
			if (dir > 0 && dir < 4)
			{
				v.x += 1f;
			}
			else if (dir > 4)
			{
				v.x -= 1f;
			}

			if (dir > 6 || dir < 2)
			{
				v.y += 1f;
			}
			else if (dir > 2 && dir < 6)
			{
				v.y -= 1f;
			}

			return v.normalized;
		}

		public void SetDPadNum(EGamepadType type, int dpadNum)
		{
			DPadNum = dpadNum;

			switch (type)
			{
				case EGamepadType.PS4DS4_BT:
					DPad = dirNumToVec(ref DPadNum, 0, 8);
					break;
				case EGamepadType.X360_USB:
					DPad = dirNumToVec(ref DPadNum, 1, 0);
					break;
				case EGamepadType.EIGP20_BT:
					DPad = dirNumToVec(ref DPadNum, -2, 0);
					break;
				default:
					break;
			}
		}

		public void ReportControl(int id, float state)
		{
			if (gamepadType == EGamepadType.PS4DS4_BT)
			{
				float range = 255f;
				switch (id)
				{
					case 5: X = true; break;    // Square
					case 6: A = true; break;    // X
					case 7: B = true; break;    // O
					case 8: Y = true; break;    // Triangle
					case 9: leftShoulder = true; break;
					case 10: rightShoulder = true; break;
					case 11: leftTrigger = true; break;
					case 12: rightTrigger = true; break;
					case 13: back = true; break;    // Share
					case 14: start = true; break;   // Options
					case 15: leftStickClick = true; break;
					case 16: rightStickClick = true; break;
					case 17: logo = true; break;
					case 18: BigPad = true; break;
					case 48: LeftStick.x = processJoy(state, range); break;
					case 49: LeftStick.y = processJoy(state, range); break;
					case 50: RightStick.x = processJoy(state, range); break;
					case 51: Triggers.x = processTrigger(state, range); break;
					case 52: Triggers.y = processTrigger(state, range); break;
					case 53: RightStick.y = processJoy(state, range); break;
					case 57:
						SetDPadNum(gamepadType, (int)state);
						break;
					default:
						break;
				}
			}
			else if (gamepadType == EGamepadType.X360_USB)
			{
				float range = 65535f;
				switch (id)
				{
					case 5: A = true; break;
					case 6: B = true; break;
					case 7: X = true; break;
					case 8: Y = true; break;
					case 9: leftShoulder = true; break;
					case 10: rightShoulder = true; break;
					case 11: back = true; break;
					case 12: start = true; break;
					case 13: leftStickClick = true; break;
					case 14: rightStickClick = true; break;
					case 48: LeftStick.x = processJoy(state, range); break;
					case 49: LeftStick.y = processJoy(state, range); break;
					case 50:
						{
							// Still need to find axes where triggers are separate
							float halfRange = range / 2f;
							Triggers.x = Math.Max(0, state - (halfRange + 0.5f)) / (halfRange - 127.5f);
							Triggers.y = Math.Max(0, -(state - halfRange)) / (halfRange - 128f);
						}
						break;
					case 51: RightStick.x = processJoy(state, range); break;
					case 52: RightStick.y = processJoy(state, range); break;
					case 57:
						SetDPadNum(gamepadType, (int)state);
						break;
					default:
						break;
				}
			}
			else if (gamepadType == EGamepadType.EIGP20_BT)
			{
				float range = 255f;
				switch (id)
				{
					case 5: A = true; break;    // 1
					case 6: B = true; break;    // 2
					case 8: X = true; break;    // 3
					case 9: Y = true; break;    // 4
					case 11: leftShoulder = true; break;
					case 12: rightShoulder = true; break;
					case 15: back = true; break;
					case 16: start = true; break;
					case 20: logo = true; break;

					case 48: LeftStick.x = processJoy(state, range); break;
					case 49: LeftStick.y = processJoy(state, range); break;
					case 51: RightStick.x = processJoy(state, range); break;
					case 52: RightStick.y = processJoy(state, range); break;
					case 57:
						SetDPadNum(gamepadType, (int)state);
						break;
					default:
						break;
				}
			}
		}

#if !UNITY_EDITOR && UNITY_WSA
		public void OpenDevice(String id)
		{
			DeviceId = id;
			var fromIdAsync = HidDevice.FromIdAsync(id, FileAccessMode.Read);
			fromIdAsync.Completed += new AsyncOperationCompletedHandler<HidDevice>(OnDeviceOpened);
		}

		private void OnDeviceOpened(IAsyncOperation<HidDevice> action, AsyncStatus status)
		{
			if (status == AsyncStatus.Completed)
			{
				HidDevice Device = action.GetResults();
				if (Device != null)
				{
					Device.InputReportReceived += Device_InputReportReceived;
					Device.GetInputReportAsync();

					// Get the gamepad type from the vendor and product id
					gamepadType = GetGamepadType(Device.VendorId, Device.ProductId);

					// Try to get numeric controls.  This is broken I think
					/*IReadOnlyList<HidNumericControlDescription> cs = Device.GetNumericControlDescriptions(HidReportType.Input, 0, 0);
					for (int i = 0; i < cs.Count; ++i)
					{
						Debug.WriteLine("up: " + cs[i].UsagePage + " uid: " + cs[i].UsageId);
					}*/
				}
				else
				{
					//Debug.WriteLine("Failed, why");
					//var status1 = Windows.Devices.Enumeration.DeviceAccessInformation.CreateFromId(DeviceId);
					//var c = status1.CurrentStatus;
				}
			}
		}

		public void CloseDevice()
		{
			if (Device != null)
			{
				Device.InputReportReceived -= Device_InputReportReceived;

				Device.Dispose();
				Device = null;
				DeviceId = null;
			}
		}

		void OnUpdate()
		{
			// Copy only control states
			AssignFrom(new DeviceState(), true);

			// Increment update count
			++updateCount;
		}

		private void Device_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
		{
			// Retrieve the sensor data
			HidInputReport inputReport = args.Report;
			IBuffer buffer = inputReport.Data;
			DataReader dr = DataReader.FromBuffer(buffer);
			byte[] bytes = new byte[inputReport.Data.Length];
			dr.ReadBytes(bytes);

			if (bytes.Length <= 0)
			{
				return;
			}

			lock(this)
			{
				// Clear the control states
				OnUpdate();

				// Report button controls
				foreach (var button in args.Report.ActivatedBooleanControls)
				{
					ReportControl((int)button.Id, 1f);
				}

				// Report numeric controls
				int[] specialControls = GetNumericControls(gamepadType);
				for (int i = 0; i < specialControls.Length; ++i)
				{
					HidNumericControl ctrl = args.Report.GetNumericControl(1, (ushort)specialControls[i]);
					if (ctrl != null)
					{
						ReportControl(specialControls[i], ctrl.Value);
					}
				}
			}
		}

		void OuputControlsSearch()
		{
			// Dumb search - Can be used to find all numeric control descriptions.  Seems that the functions that were supposed to
			// enumerate them weren't working very well.
			for (int up = 0; up < 256; ++up)
			{
				for (int uid = 0; uid < 256; ++uid)
				{
					IReadOnlyList<HidNumericControlDescription> ctrls2 = Device.GetNumericControlDescriptions(HidReportType.Input, (ushort)up, (ushort)uid);
					if (ctrls2.Count != 0)
					{
						//Debug.WriteLine("Page: " + up + ", Id: " + uid + ", Controls: " + ctrls2.Count);
					}
				}
			}
		}
#endif
    }

#if !UNITY_EDITOR && UNITY_WSA
	public class DeviceConnect
	{
		private static DeviceWatcher DeviceWatcher;

		public static List<DeviceState> states = new List<DeviceState>();

		public static void Init()
		{
			// Enumerate a specific gamepad
			//var gamepadDeviceId = HidDevice.GetDeviceSelector(1, 5, 0x45e, 0x02e0);	// older 360 usb?
			//var gamepadDeviceId = HidDevice.GetDeviceSelector(1, 5, 0x45e, 0x028e);		// 360 usb
			//var gamepadDeviceId = HidDevice.GetDeviceSelector(1, 5, 0x04e8, 0xa000);    // Samsung EI-GP20 bt
			//var gamepadDeviceId = HidDevice.GetDeviceSelector(1, 5, 0x54c, 0x5c4);		// bt ps4 ds4

			var gamepadDeviceId = HidDevice.GetDeviceSelector(1, 5);        // Enumerates all controllers, but may be undesirable

			// Create a device watcher to look for instances of the IR_Sensor device
			if (DeviceWatcher == null)
			{
				DeviceWatcher = DeviceInformation.CreateWatcher(gamepadDeviceId);

				DeviceWatcher.Added += DeviceWatcher_Added;
				DeviceWatcher.Removed += DeviceWatcher_Removed;
				DeviceWatcher.Updated += DeviceWatcher_Updated;
				DeviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
				DeviceWatcher.Start();
			}
		}

		private static void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args) { }

		private static void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
		{
			OpenDevice(args.Id);
		}

		private static void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
		{
			OpenDevice(args.Id);
		}

		private static void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
		{
			lock (states)
			{
				foreach (DeviceState ds in states)
				{
					if (ds.DeviceId == args.Id)
					{
						ds.CloseDevice();
						states.Remove(ds);
						return;
					}
				}
			}
		}

		private static void OpenDevice(string dId)
		{
			DeviceState newState = null;
			lock (states)
			{
				foreach (DeviceState ds in states)
				{
					if (ds.DeviceId == dId)
					{
						return;
					}
				}

				newState = new DeviceState();
				states.Add(newState);
			}

			if (newState != null)
			{
				newState.OpenDevice(dId);
			}
		}
	}
#endif
}

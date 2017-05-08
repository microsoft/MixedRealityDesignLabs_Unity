//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Utility;
using HUX;

// Updates gamepad from network source (most likely a controller connected to a pc)
public class InputSourceNetGamepad : InputSourceGamepadBase
{
	public bool debugPrint;
	public int messageCount;

	LLNetwork network = new LLNetwork();
	public int port = 9006;

	void Start()
	{
		network.OnReceivedBytes += Network_OnReceivedBytes;

		network.StartHost(port);
	}

	public override bool IsPresent()
	{
		return Connected();
	}

	public bool Connected()
	{
		return messageCount > 0;
	}

	public override void _Update()
	{
		network.Update();

		base._Update();
	}

	private void Network_OnReceivedBytes(byte[] bytes)
	{
		if (debugPrint)
		{
			Debug.Log("Received: " + bytes.Length);
		}

		// Expecting 1 byte
		if (bytes.Length == 17)
		{
			++messageCount;

			// Print out the bytes if enabled
			if (debugPrint)
			{
				string bstr = "";
				for (int i = 0; i < bytes.Length; ++i)
				{
					bstr += bytes[i] + " ";
				}
				Debug.Log(bstr);
			}

			// Read out button states
			int idx = 0;
			byte buttonState = bytes[idx++];
			bool aButtonPressed = (buttonState & 1) != 0 ? true : false;
			bool bButtonPressed = (buttonState & 2) != 0 ? true : false;
			bool startButtonPressed = (buttonState & 4) != 0 ? true : false;
			idx += readNormalizedFloat(out leftJoyVector.x, bytes, idx);
			idx += readNormalizedFloat(out leftJoyVector.y, bytes, idx);
			idx += readNormalizedFloat(out rightJoyVector.x, bytes, idx);
			idx += readNormalizedFloat(out rightJoyVector.y, bytes, idx);
			idx += readNormalizedFloat(out trigVector.x, bytes, idx);
			idx += readNormalizedFloat(out trigVector.y, bytes, idx);
			idx += readNormalizedFloat(out padVector.x, bytes, idx);
			idx += readNormalizedFloat(out padVector.y, bytes, idx);

			aButtonState.ApplyState(aButtonPressed);
			bButtonState.ApplyState(bButtonPressed);
			startButtonState.ApplyState(startButtonPressed);
		}
		else
		{
			Debug.Log("Bytes: " + (bytes != null ? bytes.Length.ToString() : "null"));
		}
	}
}


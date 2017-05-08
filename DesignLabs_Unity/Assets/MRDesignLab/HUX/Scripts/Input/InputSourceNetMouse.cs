//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Utility;

public class InputSourceNetMouse : InputSourceBase
{
	public bool buttonLeftPressed;
	public bool buttonRightPressed;
	public bool buttonSelectPressed;
	public bool buttonUpArrowPressed;
	public bool buttonDownArrowPressed;
	
	public Vector2 mousePos;

	public bool debugPrint;
	public int messageCount;

	LLNetwork network = new LLNetwork();
	public int port = 9004;

	void Start()
	{
		network.OnReceivedBytes += Network_OnReceivedBytes;

		network.StartHost(port);
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

		// Expecting 5 bytes
		if (bytes.Length == 5)
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
			buttonLeftPressed = (buttonState & 1) != 0 ? true : false;
			buttonRightPressed = (buttonState & 2) != 0 ? true : false;
			buttonSelectPressed = (buttonState & 4) != 0 ? true : false;
			buttonUpArrowPressed = (buttonState & 8) != 0 ? true : false;
			buttonDownArrowPressed = (buttonState & 16) != 0 ? true : false;

			// Read out the position, and convert back to float position
			Vector2 pos = Vector2.zero;
			idx += readNormalizedFloat(out pos.x, bytes, idx);
			idx += readNormalizedFloat(out pos.y, bytes, idx);

			mousePos = pos;
		}
		else
		{
			Debug.Log("Bytes: " + (bytes != null ? bytes.Length.ToString() : "null"));
		}
	}
}


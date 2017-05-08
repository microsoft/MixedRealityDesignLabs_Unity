//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Amortizer : MonoBehaviour
{
	public static YieldInstruction Process(GameObject owner, string workName, float rate, Func<int> step)
	{
		// Find or create Amortizer component.  Needed to run coroutine
		Amortizer amortizer = owner.GetComponent<Amortizer>();
		if (amortizer == null)
		{
			amortizer = owner.AddComponent<Amortizer>();
		}

		return amortizer.Process(workName, rate, step);
	}

	// Match work total history with the name of the amortized task. Kinda dumb
	Dictionary<string, int> totalWork = new Dictionary<string, int>();

	YieldInstruction Process(string workName, float rate, Func<int> step)
	{
		return StartCoroutine(CoUpdate(workName, rate, step));
	}

	IEnumerator CoUpdate(string workName, float rate, Func<int> step)
	{
		// Remember the last work done for this amortized task
		int lastTotalWork = 0;
		if (totalWork.ContainsKey(workName))
		{
			lastTotalWork = totalWork[workName];
		}

		//Debug.Log("Amortizer begin, lastTotalWork: " + lastTotalWork);

		int counter = 0;
		bool finished = false;
		while (!finished)
		{
			// Execute the step function, which does the work
			int stepWork = step();
			if (stepWork >= 0)
			{
				counter += stepWork;

				//Debug.Log("Amortizer did work " + stepWork + ", total: " + counter);
			}
			else
			{
				//Debug.Log("Amortizer " + workName + " finished!  Total: " + counter);
				// Work is done when <0 is returned.  Record the work done, and finish
				totalWork[workName] = counter;
				finished = true;
				yield break;
			}

			// ConditionalWait will return a number of seconds to delay, or null if it's okay to continue
			YieldInstruction result = ConditionalWait(ref counter, lastTotalWork, rate);
			if (result != null)
			{
				//Debug.Log("Amortizer waiting");
				yield return result;
			}

			//Debug.Log("Amortizer Looping!");
		}
	}

	// Utility method to determine when to yield and for how long
	// Default to processing everything in 5 seconds, the 'rate'
	YieldInstruction ConditionalWait(ref int counter, int total, float rate = 5f)
	{
		if (total <= 0)
		{
			return null;
		}

		const float minDelay = 1f / 30f;    // Delay requests smaller than this end up taking this long
		float delay = rate / (float)total;  // And this is the delay between ops to meet our rate

		// If there are less ops than frames, let's go ahead and delay
		if (delay > minDelay)
		{
			return new WaitForSeconds(delay);
		}
		else
		{
			// Otherwise, there are more ops than frames.  Let's just yield enough to spread the work out.
			// This won't ensure that framerate isn't affected, so adjust 'rate' as needed
			++counter;

			// How many ops are there per frame?  When 'counter' reaches it, we'll take a break
			float amount = (float)total * minDelay / rate;
			if ((float)counter >= amount)
			{
				// Decrement the counter and wait one frame!
				// Ideally this would invoke a yield return null in the caller coroutine.  WaitForEndOfFrame
				// is not ideal since it is executed just before rendering.  So hopefully WaitForSeconds(0)
				// does the trick.
				counter -= (int)amount;
				return new WaitForSeconds(0);
			}
		}
		return null;
	}
}

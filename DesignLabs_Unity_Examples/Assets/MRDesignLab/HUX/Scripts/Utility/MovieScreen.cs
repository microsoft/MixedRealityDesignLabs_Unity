//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections;

namespace HUX.Utility
{

	public class MovieScreen : MonoBehaviour
	{
		#region public vars
		[Serializable]
		public class MovieStateDatum
		{
#if !UNITY_ANDROID && !UNITY_WEBGL
			public MovieTexture targetMovie;
#endif
			public Texture targetTexture;
			public bool LoopMovie = false;
			public int LoopCount = 0;
		}

		public MovieStateDatum[] MovieStates;
		public int currentState;
#endregion

#region private vars
		private Renderer mRenderer;
#endregion

		private void Start()
		{
			mRenderer = this.gameObject.GetComponent<Renderer>();
			SetState(currentState);
		}

		public void SetState(int newState)
		{
#if !UNITY_ANDROID && !UNITY_WEBGL
			if (MovieStates.Length > newState)
			{
				MovieStateDatum stateDatum = MovieStates[newState];
				if (stateDatum.targetMovie != null)
				{
					mRenderer.material.SetTexture("_MainTex", stateDatum.targetMovie);
					mRenderer.material.SetTexture("_AlphaTex", stateDatum.targetMovie);
					stateDatum.targetMovie.loop = stateDatum.LoopMovie;

					if (stateDatum.LoopCount > 0 && !stateDatum.LoopMovie)
					{
						StartCoroutine("PlayMovieLoop", stateDatum);
					}
					else
					{
						stateDatum.targetMovie.Play();
					}

					mRenderer.enabled = true;
				}
				else
				{
					if (stateDatum.targetTexture != null)
					{
						mRenderer.material.SetTexture("_MainTex", stateDatum.targetTexture);
						mRenderer.material.SetTexture("_AlphaTex", stateDatum.targetTexture);
						mRenderer.enabled = true;
					}
					else
					{
						mRenderer.enabled = false;
					}
				}
			}
			else
			{
				Debug.LogWarning("State Index is Out of Range!");
			}
#endif
		}

		private IEnumerator PlayMovieLoop(MovieStateDatum stateDatum)
		{
#if !UNITY_ANDROID && !UNITY_WEBGL
			int loopCount = stateDatum.LoopCount;
			while (loopCount > 0)
			{
				stateDatum.targetMovie.Play();
				yield return new WaitForSeconds(stateDatum.targetMovie.duration);
				loopCount--;
			}
#else
			yield return null;
#endif
		}
	}
}

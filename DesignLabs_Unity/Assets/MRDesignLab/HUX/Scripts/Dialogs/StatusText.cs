//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class StatusText : MonoBehaviour
{
	private static StatusText m_Instance;
	public static StatusText Instance
	{
		get { return m_Instance; }
	}

	public TextMesh m_TextMesh;

	public float Delay = 1.2f;
	protected float timeSinceLastEnabled = 0.0f;
	
	public void Awake()
	{
		m_Instance = this;
	}

	public void SetText(string text)
	{
		m_TextMesh.gameObject.SetActive(true);

		m_TextMesh.text = text;
		timeSinceLastEnabled = 0.0f;
	}

	public void SetTextUntimed(string text)
	{
		m_TextMesh.gameObject.SetActive(true);

		m_TextMesh.text = text;
		timeSinceLastEnabled = -1.0f;
	}

	public void Update()
	{
		if (gameObject.activeSelf)
		{
			if (timeSinceLastEnabled >= 0.0f)
			{
				timeSinceLastEnabled += Time.deltaTime;
			}

			if (timeSinceLastEnabled > Delay)
			{
				m_TextMesh.gameObject.SetActive(false);
			}
		}
	}
}

//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class TextItem: MonoBehaviour
{
	public Color m_HighlightColour;
	private Color m_OrigColour;
	private TextMesh m_TextMesh;

	public struct Info
	{
		public float m_Duration;
		public float m_TimeElapsed;
	}

	public Info m_Info;
	
	public void Start()
	{
		m_TextMesh = GetComponent<TextMesh>();

		if (m_TextMesh != null)
		{
			m_OrigColour = m_TextMesh.color;
		}
	}

	public void Update()
	{
		if (m_Info.m_TimeElapsed <= m_Info.m_Duration)
		{
			m_Info.m_TimeElapsed += Time.deltaTime;

			SetColour();
		}
	}

	public void Highlight(float duration = 1.0f)
	{
		m_Info.m_Duration = duration;
		m_Info.m_TimeElapsed = 0.0f;

		SetColour();
	}

	public void CopyFrom(TextItem textItem)
	{
		m_TextMesh.text = textItem.GetText();
		m_Info = textItem.m_Info;

		SetColour();
	}

	public string GetText()
	{
		if (m_TextMesh != null)
		{
			return m_TextMesh.text;
		}
		return "";
	}

	public void SetText(string text)
	{
		if (m_TextMesh != null)
		{
			m_TextMesh.text = text;
		}
	}

	private void SetColour()
	{
		float percent = Mathf.Clamp01(m_Info.m_TimeElapsed / m_Info.m_Duration);

		Color col = Color.Lerp(m_HighlightColour, m_OrigColour, percent);
		m_TextMesh.color = col;
	}
}
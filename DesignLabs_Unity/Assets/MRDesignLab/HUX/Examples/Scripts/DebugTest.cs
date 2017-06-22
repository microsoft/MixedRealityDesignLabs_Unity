//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Dialogs.Debug;

public class DebugTest : MonoBehaviour
{
    public enum EnumTest
    {
        Value1,
        Value2,
        Value3,
        Value4,
    }

	#region Enum Test
	[DebugEnumTunable]
    public EnumTest m_EnumTest1 = EnumTest.Value1;

	[DebugEnumTunable(itemName: "Enum test group\\m_EnumTest2", minEnumVal: (int)EnumTest.Value2, maxEnumVal: (int)EnumTest.Value3)]
	public EnumTest m_EnumTest2 = EnumTest.Value2;
	#endregion

	#region Float Test
	[DebugNumberTunable]
	public float m_FloatTest1 = 5.0f;

	[DebugNumberTunable(itemName: "Float test Group\\m_FloatTest2")]
	public float m_FloatTest2 = 5.0f;

	[DebugNumberTunable(step: 0.5f)]
	public float m_FloatTest3 = 5.0f;

	[DebugNumberTunable(itemName: "Float test Group\\m_FloatTest4", step: 0.5f)]
	public float m_FloatTest4 = 5.0f;

	[DebugNumberTunable(minRange: 4, maxRange: 20)]
	public float m_FloatTest5 = 5.0f;

	[DebugNumberTunable(itemName: "Float test Group\\m_FloatTest6", minRange: 5, maxRange: 20)]
	public float m_FloatTest6 = 5.0f;

	[DebugNumberTunable(minRange: 5, maxRange: 20, step: 0.25f)]
	public float m_FloatTest7 = 5.0f;

	[DebugNumberTunable(itemName: "Float test Group\\m_FloatTest8", minRange: 5, maxRange: 20, step: 0.25f)]
	public float m_FloatTest8 = 5.0f;
	#endregion

	#region Int Test
	[DebugNumberTunable]
	public int m_IntTest1 = 5;

	[DebugNumberTunable(itemName: "Int test Group\\m_IntTest2")]
	public int m_IntTest2 = 5;

	[DebugNumberTunable(step: 5)]
	public int m_IntTest3 = 5;

	[DebugNumberTunable(itemName: "Int test Group\\m_IntTest4", step: 5)]
	public int m_IntTest4 = 5;

	[DebugNumberTunable(minRange: 4, maxRange: 20)]
	public int m_IntTest5 = 5;

	[DebugNumberTunable(itemName: "Int test Group\\m_IntTest6", minRange: 5, maxRange: 20)]
	public int m_IntTest6 = 5;

	[DebugNumberTunable(minRange: 5, maxRange: 20, step: 5)]
	public int m_IntTest7 = 5;

	[DebugNumberTunable(itemName: "Int test Group\\m_IntTest8", minRange: 5, maxRange: 20, step: 5)]
	public int m_IntTest8 = 5;
	#endregion

	[DebugBoolTunable]
	public bool m_BoolTest;

	#region Vector3 Test
	[DebugVector3Tunable]
	public Vector3 m_Vector3Test1 = Vector3.one;

	[DebugVector3Tunable("Vector3 Test Group\\m_Vector3Test2")]
	public Vector3 m_Vector3Test2 = Vector3.one;

	[DebugVector3Tunable]
	public Vector3 m_Vector3Test3 = Vector3.one;

	[DebugVector3Tunable("Vector3 Test Group\\m_Vector3Test2", 0.5f, 0.2f, 0.1f)]
	public Vector3 m_Vector3Test4 = Vector3.one;
    #endregion

    private EnumTest m_EnumTest;
    private float m_FloatTest;
    private int m_IntTest;


    // Use this for initialization
    void Start ()
    {
		DebugMenu.Instance.AddEnumItem<EnumTest>("Enum Test Group\\Enum Test", (int newVal) => { m_EnumTest = (EnumTest)newVal; }, null, (int)m_EnumTest, (int)EnumTest.Value1, (int)EnumTest.Value3, this.gameObject);

        DebugMenu.Instance.AddFloatItem("Test 1\\Float Test", (float newVal) => { m_FloatTest = newVal; }, null, m_FloatTest, 1f, 10f, this.gameObject, 0.5f);

        DebugMenu.Instance.AddEnumItem<EnumTest>("Enum Test Group\\Enum Test", (int newVal) => { m_EnumTest = (EnumTest)newVal; }, null, (int)m_EnumTest, (int)EnumTest.Value1, (int)EnumTest.Value3, this.gameObject);

        DebugMenu.Instance.AddBoolItem("Bool Test", (bool newVal) => { m_BoolTest = newVal; }, null, m_BoolTest, this.gameObject);

        DebugMenu.Instance.AddIntItem("Int Test", (int newVal) => { m_IntTest = newVal; }, null, m_IntTest, 1, 10, this.gameObject, 2);

		DebugMenu.Instance.AddButtonItem("Button Test", "Button 1", OnDebugBtn1Pressed);
        DebugMenu.Instance.AddButtonItem("Button Test", "Button 2", OnDebugBtn2Pressed);

        DebugMenu.Instance.AddEnumItem<EnumTest>("Enum Test Group\\Enum Test", (int newVal) => { m_EnumTest = (EnumTest)newVal; }, null, (int)m_EnumTest, (int)EnumTest.Value1, (int)EnumTest.Value3, this.gameObject);

		//DebugMenu.Instance.AddButtonItem("Group 1\\Button Test", "Button 1", OnDebugBtn1Pressed);
		//DebugMenu.Instance.AddButtonItem("Group 2\\Button Test", "Button 2", OnDebugBtn2Pressed);
		//DebugMenu.Instance.AddButtonItem("Button Test", "Button 3", OnDebugBtn3Pressed);
	}

	[DebugButton]
    private void OnDebugBtn1Pressed()
    {
        Debug.Log("Button 1 Pressed");
    }

	[DebugButton(itemName: "Button Test Group\\Button Test")]
	private void OnDebugBtn2Pressed()
    {
        Debug.Log("Button 2 Pressed");
    }

	
	[DebugButton(itemName: "Button Test Group\\Button Test 2", buttonName: "Press")]
	public void OnDebugBtn3Pressed()
	{
		Debug.Log("Button 3 Pressed");
	}
}

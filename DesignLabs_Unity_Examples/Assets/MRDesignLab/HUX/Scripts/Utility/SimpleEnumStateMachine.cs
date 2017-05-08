//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class SimpleEnumStateMachine<T> where T : struct, System.IComparable
{
    private class StateCallbacks
    {
        public System.Action enterStateCallback;
        public System.Action updateStateCallback;
        public System.Action exitStateCallback;
    }

    private int m_CurrentStateIndex = -1;
    private StateCallbacks m_CurrentStateCallbacks = null;
    private StateCallbacks[] m_StateCallbacks;

    public SimpleEnumStateMachine()
    {
        int enumCount = System.Enum.GetValues(typeof(T)).Length;
        m_StateCallbacks = new StateCallbacks[enumCount];

        for (int i = 0; i < enumCount; ++i)
        {
            m_StateCallbacks[i] = new StateCallbacks();
        }
    }

    public void SetStateCallbacks(T state, System.Action enterStateCallback, System.Action updateStateCallback, System.Action exitStateCallback)
    {
        int index = (int)(object)state;
        StateCallbacks stateCallbacks = m_StateCallbacks[index];

        stateCallbacks.enterStateCallback = enterStateCallback;
        stateCallbacks.updateStateCallback = updateStateCallback;
        stateCallbacks.exitStateCallback = exitStateCallback;
    }

    public void SetState(T state)
    {
        int stateIndex = (int)(object)state;
        if (stateIndex == m_CurrentStateIndex) { return; }

        // Leave current state
        if (m_CurrentStateCallbacks != null && m_CurrentStateCallbacks.exitStateCallback != null)
        {
            m_CurrentStateCallbacks.exitStateCallback();
        }

        // Set state
        m_CurrentStateIndex = stateIndex;
        m_CurrentStateCallbacks = m_StateCallbacks[m_CurrentStateIndex];

        // Enter new state
        if (m_CurrentStateCallbacks != null && m_CurrentStateCallbacks.enterStateCallback != null)
        {
            m_CurrentStateCallbacks.enterStateCallback();
        }
    }

    public T GetState()
    {
        return (T)(object)m_CurrentStateIndex;
    }

    public void Update()
    {
        if (m_CurrentStateCallbacks != null && m_CurrentStateCallbacks.updateStateCallback != null)
        {
            m_CurrentStateCallbacks.updateStateCallback();
        }
    }
}
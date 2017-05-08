//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;
using System;

namespace HUX.Dialogs.Debug
{
    /// <summary>
    /// A debug menu item to display a number slider.
    /// </summary>
    public class DebugSliderItem : DebugMenuItem
    {
        /// <summary>
        /// The UI Slider element.
        /// </summary>
        [SerializeField]
        private Slider m_Slider;

        /// <summary>
        /// The UI text element to display the current value in.
        /// </summary>
        [SerializeField]
        private Text m_NumberValue;

        /// <summary>
        /// The function to call if we are changing a floating point value and it changes.
        /// </summary>
        private Action<float> m_FloatCallback;

        /// <summary>
        /// The function to call if we are changing an integer value and it changes.
        /// </summary>
        private Action<int> m_IntCallback;

		/// <summary>
		/// The function to call to get the current float value.
		/// </summary>
		private Func<float> m_FloatValFunc;

		/// <summary>
		/// The function to call to get the current int value;
		/// </summary>
		private Func<int> m_IntValFunc;

        /// <summary>
        /// The amount to step the value by.
        /// </summary>
        private float m_Step = 0.1f;

		private bool blockCallback;

		/// <summary>
		/// The floating point setup function.
		/// </summary>
		/// <param name="name">The display name.</param>
		/// <param name="callback">The function to call when the value changes.</param>
		/// <param name="valFunc">The function to call to get the value.</param>
		/// <param name="initialVal">The initial value.</param>
		/// <param name="minVal">The minimum value.</param>
		/// <param name="maxVal">The maximum value.</param>
		/// <param name="step">The amount to step the value by on the slider.</param>
		public void Setup(string name, Action<float> callback, Func<float> valFunc, float initialVal, float minVal, float maxVal, float step)
		{
			Title = name;
			
			m_Slider.minValue = minVal;
			m_Slider.maxValue = maxVal;

			// We need to set the callback after setting m_Min and m_Max.  *Important* bug fix here.
			m_FloatCallback = callback;
			m_FloatValFunc = valFunc;

			if (m_FloatValFunc != null)
			{				
				initialVal = m_FloatValFunc();
			}

			UpdateUIValue(initialVal);

			m_Step = step;
		}

		/// <summary>
		/// The integer setup function.
		/// </summary>
		/// <param name="name">The display name.</param>
		/// <param name="callback">The function to call when the value changes.</param>
		/// <param name="valFunc">The function to call to get the value.</param>
		/// <param name="initialVal">The initial value.</param>
		/// <param name="minVal">The minimum value.</param>
		/// <param name="maxVal">The maximum value.</param>
		/// <param name="step">The amount to step the value by on the slider.</param>
		public void Setup(string name, Action<int> callback, Func<int> valFunc, int initialVal, int minVal, int maxVal, int step)
		{
			Title = name;
			
			m_Slider.minValue = minVal;
			m_Slider.maxValue = maxVal;

			m_Slider.wholeNumbers = true;

			// We need to set the callback after setting m_Min and m_Max.  *Important* bug fix here.
			m_IntCallback = callback;
			m_IntValFunc = valFunc;

			if (m_IntValFunc != null)
			{
				initialVal = m_IntValFunc();
			}

			UpdateUIValue(initialVal);

			m_Step = step;
		}

		/// <summary>
		/// The callback when the slider value changes.
		/// </summary>
		public void SliderValueChanged()
        {
			if (blockCallback)
			{
				return;
			}

			float value = m_Slider.value;

            float range = m_Slider.maxValue - m_Slider.minValue;
            int numSteps = (int)((range) / m_Step) + 1;
            float stepPercent = numSteps * ((value - m_Slider.minValue) / range);

            int lowStepNum = (int)Mathf.Floor(stepPercent);
            int highStepNum = (int)Mathf.Ceil(stepPercent);

            float lowStep = (m_Step * lowStepNum) + m_Slider.minValue;
            float highStep = (m_Step * highStepNum) + m_Slider.minValue;

            value = (Mathf.Abs(lowStep - value) < Mathf.Abs(highStep - value)) ? lowStep : highStep;

            value = Mathf.Clamp(value, m_Slider.minValue, m_Slider.maxValue);

			UpdateUIValue(value);

			if (m_FloatCallback != null)
            {
                m_FloatCallback(value);
            }

            if (m_IntCallback != null)
            {
                m_IntCallback((int)value);
            }
        }

        /// <summary>
        /// Called when the increment button is pressed.
        /// </summary>
        public void OnInc()
        {
            float newValue = m_Slider.value + m_Step;
            if (newValue > m_Slider.maxValue)
            {
                newValue = m_Slider.maxValue;
            }
            m_Slider.value = newValue;

            this.SliderValueChanged();
        }

        /// <summary>
        /// Called when the decrement button is pressed.
        /// </summary>
        public void OnDec()
        {
            float newValue = m_Slider.value - m_Step;
            if (newValue < m_Slider.minValue)
            {
                newValue = m_Slider.minValue;
            }
            m_Slider.value = newValue;

            this.SliderValueChanged();
        }

		/// <summary>
		/// Updates the sliders value and number label
		/// </summary>
		/// <param name="num"></param>
		private void UpdateUIValue(float num)
		{
			blockCallback = true;

			m_Slider.value = num;
			m_NumberValue.text = num.ToString();

			blockCallback = false;
		}

		/// <summary>
		/// Windows update Function.
		/// </summary>
		private void Update()
		{
			if (m_FloatValFunc != null)
			{
				UpdateUIValue(m_FloatValFunc());
			}
			else if (m_IntValFunc != null)
			{
				UpdateUIValue(m_IntValFunc());
			}
		}
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#if !UNITY_EDITOR

using System.Collections;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#endif
#endif

public class InputMotionControllerReceiver : MonoBehaviour
{
    [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. This will override the platform left controller model.")]
    [SerializeField]
    protected GameObject LeftControllerModelPrefab;
    [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. This will override the platform right controller model.")]
    [SerializeField]
    protected GameObject RightControllerModelPrefab;

    [SerializeField]
    protected GameObject LeftControllerInputSource;
    [SerializeField]
    protected GameObject RightControllerInputSource;

    [Tooltip("Use this to override the indicator used to show the user's touch location on the touchpad. Default is a sphere.")]
    [SerializeField]
    protected GameObject TouchpadTouchedOverride;

    [Tooltip("This shader will be used on the loaded GLTF controller model. This does not affect the above overrides.")]
    public Shader GLTFShader;

    private UnityEngine.XR.WSA.Input.InteractionSourceState m_PrevState = new UnityEngine.XR.WSA.Input.InteractionSourceState();

#if !UNITY_EDITOR && UNITY_WSA
        // This is used to get the renderable controller model, since Unity does not expose this API.
        private SpatialInteractionManager spatialInteractionManager;
#endif

    // This will be used to keep track of our controllers, indexed by their unique source ID.
    public Dictionary<uint, ControllerInfo> controllerDictionary;

    public Action<InputSourceMotionController> MenuButtonClicked { get; internal set; }
    public Action<InputSourceMotionController> MenuButtonUnclicked { get; internal set; }
    public Action<InputSourceMotionController> TriggerClicked { get; internal set; }
    public Action<InputSourceMotionController> TriggerUnclicked { get; internal set; }
    public Action<InputSourceMotionController> SteamClicked { get; internal set; }
    public Action<InputSourceMotionController> PadClicked { get; internal set; }
    public Action<InputSourceMotionController> PadUnclicked { get; internal set; }
    public Action<InputSourceMotionController> PadTouched { get; internal set; }
    public Action<InputSourceMotionController> PadUntouched { get; internal set; }
    public Action<InputSourceMotionController> Gripped { get; internal set; }
    public Action<InputSourceMotionController> Ungripped { get; internal set; }

    private void Start()
    {
        controllerDictionary = new Dictionary<uint, ControllerInfo>();

#if UNITY_WSA
        // Since we're using non-Unity APIs, glTF will only load in a UWP app.
        if (LeftControllerModelPrefab == null && RightControllerModelPrefab == null)
        {
            Debug.Log("Running in the editor won't render the glTF models, and no controller overrides are set. Please specify them on " + name + ".");
        }
        else if (LeftControllerModelPrefab == null || RightControllerModelPrefab == null)
        {
            Debug.Log("Running in the editor won't render the glTF models, and only one controller override is specified. Please set the " + ((LeftControllerModelPrefab == null) ? "left" : "right") + " override on " + name + ".");
        }

        InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;

        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
#endif
    }

#if UNITY_WSA
    private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        if (obj.state.source.kind == InteractionSourceKind.Controller && !controllerDictionary.ContainsKey(obj.state.source.id))
        {
            GameObject controllerModelGameObject;
            if (obj.state.source.handedness == InteractionSourceHandedness.Left && LeftControllerModelPrefab != null)
            {
                controllerModelGameObject = Instantiate(LeftControllerModelPrefab);
            }
            else if (obj.state.source.handedness == InteractionSourceHandedness.Right && RightControllerModelPrefab != null)
            {
                controllerModelGameObject = Instantiate(RightControllerModelPrefab);
            }
            else // InteractionSourceHandedness.Unknown || both overrides are null
            {
                return;
            }

            FinishControllerSetup(controllerModelGameObject, true, obj.state.source.handedness.ToString(), obj.state.source.id);
        }
    }

    /// <summary>
    /// When a controller is lost, the model is destroyed and the controller object
    /// is removed from the tracking dictionary.
    /// </summary>
    /// <param name="obj">The source event args to be used to determine the controller model to be removed.</param>
    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        InteractionSource source = obj.state.source;
        if (source.kind == InteractionSourceKind.Controller)
        {
            ControllerInfo controller;
            if (controllerDictionary != null && controllerDictionary.TryGetValue(source.id, out controller))
            {
                Destroy(controller.gameObject);

                // After destruction, the reference can be removed from the dictionary.
                controllerDictionary.Remove(source.id);
            }
        }
    }

    private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        ControllerInfo currentController;
        if (controllerDictionary != null && controllerDictionary.TryGetValue(obj.state.source.id, out currentController))
        {
            // Update input sources
            foreach(InputSourceMotionController inputSource in FindObjectsOfType<InputSourceMotionController>())
            {
                if (inputSource.handedness == obj.state.source.handedness)
                    inputSource.UpdateInput(obj);

                //InteractionSourceState currentState = obj.state;

                //m_PrevState = currentState;
            }

            // Update visuals
            currentController.AnimateSelect(obj.state.selectPressedAmount);

            if (obj.state.source.supportsGrasp)
            {
                currentController.AnimateGrasp(obj.state.grasped);
            }

            if (obj.state.source.supportsMenu)
            {
                currentController.AnimateMenu(obj.state.menuPressed);
            }

            if (obj.state.source.supportsThumbstick)
            {
                currentController.AnimateThumbstick(obj.state.thumbstickPressed, obj.state.thumbstickPosition);
            }

            if (obj.state.source.supportsTouchpad)
            {
                currentController.AnimateTouchpad(obj.state.touchpadPressed, obj.state.touchpadTouched, obj.state.touchpadPosition);
            }

            Vector3 newPosition;
            if (obj.state.sourcePose.TryGetPosition(out newPosition, InteractionSourceNode.Grip))
            {
                currentController.gameObject.transform.localPosition = newPosition;
            }

            Quaternion newRotation;
            if (obj.state.sourcePose.TryGetRotation(out newRotation, InteractionSourceNode.Grip))
            {
                currentController.gameObject.transform.localRotation = newRotation;
            }
        }
    }

    private void FinishControllerSetup(GameObject controllerModelGameObject, bool isOverride, string handedness, uint id)
    {
        var parentGameObject = new GameObject
        {
            name = handedness + "Controller"
        };

        parentGameObject.transform.parent = transform;
        controllerModelGameObject.transform.parent = parentGameObject.transform;

        var newControllerInfo = parentGameObject.AddComponent<ControllerInfo>();
        if (!isOverride)
        {
            newControllerInfo.LoadInfo(controllerModelGameObject.GetComponentsInChildren<Transform>(), this);
        }
        controllerDictionary.Add(id, newControllerInfo);
    }
#endif

    public GameObject SpawnTouchpadVisualizer(Transform parentTransform)
    {
        GameObject touchVisualizer;
        if (TouchpadTouchedOverride != null)
        {
            touchVisualizer = Instantiate(TouchpadTouchedOverride);
        }
        else
        {
            touchVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            touchVisualizer.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
            touchVisualizer.GetComponent<Renderer>().material.shader = GLTFShader;
        }
        Destroy(touchVisualizer.GetComponent<Collider>());
        touchVisualizer.transform.parent = parentTransform;
        touchVisualizer.transform.localPosition = Vector3.zero;
        touchVisualizer.transform.localRotation = Quaternion.identity;
        touchVisualizer.SetActive(false);
        return touchVisualizer;
    }
}

public class ControllerInfo : MonoBehaviour
{
    private GameObject home;
    private Transform homePressed;
    private Transform homeUnpressed;
    private GameObject menu;
    private Transform menuPressed;
    private Transform menuUnpressed;
    private GameObject grasp;
    private Transform graspPressed;
    private Transform graspUnpressed;
    private GameObject thumbstickPress;
    private Transform thumbstickPressed;
    private Transform thumbstickUnpressed;
    private GameObject thumbstickX;
    private Transform thumbstickXMin;
    private Transform thumbstickXMax;
    private GameObject thumbstickY;
    private Transform thumbstickYMin;
    private Transform thumbstickYMax;
    private GameObject select;
    private Transform selectPressed;
    private Transform selectUnpressed;
    private GameObject touchpadPress;
    private Transform touchpadPressed;
    private Transform touchpadUnpressed;
    private GameObject touchpadPressX;
    private Transform touchpadPressXMin;
    private Transform touchpadPressXMax;
    private GameObject touchpadPressY;
    private Transform touchpadPressYMin;
    private Transform touchpadPressYMax;
    private GameObject touchpadTouchX;
    private Transform touchpadTouchXMin;
    private Transform touchpadTouchXMax;
    private GameObject touchpadTouchY;
    private Transform touchpadTouchYMin;
    private Transform touchpadTouchYMax;
    private GameObject touchpadTouchVisualizer;

    // These bools and doubles are used to determine if a button's state has changed.
    private bool wasGrasped;
    private bool wasMenuPressed;
    private bool wasThumbstickPressed;
    private bool wasTouchpadPressed;
    private bool wasTouchpadTouched;
    private Vector2 lastThumbstickPosition;
    private Vector2 lastTouchpadPosition;
    private double lastSelectPressedAmount;

    /// <summary>
    /// Iterates through the Transform array to find specifically named GameObjects.
    /// These GameObjects specifiy the animation bounds and the GameObject to modify for button,
    /// thumbstick, and touchpad animation.
    /// </summary>
    /// <param name="childTransforms">The transforms of the glTF model.</param>
    /// <param name="visualizerScript">The script containing references to any objects to spawn.</param>
    public void LoadInfo(Transform[] childTransforms, InputMotionControllerReceiver visualizerScript)
    {
        foreach (Transform child in childTransforms)
        {
            // Animation bounds are named in two pairs:
            // pressed/unpressed and min/max. There is also a value
            // transform, which is the transform to modify to
            // animate the interactions. We also look for the
            // touch transform, in order to spawn the touchpadTouched
            // visualizer.
            switch (child.name.ToLower())
            {
                case "pressed":
                    switch (child.parent.name.ToLower())
                    {
                        case "home":
                            homePressed = child;
                            break;
                        case "menu":
                            menuPressed = child;
                            break;
                        case "grasp":
                            graspPressed = child;
                            break;
                        case "select":
                            selectPressed = child;
                            break;
                        case "thumbstick_press":
                            thumbstickPressed = child;
                            break;
                        case "touchpad_press":
                            touchpadPressed = child;
                            break;
                    }
                    break;
                case "unpressed":
                    switch (child.parent.name.ToLower())
                    {
                        case "home":
                            homeUnpressed = child;
                            break;
                        case "menu":
                            menuUnpressed = child;
                            break;
                        case "grasp":
                            graspUnpressed = child;
                            break;
                        case "select":
                            selectUnpressed = child;
                            break;
                        case "thumbstick_press":
                            thumbstickUnpressed = child;
                            break;
                        case "touchpad_press":
                            touchpadUnpressed = child;
                            break;
                    }
                    break;
                case "min":
                    switch (child.parent.name.ToLower())
                    {
                        case "thumbstick_x":
                            thumbstickXMin = child;
                            break;
                        case "thumbstick_y":
                            thumbstickYMin = child;
                            break;
                        case "touchpad_press_x":
                            touchpadPressXMin = child;
                            break;
                        case "touchpad_press_y":
                            touchpadPressYMin = child;
                            break;
                        case "touchpad_touch_x":
                            touchpadTouchXMin = child;
                            break;
                        case "touchpad_touch_y":
                            touchpadTouchYMin = child;
                            break;
                    }
                    break;
                case "max":
                    switch (child.parent.name.ToLower())
                    {
                        case "thumbstick_x":
                            thumbstickXMax = child;
                            break;
                        case "thumbstick_y":
                            thumbstickYMax = child;
                            break;
                        case "touchpad_press_x":
                            touchpadPressXMax = child;
                            break;
                        case "touchpad_press_y":
                            touchpadPressYMax = child;
                            break;
                        case "touchpad_touch_x":
                            touchpadTouchXMax = child;
                            break;
                        case "touchpad_touch_y":
                            touchpadTouchYMax = child;
                            break;
                    }
                    break;
                case "value":
                    switch (child.parent.name.ToLower())
                    {
                        case "home":
                            home = child.gameObject;
                            break;
                        case "menu":
                            menu = child.gameObject;
                            break;
                        case "grasp":
                            grasp = child.gameObject;
                            break;
                        case "select":
                            select = child.gameObject;
                            break;
                        case "thumbstick_press":
                            thumbstickPress = child.gameObject;
                            break;
                        case "thumbstick_x":
                            thumbstickX = child.gameObject;
                            break;
                        case "thumbstick_y":
                            thumbstickY = child.gameObject;
                            break;
                        case "touchpad_press":
                            touchpadPress = child.gameObject;
                            break;
                        case "touchpad_press_x":
                            touchpadPressX = child.gameObject;
                            break;
                        case "touchpad_press_y":
                            touchpadPressY = child.gameObject;
                            break;
                        case "touchpad_touch_x":
                            touchpadTouchX = child.gameObject;
                            break;
                        case "touchpad_touch_y":
                            touchpadTouchY = child.gameObject;
                            break;
                    }
                    break;
                case "touch":
                    touchpadTouchVisualizer = visualizerScript.SpawnTouchpadVisualizer(child);
                    break;
            }
        }
    }

    public void AnimateGrasp(bool isGrasped)
    {
        if (grasp != null && graspPressed != null && graspUnpressed != null && isGrasped != wasGrasped)
        {
            SetLocalPositionAndRotation(grasp, isGrasped ? graspPressed : graspUnpressed);
            wasGrasped = isGrasped;
        }
    }

    public void AnimateMenu(bool isMenuPressed)
    {
        if (menu != null && menuPressed != null && menuUnpressed != null && isMenuPressed != wasMenuPressed)
        {
            SetLocalPositionAndRotation(menu, isMenuPressed ? menuPressed : menuUnpressed);
            wasMenuPressed = isMenuPressed;
        }
    }

    public void AnimateSelect(float newSelectPressedAmount)
    {
        if (select != null && selectPressed != null && selectUnpressed != null && newSelectPressedAmount != lastSelectPressedAmount)
        {
            select.transform.localPosition = Vector3.Lerp(selectUnpressed.localPosition, selectPressed.localPosition, newSelectPressedAmount);
            select.transform.localRotation = Quaternion.Lerp(selectUnpressed.localRotation, selectPressed.localRotation, newSelectPressedAmount);
            lastSelectPressedAmount = newSelectPressedAmount;
        }
    }

    public void AnimateThumbstick(bool isThumbstickPressed, Vector2 newThumbstickPosition)
    {
        if (thumbstickPress != null && thumbstickPressed != null && thumbstickUnpressed != null && isThumbstickPressed != wasThumbstickPressed)
        {
            SetLocalPositionAndRotation(thumbstickPress, isThumbstickPressed ? thumbstickPressed : thumbstickUnpressed);
            wasThumbstickPressed = isThumbstickPressed;
        }

        if (thumbstickX != null && thumbstickY != null && thumbstickXMin != null && thumbstickXMax != null && thumbstickYMin != null && thumbstickYMax != null && newThumbstickPosition != lastThumbstickPosition)
        {
            Vector2 thumbstickNormalized = (newThumbstickPosition + Vector2.one) / 2.0f;

            thumbstickX.transform.localPosition = Vector3.Lerp(thumbstickXMin.localPosition, thumbstickXMax.localPosition, thumbstickNormalized.x);
            thumbstickX.transform.localRotation = Quaternion.Lerp(thumbstickXMin.localRotation, thumbstickXMax.localRotation, thumbstickNormalized.x);

            thumbstickY.transform.localPosition = Vector3.Lerp(thumbstickYMax.localPosition, thumbstickYMin.localPosition, thumbstickNormalized.y);
            thumbstickY.transform.localRotation = Quaternion.Lerp(thumbstickYMax.localRotation, thumbstickYMin.localRotation, thumbstickNormalized.y);

            lastThumbstickPosition = newThumbstickPosition;
        }
    }

    public void AnimateTouchpad(bool isTouchpadPressed, bool isTouchpadTouched, Vector2 newTouchpadPosition)
    {
        if (touchpadPress != null && touchpadPressed != null && touchpadUnpressed != null && isTouchpadPressed != wasTouchpadPressed)
        {
            SetLocalPositionAndRotation(touchpadPress, isTouchpadPressed ? touchpadPressed : touchpadUnpressed);
            wasTouchpadPressed = isTouchpadPressed;
        }

        if (touchpadTouchVisualizer != null && isTouchpadTouched != wasTouchpadTouched)
        {
            touchpadTouchVisualizer.SetActive(isTouchpadTouched);
            wasTouchpadTouched = isTouchpadTouched;
        }

        if (touchpadTouchX != null && touchpadTouchY != null && touchpadTouchXMin != null && touchpadTouchXMax != null && touchpadTouchYMin != null && touchpadTouchYMax != null && newTouchpadPosition != lastTouchpadPosition)
        {
            Vector2 touchpadNormalized = (newTouchpadPosition + Vector2.one) / 2.0f;

            touchpadTouchX.transform.localPosition = Vector3.Lerp(touchpadTouchXMin.localPosition, touchpadTouchXMax.localPosition, touchpadNormalized.x);
            touchpadTouchX.transform.localRotation = Quaternion.Lerp(touchpadTouchXMin.localRotation, touchpadTouchXMax.localRotation, touchpadNormalized.x);

            touchpadTouchY.transform.localPosition = Vector3.Lerp(touchpadTouchYMax.localPosition, touchpadTouchYMin.localPosition, touchpadNormalized.y);
            touchpadTouchY.transform.localRotation = Quaternion.Lerp(touchpadTouchYMax.localRotation, touchpadTouchYMin.localRotation, touchpadNormalized.y);

            lastTouchpadPosition = newTouchpadPosition;
        }
    }

    private void SetLocalPositionAndRotation(GameObject buttonGameObject, Transform newTransform)
    {
        buttonGameObject.transform.localPosition = newTransform.localPosition;
        buttonGameObject.transform.localRotation = newTransform.localRotation;
    }
}
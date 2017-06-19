//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Dialogs;
using HUX.Interaction;
using HUX.Receivers;
using System.Collections;
using UnityEngine;

namespace HUX
{
    public class DialogAndMenuExample : InteractionReceiver
    {
        public GameObject DialogPrefab;
        public GameObject[] LaunchDialogButtons;
        public TextMesh Result;

        [Header ("Dialog 1 options")]
        public string Dialog1Title = "Close Dialog";
        [TextArea]
        public string Dialog1Message = "This is a message for dialog 1.";
        SimpleDialog.ButtonTypeEnum Dialog1Button = SimpleDialog.ButtonTypeEnum.Close;


        [Header("Dialog 2 options")]
        public string Dialog2Title = "Yes No Dialog";
        [TextArea]
        public string Dialog2Message = "This is a message for dialog 2. Longer messages will be wrapped automatically. However you still need to be aware of overflow.";
        SimpleDialog.ButtonTypeEnum Dialog2Button1 = SimpleDialog.ButtonTypeEnum.Yes;
        SimpleDialog.ButtonTypeEnum Dialog2Button2 = SimpleDialog.ButtonTypeEnum.No;

        /*public string Dialog3Title = "Yes No Cancel Dialog";
        [TextArea]
        public string Dialog3Message = "This is a message for dialog 3. Longer messages will be wrapped automatically. However you still need to be aware of overflow.";
        SimpleDialog.ButtonTypeEnum Dialog3Button1 = SimpleDialog.ButtonTypeEnum.Yes;
        SimpleDialog.ButtonTypeEnum Dialog3Button2 = SimpleDialog.ButtonTypeEnum.No;
        SimpleDialog.ButtonTypeEnum Dialog3Button3 = SimpleDialog.ButtonTypeEnum.Cancel;
        */

        protected bool launchedDialog;

        protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            base.OnTapped(obj, eventArgs);
            if (launchedDialog)
                return;

            SimpleDialog.ButtonTypeEnum buttons = SimpleDialog.ButtonTypeEnum.Close;
            string title = string.Empty;
            string message = string.Empty;

            switch (obj.name)
            {
                case "Dialog1":
                default:
                    title = Dialog1Title;
                    message = Dialog1Message;
                    buttons = Dialog1Button;
                    break;

                case "Dialog2":
                    title = Dialog2Title;
                    message = Dialog2Message;
                    buttons = Dialog2Button1 | Dialog2Button2;
                    break;

                /*case "Dialog3":
                    title = Dialog3Title;
                    message = Dialog3Message;
                    buttons = Dialog3Button1 | Dialog3Button2 | Dialog3Button3;
                    break;*/
            }

            launchedDialog = true;
            StartCoroutine(LaunchDialogOverTime(buttons, title, message));
        }

        protected IEnumerator LaunchDialogOverTime(SimpleDialog.ButtonTypeEnum buttons, string title, string message)
        {
            // Disable all our buttons
            foreach (GameObject buttonGo in Interactibles)
            {
                buttonGo.SetActive(false);
            }
            Result.gameObject.SetActive(false);

            SimpleDialog dialog = SimpleDialog.Open(DialogPrefab, buttons, title, message);
            dialog.OnClosed += OnClosed;

            // Wait for dialog to close
            while (dialog.State != SimpleDialog.StateEnum.Closed)
            {
                yield return null;
            }

            // Enable all our buttons
            foreach (GameObject buttonGo in Interactibles)
            {
                buttonGo.SetActive(true);
            }
            Result.gameObject.SetActive(true);
            launchedDialog = false;
            yield break;
        }

        protected void OnClosed(SimpleDialogResult result)
        {
            Result.text = "Dialog result: " + result.Result.ToString();
        }
    }
}

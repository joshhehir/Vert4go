using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FPSController
{
    public class InputWindow : MonoBehaviour
    {
        private static InputWindow instance;

        private Button okButton;
        private Button cancelButton;
        private TextMeshProUGUI titleText;
        private TMP_InputField inputField;

        private void Awake()
        {
            instance = this;

            okButton = transform.Find("okButton").GetComponent<Button>();
            cancelButton = transform.Find("cancelButton").GetComponent<Button>();
            titleText = transform.Find("titleText").GetComponent<TextMeshProUGUI>();
            inputField = transform.Find("inputField").GetComponent<TMP_InputField>();

            Hide();
        }

        private void Show(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();

            titleText.text = titleString;

            inputField.characterLimit = characterLimit;
            inputField.onValidateInput = (string text, int charIndex, char addedChar) => {
                return ValidateChar(validCharacters, addedChar);
            };

            inputField.text = inputString;
            inputField.Select();

            okButton.ClickFunc = () => {
                Hide();
                onOk(inputField.text);
            };

            cancelButton.ClickFunc = () => {
                Hide();
                onCancel();
            };
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private char ValidateChar(string validCharacters, char addedChar)
        {
            if (validCharacters.IndexOf(addedChar) != -1)
            {
                // Valid
                return addedChar;
            }
            else
            {
                // Invalid
                return '\0';
            }
        }

        public static void Show_Static(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk)
        {
            instance.Show(titleString, inputString, validCharacters, characterLimit, onCancel, onOk);
        }

       /* public static void Show_Static(string titleString, int defaultInt, Action onCancel, Action<int> onOk)
        {
            instance.Show(titleString, defaultInt.ToString(), "0123456789-", 10, onCancel,
                (string inputText) => {
                // Try to Parse input string
                if (int.TryParse(inputText, out int _i))
                    {
                        onOk(_i);
                    }
                    else
                    {
                        onOk(defaultInt);
                    }
                }
            );
        }*/
    }
}



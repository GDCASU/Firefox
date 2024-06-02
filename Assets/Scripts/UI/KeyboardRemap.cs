﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerInput;

public class KeyboardRemap : MonoBehaviour
{
    public PlayerButton action;
    KeyCode button;
    int index;
    public string keyName;
    bool remapping;
    public Text textUI;

    private void Start()
    {
        InitiateButton();
    }

    private void OnDisable()
    {
        textUI.text = keyName;
        SetRemapping(false);
    }

    public void Update()
    {
        if (remapping)
        {
            textUI.text = "";
            if (Input.anyKey)
            {
                foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(vKey) && !DoesKeybindExist(vKey))
                    {
                        SetButton(vKey);
                        SetRemapping(false);
                    }
                }
            }
        }
    }

    public void SetRemapping(bool _remapping)
    {
        remapping = _remapping;
    }
    public void InitiateButton()
    {
        button = InputManager.allKeybinds[InputManager.InputMode.keyboard][action];
        SetKeyName(button);
        textUI.text = keyName;
    }

    private void SetKeyName(KeyCode newKey)
    {
        if (InputManager.mouseButtonToNameMap.ContainsKey(newKey))
        {
            keyName = InputManager.mouseButtonToNameMap[newKey];
        }
        else
        {
            keyName = newKey.ToString();
        }
    }

    private bool DoesKeybindExist(KeyCode key)
    {
        var allBinds = InputManager.allKeybinds[InputManager.InputMode.keyboard];
        bool isCurrKey = allBinds[action] == key;
        bool isBoundToOtherAction = allBinds.ContainsValue(key);
        return isBoundToOtherAction && !isCurrKey;
    }
    public void SetButton(KeyCode passed)
    {
        List<string> keyboardCodes = PauseMenu.singleton.keyboardCodes;

        InputManager.allKeybinds[InputManager.InputMode.keyboard][action] = passed;
        keyboardCodes.Remove(keyName);
        SetKeyName(passed);
        keyboardCodes.Add(keyName);
        GetComponentInChildren<Text>().text = keyName;
        InputManager.SaveKeybinds();
    }

}

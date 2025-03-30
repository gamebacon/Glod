using System;
using UnityEngine;

public class ControlManager : MonoBehaviour
{

    [SerializeField] private ControlMapper _controlMapper;

    // store dictionary for better performance.

    public KeyCode GetKey(PlayerAction action)
    {
        Control control = _controlMapper.controls.Find(control => control.action == action);
        // Debug.Log(control.ToString());
        return control.key;
    }

    internal string GetKeyString(PlayerAction action)
    {
        KeyCode key = GetKey(action);
        return key.ToString();
    }
}

[System.Serializable]
public class Control
{
    public PlayerAction action; 
    public KeyCode key; 

    public Control(PlayerAction action, KeyCode key)
    {
        this.action = action;
        this.key = key;
    }

    public string ToString()
    {
        return $"{action.ToString()} {key.ToString()}";  
    }
}


public enum PlayerAction
{
    NONE,
    INTERACT,
    DROP_ITEM,
    ATTACK,
} 

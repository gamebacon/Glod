using System;
using UnityEngine;
using UnityEngine.UI;

public class HotBarItem : MonoBehaviour
{
    [SerializeField] public GameObject carretImage;
    [SerializeField] public int index;
    [SerializeField] public KeyCode keycode;

    public void SetActive(bool active)
    {
        carretImage.SetActive(active);
    }



}

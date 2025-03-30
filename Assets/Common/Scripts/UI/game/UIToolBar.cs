using System;
using System.Collections.Generic;
using UnityEngine;

public class UIToolBar : MonoBehaviour
{

    [SerializeField] public List<HotBarItem> items;
    public int activeIndex = 0;

    private void Start()
    {
        SetActiveIndex(activeIndex);
    }

    private void Update()
    {
        foreach (HotBarItem item in items)
        {
            if (Input.GetKeyDown(item.keycode))
            {
                SetActiveIndex(item.index);
            } 
        }

        float mouse = Input.GetAxis("Mouse ScrollWheel");

        if(mouse > 0)
        {
            int index = activeIndex == items.Count - 1 ? 0 : activeIndex + 1;
            SetActiveIndex(index);
        } else if (mouse < 0)
        {
            int index = activeIndex == 0 ? items.Count - 1 : activeIndex - 1;
            SetActiveIndex(index);
        }
    }


    private void SetActiveIndex(int index)
    {
        items[activeIndex].SetActive(false);
        items[index].SetActive(true);
        activeIndex = index;
        AudioManager.GetInstance().Play(SoundType.UI_SELECT);
    }

}

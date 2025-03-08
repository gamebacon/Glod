using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{

    public void OnPointerEnter(PointerEventData ped)
    {
        AudioManager.GetInstance().Play(SoundType.UI_SELECT);
    }

    public void OnPointerDown(PointerEventData ped)
    {
        // AudioManager.GetInstance().Play(SoundType.UI_CLICK);
    }
}


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{

    public void OnPointerEnter(PointerEventData ped)
    {
        AudioManager.Instance.Play(SoundType.PLAYER_FOOTSTEP);
    }

    public void OnPointerDown(PointerEventData ped)
    {
        AudioManager.Instance.Play(SoundType.ENTITY_HIT_TREE);
    }
}


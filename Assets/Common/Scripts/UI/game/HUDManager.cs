using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private List<StatSlider> statSliders;
    [SerializeField] private UIActionHelper actionHelper;
    [SerializeField] private ControlManager _controlManager;


    private void Start()
    {
        _controlManager = GameManager.GetInstance().controlManager;

        Hand hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();

        hand.OnEqiupItem += HandleEqiupItem;
        hand.OnDropItem += HandleDropItem;
    }


    public void SetSliderValue(PlayerStatType type, float value)
    {
        StatSlider statSlider = statSliders.Find(slider => slider.type == type);

        if (statSlider.GetValue() != value) {
            statSlider.SetValue(value);
        }
    }


    void HandleEqiupItem(Item item)
    {
        actionHelper.SetVisible(true);

        actionHelper.SetText(
            $"[{_controlManager.GetKeyString(PlayerAction.ATTACK)}]  use",
            $"[{_controlManager.GetKeyString(PlayerAction.DROP_ITEM)}]  drop"
        );
    }

    void HandleDropItem(Item item)
    {
        actionHelper.SetVisible(false);
    }


    [Serializable]
    public class StatSlider
    {
        [SerializeField] private Slider slider;
        public PlayerStatType type;

        public float GetValue()
        {
            return slider.value;
        }
        public void SetValue(float value)
        {
            slider.value = value;
        }

    }

}

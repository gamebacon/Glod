using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private List<StatSlider> statSliders;

    public void SetSliderValue(PlayerStatType type, float value)
    {
        StatSlider statSlider = statSliders.Find(slider => slider.type == type);

        if (statSlider.GetValue() != value) {
            statSlider.SetValue(value);
        }


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

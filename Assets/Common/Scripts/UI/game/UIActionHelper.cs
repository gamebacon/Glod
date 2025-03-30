using TMPro;
using UnityEngine;

public class UIActionHelper : MonoBehaviour {

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text text1;
    [SerializeField] private TMP_Text text2;

    private void Start()
    {
        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        panel.SetActive(visible);
    }


    public void SetText(string text1 = "", string text2 = "")
    {
        this.text1.text = text1;
        this.text2.text = text2;

    }
}
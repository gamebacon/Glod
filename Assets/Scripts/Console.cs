using UnityEngine;
using TMPro;

public class Console : MonoBehaviour
{
    public TMP_InputField consoleText;
    public static Console instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMessage(string msg) {
        consoleText.text += "\n" + msg;
    }

}

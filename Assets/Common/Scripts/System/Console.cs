using UnityEngine;
using TMPro;

public class Console : MonoBehaviour
{
    public TMP_InputField consoleText;
    public static Console instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void AddMessage(string msg) {
        instance.consoleText.text += "\n" + msg;
    }

}

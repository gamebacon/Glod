using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class DebugNet : MonoBehaviour
{
    public TextMeshProUGUI fps;
    public GameObject console;

    [SerializeField]
    public bool isOn = false;

    private bool fpsOn = true;
    private bool speedOn = true;
    private bool pingOn = true;
    private bool bandwidthOn = true;

    private float deltaTime;
    public static List<string> messages = new List<string>();
    public static DebugNet Instance;

    private float byteUp;
    private float byteDown;
    private float packetsSent;
    private float packetsReceived;

    private void Start()
    {
        Instance = this;
        InvokeRepeating(nameof(UpdateBandwidth), 1f, 1f);
    }

    public void ToggleConsole()
    {
        console.SetActive(!console.activeSelf);
    }

    private void Update()
    {
        if (isOn) {
            DisplayFps();
        }
    }

    private void DisplayFps()
    {
        if (!fpsOn && !speedOn && !pingOn && !bandwidthOn)
        {
            if (fps.enabled)
                fps.gameObject.SetActive(false);
            return;
        }

        if (!fps.gameObject.activeSelf)
            fps.gameObject.SetActive(true);

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float ms = deltaTime * 1000f;
        float fpsValue = 1f / deltaTime;

        var displayText = new System.Text.StringBuilder();

        if (fpsOn)
            displayText.AppendFormat("{0:0.0} ms ({1:0.} fps)", ms, fpsValue);

        if (pingOn)
        {
            int ping = NetStatus.GetPing();
            string pingColor = ping < 60 ? "green" : ping < 100 ? "yellow" : "red";
            displayText.AppendFormat("\n<color={0}>ping: {1}ms</color>", pingColor, ping);
        }

        if (bandwidthOn)
        {
            displayText.AppendFormat("\nbyte up/s: {0}", byteUp);
            displayText.AppendFormat("\nbyte down/s: {0}", byteDown);
            displayText.AppendFormat("\npacket up/s: {0}", packetsSent);
            displayText.AppendFormat("\npacket down/s: {0}", packetsReceived);
        }

        uint totalAllocated = Profiler.GetTotalAllocatedMemory() / 1048576U;
        uint totalReserved = Profiler.GetTotalReservedMemory() / 1048576U;

        displayText.AppendFormat("\nRAM Total: {0} MB | {1} MB / {2} MB", totalAllocated, totalReserved, SystemInfo.systemMemorySize);
        Friend friend = new Friend(LocalClient.instance.serverHost.Value);
        displayText.AppendFormat("\nServer Host: {0} ({1})", LocalClient.instance.serverHost, "fName");
        displayText.AppendFormat("\nMy Server ID: {0}", LocalClient.instance.myId);
        displayText.AppendFormat("\nServer Owner: {0}", LocalClient.serverOwner);

        fps.text = displayText.ToString();
    }

    private void UpdateBandwidth()
    {
        byteUp = ClientSend.bytesSent;
        byteDown = LocalClient.byteDown;
        packetsSent = ClientSend.packetsSent;
        packetsReceived = LocalClient.packetsReceived;

        ClientSend.bytesSent = 0;
        ClientSend.packetsSent = 0;
        LocalClient.byteDown = 0;
        LocalClient.packetsReceived = 0;
    }

    public void OpenConsole()
    {
        console.SetActive(true);
    }

    public void CloseConsole()
    {
        console.SetActive(false);
    }
}

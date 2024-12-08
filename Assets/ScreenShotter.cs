using UnityEngine;

public class ScreenShotter : MonoBehaviour
{
    [SerializeField]
    private bool isActive = true;

    [SerializeField]
    private KeyCode screenshotKey = KeyCode.F12; // Default key to F12, but can be changed in the Inspector.

    private void Update()
    {
        if (isActive && Input.GetKeyDown(screenshotKey)) // Check for the screenshot key press
        {
            string folderPath = "Assets/Screenshots/";

            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            var screenshotName =
                "Screenshot_" +
                System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") +
                ".png";
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 2);
            Debug.Log("Screenshot saved: " + System.IO.Path.Combine(folderPath, screenshotName));
        }
    }
}


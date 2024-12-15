using UnityEngine;
using TMPro;
using UnityEngine.Rendering.UI;


[RequireComponent(typeof(FacePlayer))]
public class EntityCanvas : MonoBehaviour
{
    [SerializeField]
    private TMP_Text idText;
    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private GameObject canvasParent;

    private Transform playerTransform;

    void Awake() {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    void Update () {
        if (!playerTransform) {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        } else {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance > 50) {
                canvasParent.SetActive(false);
            } else {
                canvasParent.SetActive(true);
            }
        }
    }

    public void SetId(int id) {
        idText.text = $"ID: {id}";
    }

    public void SetHealth(int currentHealth, int maxHealth) {
        healthText.text = $"{currentHealth}/{maxHealth}";
    }

}

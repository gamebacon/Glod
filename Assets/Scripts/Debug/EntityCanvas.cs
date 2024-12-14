using UnityEngine;
using TMPro;


[RequireComponent(typeof(FacePlayer))]
public class EntityCanvas : MonoBehaviour
{
    [SerializeField]
    private TMP_Text idText;
    [SerializeField]
    private TMP_Text healthText;

    void Awake() {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    public void SetId(int id) {
        idText.text = $"ID: {id}";
    }

    public void SetHealth(int currentHealth, int maxHealth) {
        healthText.text = $"{currentHealth}/{maxHealth}";
    }

}

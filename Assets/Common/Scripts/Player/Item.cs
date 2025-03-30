using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Interactable))]
public class Item : MonoBehaviour
{
    private Transform position;
    public ItemInfo itemInfo;

    [HideInInspector] public Rigidbody rb; 
    [HideInInspector] public Collider col;

    void Start()
    {
        GetComponent<MeshCollider>().convex = true;

        if (itemInfo == null)
        {
            Debug.LogWarning($"Iteminfo for item {gameObject.name} is not assigned");
        }

        gameObject.layer = LayerMask.NameToLayer("Interactable");

        rb = GetComponent<Rigidbody>();

        if (gameObject.TryGetComponent<MeshCollider>(out MeshCollider mc))
        {
            col = mc;
        } else
        {
            col = gameObject.GetComponent<Collider>();
        }
    }

    public ItemType GetItemType()
    {
        return itemInfo.GetItemType();
    }
}

using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Interactable))]
public class HandItem : MonoBehaviour
{
    private Transform position; 
    [HideInInspector] public Rigidbody rb; 
    [HideInInspector] public Collider col;


    [SerializeField] public Vector3 handPosition;
    [SerializeField] public Vector3 handRotation;

    private Transform handPosRef;
    [SerializeField] private ItemType type;

    void Start()
    {
        GetComponent<MeshCollider>().convex = true;
        GetComponent<Interactable>().OnInteract += HandleEqiup;
        gameObject.layer = LayerMask.NameToLayer("Interactable");

        rb = GetComponent<Rigidbody>();

        if (gameObject.TryGetComponent<MeshCollider>(out MeshCollider mc))
        {
            col = mc;
        } else
        {
            col = gameObject.GetComponent<Collider>();
        }

        handPosRef = GameObject.FindGameObjectWithTag("HandPosRef").transform;
    }


    private void OnDestroy()
    {
      GetComponent<Interactable>().OnInteract -= HandleEqiup;
    }

    public ItemType GetItemType()
    {
        return type;
    }

    private void HandleEqiup()
    {
        Hand hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
        hand.EqiupItem(this);
    }

    public void SetPos(Vector3 pos, Vector3 rot)
    {
        handPosition = pos;
        handRotation = rot;
    }







}

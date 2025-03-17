using UnityEngine;

public class Hand : MonoBehaviour
{
    private PlayerInteraction _playerInteraction;

    [SerializeField] private Transform rightHandRef;

    private HandItem heldItem;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Hit()
    {
        _playerInteraction.RaycastAttack();
    } 
    public void EqiupItem(Transform handItem)
    {
        handItem.parent = rightHandRef;
        handItem.position = rightHandRef.position;
        handItem.rotation = rightHandRef.rotation;

        heldItem = handItem.GetComponent<HandItem>();
        heldItem.rb.isKinematic = true;
    }
    public void DropItem()
    {
        if (heldItem == null)
        {
            return;
        }

        heldItem.transform.parent = null;

        heldItem.rb.isKinematic = false;
        heldItem.rb.AddForce(rightHandRef.transform.right * 100);

        heldItem = null;
    }

}

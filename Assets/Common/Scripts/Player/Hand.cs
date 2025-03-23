using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Hand : MonoBehaviour
{
    private PlayerInteraction _playerInteraction;

    [SerializeField] private TwoBoneIKConstraint bone;

    private HandPositioner _handRef;

    private HandItem heldItem;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.tag = "Hand";
        bone.weight = 0;

        _playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
        _handRef = GameObject.FindGameObjectWithTag("HandPosRef").GetComponent<HandPositioner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Hit()
    {
        _playerInteraction.RaycastAttack();
    } 
    public void EqiupItem(HandItem item)
    {
        /*
            Already holding something
        */
        if (heldItem)
        {
            return;
        }

        /*
            Parent and set pos
        */

        _handRef.Position(item);



        /*
            Prepare item props
        */
        item.rb.isKinematic = true;
        item.col.isTrigger = true;

        bone.weight = 1;

        heldItem = item;
    }
    public void DropItem()
    {
        if (heldItem == null)
        {
            return;
        }

        heldItem.transform.parent = null;

        heldItem.rb.isKinematic = false;
        heldItem.rb.AddForce(_handRef.transform.forward * 100);
        heldItem.col.isTrigger = false;

        heldItem = null;
        bone.weight = 0;
    }

}

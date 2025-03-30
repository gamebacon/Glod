using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Hand : MonoBehaviour
{
    private PlayerInteraction _playerInteraction;

    [SerializeField] private TwoBoneIKConstraint bone;

    private HandPositioner _handRef;

    private Item heldItem;

    private Transform _playerTransform;

    public Action<Item> OnEqiupItem;
    public Action<Item> OnDropItem;

    private void Awake()
    {
        gameObject.tag = "Hand";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bone.weight = 0;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _playerTransform = player.transform;
        _playerInteraction = player.GetComponent<PlayerInteraction>();
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
    public void AttemptEquipItem(Item item)
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

        OnEqiupItem?.Invoke(heldItem);
    }
    public void DropItem()
    {
        if (heldItem == null)
        {
            return;
        }

        heldItem.transform.parent = null;

        heldItem.col.isTrigger = false;
        heldItem.rb.isKinematic = false;

        heldItem.rb.AddForce(_playerTransform.transform.forward * 100);

        bone.weight = 0;

        OnDropItem?.Invoke(heldItem);

        heldItem = null;
    }

}

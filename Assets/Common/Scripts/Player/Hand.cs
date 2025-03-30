using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Hand : MonoBehaviour
{
    private PlayerInteraction _playerInteraction;
    public float interactionRange = 3f;      // Range for interacting with objects


    [SerializeField] private TwoBoneIKConstraint bone;

    private HandPositioner _handRef;

    private Item heldItem;

    private Transform _playerTransform;

    public Action<Item> OnEqiupItem;
    public Action<Item> OnDropItem;

    private Transform _camTransform;


    private void Awake()
    {
        gameObject.tag = "Hand";
        _camTransform = Camera.main.transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetBoneWeight(0);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _playerTransform = player.transform;
        _playerInteraction = player.GetComponent<PlayerInteraction>();
        _handRef = GameObject.FindGameObjectWithTag("HandPosRef").GetComponent<HandPositioner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RayCastItem()
    {
        // Detect objects within interaction range
        RaycastHit hit;
        Vector3 interactionDirection = _camTransform.forward;

        // Debug line for interaction range in the Scene view
        Debug.DrawRay(_camTransform.position, interactionDirection * interactionRange, Color.blue, 1.0f);

        // if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _interaction.interactionRange, LayerMask.GetMask("Interactable")))
        if (Physics.Raycast(_camTransform.position, interactionDirection, out hit, interactionRange, LayerMask.GetMask("Interactable")))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            Item item = hit.collider.GetComponent<Item>();

            if (interactable != null)
            {
                interactable.Interact();  // Call Interact on Interactable script
            }
            if (item != null)
            {
                AttemptEquipItem(item);
            }
        }
        else
        {
            Debug.Log("Interaction failed - no interactable object in range");
        }
    }


    public void Hit()
    {
        _playerInteraction.RaycastAttack();
    } 
    private void AttemptEquipItem(Item item)
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

        SetBoneWeight(1);

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

        SetBoneWeight(0);

        OnDropItem?.Invoke(heldItem);

        heldItem = null;
    }


    private void SetBoneWeight(float weight)
    {
        return;
        this.bone.weight = weight;
    }

}

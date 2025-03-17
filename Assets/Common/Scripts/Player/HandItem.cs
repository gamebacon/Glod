using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Interactable))]
public class HandItem : MonoBehaviour
{
    [SerializeField] private Transform position; 
    public Rigidbody rb; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        GetComponent<Interactable>().OnInteract += HandleEqiup;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
      GetComponent<Interactable>().OnInteract -= HandleEqiup;
    }

    private void HandleEqiup()
    {
        Debug.Log("Interact handItem!!");

        Hand hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
        hand.EqiupItem(this.transform);
        /*
        Transform pos = hand.transform;
        SetPosition(pos);
        */
    }



}

using UnityEngine;

public class Hand : MonoBehaviour
{
    private PlayerInteraction _playerInteraction;

    [SerializeField] private Transform rightHandRef;


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
    }

}

using UnityEngine;

public class Hand : MonoBehaviour
{
    private PlayerInteraction _playerInteraction; 

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
        Debug.Log("Hand!!!!!!");
        _playerInteraction.RaycastAttack();
    } 
}

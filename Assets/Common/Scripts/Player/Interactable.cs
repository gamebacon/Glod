using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact()
    {
        // Define custom interaction logic in derived classes
        Debug.Log("Interacting with " + gameObject.name);
    }
}

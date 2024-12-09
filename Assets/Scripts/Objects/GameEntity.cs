using UnityEngine;

public class GameEntity : MonoBehaviour
{

    public int id;
    // Initialize the object with an ID
    public void Initialize(int uniqueId)
    {
        id = uniqueId;
    }

    // Optional: Cleanup when the object is destroyed
    private void OnDestroy()
    {
        ObjectManager.RemoveObject(id);
    }
}

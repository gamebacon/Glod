using UnityEngine;

public class GameEntity : MonoBehaviour
{

    public int id;
    // Initialize the object with an ID
    public EntityCanvas entityCanvas;


    public void Initialize(int uniqueId)
    {
        id = uniqueId;

        entityCanvas.SetId(uniqueId);
    }

    // Optional: Cleanup when the object is destroyed
    private void OnDestroy()
    {
        ObjectManager.GetInstance().RemoveObject(id);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static Dictionary<int, GameEntity> myObjects = new Dictionary<int, GameEntity>();
    private static int idCounter = 0; // Counter for unique IDs

    public static ObjectManager instance;

    public List<GameObject> debugSpawnList = new List<GameObject>();
    public Transform debugSpawnPos;
    public GameObject entityCanvasPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
           Destroy(instance);
        }

        foreach (var obj in debugSpawnList)
        {
            SpawnObject(obj, debugSpawnPos.position);
        }

    }

    // Method to instantiate and register an object
    public GameObject SpawnObject(GameObject obj, Vector3 position)
    {
        Debug.Log("spawn obj");
        // Instantiate the object
        GameObject gameObject = Instantiate(obj, position, Quaternion.identity);

        // Add EntityCanvas
         // Instantiate(entityCanvasPrefab, gameObject);
         EntityCanvas entityCanvas = Instantiate(entityCanvasPrefab, Vector3.zero, Quaternion.identity, gameObject.transform).GetComponent<EntityCanvas>();

        // Add the MyGameObject component
        GameEntity entity = gameObject.AddComponent<GameEntity>();
        entity.entityCanvas = entityCanvas;

        // Assign a unique ID and add it to the dictionary
        int newId = GenerateUniqueId();
        entity.Initialize(newId);
        myObjects.Add(newId, entity);

        return gameObject;
    }

    // Generate a unique ID for new objects
    private int GenerateUniqueId()
    {
        return idCounter++;
    }

    // Optional: Remove object from dictionary when destroyed
    public static void RemoveObject(int id)
    {
        if (myObjects.ContainsKey(id))
        {
            myObjects.Remove(id);
        }
    }

    internal void Damage(int damage, int entityId)
    {
        GameEntity entity = myObjects[entityId];
        entity.GetComponent<Hittable>().TakeDamage(damage);
    }
}


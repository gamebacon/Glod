using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public Dictionary<int, GameEntity> myObjects;
    private int idCounter = 0; // Counter for unique IDs

    private static ObjectManager instance;

    public List<GameObject> debugSpawnList = new List<GameObject>();
    public Transform debugSpawnPos;
    public GameObject entityCanvasPrefab;

    public int id;

    [SerializeField] private Transform parentTransform;



    private void Awake()
    {
        foreach (var obj in debugSpawnList)
        {
            SpawnObject(obj, debugSpawnPos.position);
        }

        id = UnityEngine.Random.Range(0, 1000);
        myObjects = new Dictionary<int, GameEntity>();

    }

    private void OnEnable()
    {
        if (myObjects == null)
        {
            myObjects = new Dictionary<int, GameEntity>();
            RestoreObjects();
        }
    }

    private void RestoreObjects()
    {
        foreach(Transform transform in parentTransform)
        {
            GameEntity entity = transform.GetComponent<GameEntity>();
            myObjects.Add(entity.id, entity);
        }
    }

    public static ObjectManager GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindFirstObjectByType<ObjectManager>();
        }

        return instance;
    }

    // Method to instantiate and register an object
    public GameObject SpawnObject(GameObject obj, Vector3 position)
    {
        // Instantiate the object
        GameObject gameObject = Instantiate(obj, position, Quaternion.identity, parentTransform);

        // Add EntityCanvas
         EntityCanvas entityCanvas = Instantiate(
             entityCanvasPrefab,
             Vector3.zero,
             Quaternion.identity,
             gameObject.transform
         ).GetComponent<EntityCanvas>();

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
    public void RemoveObject(int id)
    {
        if (myObjects.ContainsKey(id))
        {
            myObjects.Remove(id);
        }

    }

    internal void Damage(float damage, int entityId)
    {
        GameEntity entity = myObjects[entityId];
        entity.GetComponent<Hittable>().TakeDamage(damage);
    }
}


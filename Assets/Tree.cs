using UnityEngine;

public class TreeProps : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Set a random scale between a minimum and maximum value
        float randomScale = Random.Range(0.1f, 10.2f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        // Set a random rotation on the Y-axis (useful for trees or other objects with symmetry on the vertical axis)
        float randomYRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

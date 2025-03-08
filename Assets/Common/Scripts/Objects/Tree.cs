using UnityEngine;

[RequireComponent(typeof(Hittable))]
public class Tree : MonoBehaviour
{
       private Hittable hittable;
    // Start is called before the first frame update
    void Start()
    {
        /*
        // Set a random scale between a minimum and maximum value
        float randomScale = Random.Range(.5f, 2.5f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        */

        // Set a random rotation on the Y-axis (useful for trees or other objects with symmetry on the vertical axis)
        float randomYRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);

        // Get the Hittable component and subscribe to the Die event
        hittable = GetComponent<Hittable>();
        hittable.OnDie += HandleTreeFall;
        hittable.OnDamage += OnDamage;
    }

    private void OnDamage () {
        AudioManager.Instance.Play("WoodHit");
    }

    private void HandleTreeFall()
    {
        AudioManager.Instance.Play("TreeFalling");
        // Add Rigidbody to make the tree fall
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        // Optionally set Rigidbody properties for a realistic fall
        rb.mass = 25f;
        rb.angularDamping = 2.5f;

        // Apply a small random torque to simulate a natural fall
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-1f, 1f)
        );
        rb.AddTorque(randomTorque * 100f);

/*
        // Disable the collider to avoid interactions
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        */

        // Optionally disable other components or scripts related to the tree
        Tree treeProps = GetComponent<Tree>();
        if (treeProps != null)
        {
            treeProps.enabled = false;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the Die event to avoid memory leaks
        hittable.OnDie -= HandleTreeFall;
    }
}

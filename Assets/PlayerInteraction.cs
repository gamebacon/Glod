using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float attackRange = 2f;           // Range for melee attacks
    public int attackDamage = 20;            // Damage dealt per attack
    public float interactionRange = 3f;      // Range for interacting with objects
    public float attackCooldown = 1f;        // Time between attacks

    private float _lastAttackTime;           // To track time since last attack

    void Update()
    {
        // Handle melee attack
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            AttemptAttack();
        }

        // Handle interactions
        if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to interact
        {
            AttemptInteraction();
        }
    }

    void AttemptAttack()
    {
        Debug.Log("Attempting attack");

        // Check if attack cooldown has passed
        if (Time.time - _lastAttackTime < attackCooldown)
        {
            Debug.Log("Attack on cooldown");
            return;
        }

        // Update the last attack time
        _lastAttackTime = Time.time;

        // Detect objects within attack range
        RaycastHit hit;
        Vector3 attackDirection = transform.forward;

        // Debug line for attack range in the Scene view
        Debug.DrawRay(transform.position, attackDirection * attackRange, Color.red, 1.0f);

        if (Physics.Raycast(transform.position, attackDirection, out hit, attackRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            // Check if the object has a Hittable component
            Hittable hittable = hit.collider.GetComponent<Hittable>();
            if (hittable != null)
            {
                Debug.Log("Hitting: " + hittable.gameObject.name);
                hittable.TakeDamage(attackDamage);  // Call TakeDamage on Hittable script
                // Play attack animation or sound (if applicable)
                // Animator.SetTrigger("Attack"); // Example
            }
            else
            {
                Debug.Log("Hit an object, but it is not hittable: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("Attack missed - no object in range");
        }
    }

    void AttemptInteraction()
    {
        Debug.Log("Attempting interaction");

        // Detect objects within interaction range
        RaycastHit hit;
        Vector3 interactionDirection = transform.forward;

        // Debug line for interaction range in the Scene view
        Debug.DrawRay(transform.position, interactionDirection * interactionRange, Color.blue, 1.0f);

        if (Physics.Raycast(transform.position, interactionDirection, out hit, interactionRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            // Check if the object has an Interactable component
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.Log("Interacting with: " + interactable.gameObject.name);
                interactable.Interact();  // Call Interact on Interactable script
            }
            else
            {
                Debug.Log("Hit an object, but it is not interactable: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("Interaction failed - no interactable object in range");
        }
    }

    // Optional: Visualize attack and interaction ranges in the Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * interactionRange);
    }
}

using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float attackRange = 2f;           // Range for melee attacks
    public int attackDamage = 20;            // Damage dealt per attack
    public float interactionRange = 3f;      // Range for interacting with objects
    public float attackCooldown = 0f;        // Time between attacks
    public float attackSpeed = 3f;        // Time between attacks

    private float _lastAttackTime;           // To track time since last attack


    [SerializeField]
    private Animator _handAnimator;

    private PlayerStats _playerStats;

    private Transform _camTransform;

    private Hand _hand;

    private ControlManager _controlManager;

    private HUDManager _HUDManager;

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _camTransform = Camera.main.transform;
        _hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
        _controlManager = GameManager.GetInstance().controlManager;
        _HUDManager = FindFirstObjectByType<HUDManager>();
    }

    void Update()
    {
        /*
        if (GameManager.GetInstance().gameState != GameState.Game) {
            return;
        }
        */

        if (Input.GetKeyDown(_controlManager.GetKey(PlayerAction.ATTACK))) 
        {
            _handAnimator.SetTrigger("Swing");
        }
        if (Input.GetKeyDown(_controlManager.GetKey(PlayerAction.INTERACT)))
        {
            _handAnimator.SetTrigger("Grab");
        }
        if (Input.GetKeyDown(_controlManager.GetKey(PlayerAction.DROP_ITEM)))
        {
            _handAnimator.SetTrigger("Throw");
        }


    }

    void AttemptAttack()
    {

        // Check if attack cooldown has passed
        if (Time.time - _lastAttackTime < attackCooldown)
        {
            return;
        }

        /*
        _handAnimator.SetFloat("AttackSpeed", attackSpeed);
        _handAnimator.SetTrigger("Swing");
        */

        // Update the last attack time
        _lastAttackTime = Time.time;

    }

    public void RaycastAttack()
    {
        // Detect objects within attack range
        RaycastHit hit;
        Vector3 attackDirection = _camTransform.forward;

        // Debug line for attack range in the Scene view
        Debug.DrawRay(_camTransform.position, attackDirection * attackRange, Color.red, 1.0f);

        if (Physics.Raycast(_camTransform.position, attackDirection, out hit, attackRange))
        {

            // Check if the object has a Hittable component
            Hittable hittable = hit.collider.GetComponent<Hittable>();
            if (hittable != null)
            {
                GameEntity gameEntity = hit.collider.GetComponent<GameEntity>();

                HandleAttack(gameEntity);

                // hittable.TakeDamage(attackDamage);  // Call TakeDamage on Hittable script
                // Play attack animation or sound (if applicable)
                // Animator.SetTrigger("Attack"); // Example
            }
            else
            {
                 Debug.Log("Hit an object, but it is not hittable: " + hit.collider.gameObject.name);
            }
        }

    }

    public float GetAttackDamage()
    {
        float critMultiplier = UnityEngine.Random.Range(1f, 1.5f);
        float finalDamage = attackDamage * critMultiplier;
        return finalDamage;
    }

    private void HandleAttack (GameEntity entity)
    {
        if (GameManager.GetInstance().isSinglePlayer)
        {
            ObjectManager.GetInstance().Damage(GetAttackDamage(), entity.id);
        }
        else
        {
            ClientSend.AttackEntity(entity.id);
        }
    }



    void AttemptDropItem ()
    {
        _hand.DropItem();
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

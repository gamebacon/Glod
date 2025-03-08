using UnityEngine;

[RequireComponent(typeof(Hittable))]
public class Rock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Hittable hittable;

    void Start()
    {
        hittable = GetComponent<Hittable>();
        // hittable.OnDie += HandleTreeFall;
        hittable.OnDamage += OnDamage;
    }

    // Update is called once per frame
    void OnDamage()
    {
        AudioManager.Instance.Play(SoundType.ENTITY_HIT_TREE);
    }


    void HandleDeath() {

    }

}

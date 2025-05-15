using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] private int durability;
    private Enemy_Melee enemy_Melee;


    void Awake()
    {
        enemy_Melee = GetComponentInParent<Enemy_Melee>();
    }
    public void ReduceDurability(int bulletDamge)
    {
        durability -= bulletDamge;
        if (durability <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReduceDurability()
    {
        durability --;
        if (durability <= 0)
        {
            enemy_Melee.anim.SetFloat("ChaseIndex", 0); // Enables the chase animation
            Destroy(gameObject);
        }
    }
}

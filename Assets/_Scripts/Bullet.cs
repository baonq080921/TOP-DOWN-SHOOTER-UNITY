using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce;


    private Player player;
    private BoxCollider cd;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    
    [SerializeField] private GameObject bulletImpactFX;


    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void BulletSetup(float flyDistance, float impactForce)
    {
        this.impactForce = impactForce;
        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = .25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f; // magic number .5f is a length of tip of the laser ( Check method UpdateAimVisuals() on PlayerAim script) ;
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }
    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }
    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime; // magic number 2 is choosen trhou testing
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFx(collision);
        ReturnBulletToPool();

        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        EnemyShield enemyShield = collision.gameObject.GetComponentInParent<EnemyShield>();

        if (enemyShield != null)
        {
            Debug.Log("Hit shield");
            Debug.Log(player.weapon.CurrentWeapon().gunDamage);
            enemyShield.ReduceDurability(player.weapon.CurrentWeapon().gunDamage);
            return;
        }
        //testing puroposes on enemy shield
        // if(enemyShield != null)
        // {
        //     enemyShield.ReduceDurability();
        //     return;
        // }


        if (enemy != null)
        {
            Vector3 force = rb.linearVelocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;
            enemy.GetHit(player.weapon.CurrentWeapon().gunDamage);
            enemy.DeathImpact(force, collision.contacts[0].point, hitRigidbody);
        }
    }


    private void ReturnBulletToPool() => ObjectPool.instance.ReturnObject(gameObject);


    private void CreateImpactFx(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFx = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFx.transform.position = contact.point;

            ObjectPool.instance.ReturnObject(newImpactFx, 1);
        }
    }
}

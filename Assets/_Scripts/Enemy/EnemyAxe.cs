using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeTransform;
    private float flySpeed;
    private float rotationSpeed;
    private Transform player;
    private Vector3 direction;

    private float timer = 1f;


    public void AxeSetup(Transform player, float flySpeed, float timer)
    {
        this.player = player;
        this.flySpeed = flySpeed;
        this.timer = timer;
        rotationSpeed = 1600f;
    }


    void Update()
    {
        axeTransform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        // Timer to stop the axe move toward the player
        if(timer > 0)
            direction = player.position + Vector3.up - transform.position;

        rb.linearVelocity = direction.normalized * flySpeed;

        transform.forward = rb.linearVelocity;
    }


    void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if(bullet != null || player != null)
        {
            GameObject newFx = ObjectPool.instance.GetObject(impactEffect);
            newFx.transform.position = transform.position;

            ObjectPool.instance.ReturnObject(newFx,1f);
            ObjectPool.instance.ReturnObject(gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{

    [SerializeField] protected int healthPoints = 20;
    [Header("Idle data")]
    public float idleTime;
    public float aggresionRange;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPositions;
    private int currentPatrolIndex;
    

    public bool inBattleMode { get; private set; }


    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

  

    protected virtual void Update()
    {
        
    }

    protected bool ShouldEnterBattleMode()
    {
        bool inAggresionRange = Vector3.Distance(transform.position, player.position) < aggresionRange; ;

        if(inAggresionRange && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }
        return false;
    }
    
    protected virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }


    // public virtual void GetHit(){
    //     healthPoints--;
    // }
    public virtual void GetHit(int damage)
    {
        EnterBattleMode();
        healthPoints -= damage;
    }


    // This function is used to apply force to the enemy when hit by a projectile or other force.
    // It uses a coroutine to delay the application of the force slightly, allowing for smoother physics interactions.
    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb){
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }




     public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }



    #region Animation events
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void AbilityTrigger() => stateMachine.currentState.AbilityTrigger();


    #endregion

    #region Patrol
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPositions[currentPatrolIndex];

        currentPatrolIndex++;
        Debug.Log(destination);


        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }
    private void InitializePatrolPoints()
    {
       patrolPointsPositions = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPositions[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }


    #endregion
   
}

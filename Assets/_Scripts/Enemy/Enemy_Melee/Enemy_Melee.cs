using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 2)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}
public enum AttackType_Melee { Close,Charge}

public enum EnemyMelee_Type
{
    Regular,
    Shield,
    DodgeRoll,
    AxeThrow
}

public class Enemy_Melee : Enemy
{


    private Enemy_Visual enemy_Visual;

    
    #region StateMachine
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }

    public DeadState_Melee deadState { get; private set; }

    public AbilityState_Melee abilityState { get; private set; }

    #endregion

    [Header("Enemy Type")]
    public EnemyMelee_Type meleeType;
    public Transform shieldTransform;
    public float dodgeCoolDown;
    private float lastTimeDodged = -10f;

    [Header("Axe throw ability")]

    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCoolDown;
    private float lastTimeAxeThrown;
    public Transform axeStartPoint;



    [Header("Attack Data")]
    public AttackData attackData;
    public List<AttackData> attackList;

    protected override void Awake()
    {
        base.Awake();
        enemy_Visual = GetComponent<Enemy_Visual>();
        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); // idle animation for place holder, will be changed to dead animation later
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        InitializeSpeciality();
        enemy_Visual.SetUpLook();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    protected override void EnterBattleMode()
    {
        if(inBattleMode)
            return;
            
        base.EnterBattleMode();

        stateMachine.ChangeState(recoveryState);
    }


    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        moveSpeed = moveSpeed * .6f;
        EnableWeaponModel(false);

    }


    private void InitializeSpeciality()
    {

        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            enemy_Visual.SetUpWeaponType(Enemy_MeleeWeaponType.Throw);
        }
        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            enemy_Visual.SetUpWeaponType(Enemy_MeleeWeaponType.OneHand);
        }
    }
    public void EnableWeaponModel(bool active)
    {
        enemy_Visual.cuurrentWeaponModel.gameObject.SetActive(active);
    }

    public override void GetHit(int bulletDamge)
    {
        
        base.GetHit(bulletDamge);
        if(healthPoints  <= 0){
            healthPoints = 0;
            stateMachine.ChangeState(deadState);
        }
    }


    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    public void ActiveDodgeRoll(){

        
        if(meleeType != EnemyMelee_Type.DodgeRoll){
            return;
        }
        if(stateMachine.currentState != chaseState){
            return;
        }
        if(Vector3.Distance(transform.position,player.position) < 2f){
            return;
        }

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge Roll");
        Debug.Log(dodgeAnimationDuration);

        if (Time.time > dodgeCoolDown + dodgeAnimationDuration + lastTimeDodged)
        {
            lastTimeDodged = Time.time;
            anim.SetTrigger("DodgeRoll");
        }

    }


    private float GetAnimationClipDuration(string clipname)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipname)
            {
                return clip.length;
            }
        }

        Debug.Log(clipname + " animation clip not found");
        return 0f;
    }


    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow)
        {
            return false;
        }
        if (Time.time > lastTimeAxeThrown + axeThrowCoolDown)
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }
        return false;
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);

    }
}

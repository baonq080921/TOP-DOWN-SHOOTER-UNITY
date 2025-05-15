using UnityEngine;

public class DeadState_Melee : EnemyState
{

    private Enemy_Melee enemy;
    private Enemy_Ragdoll enemyRagdoll;
    private bool interactionsDisabled;
    public DeadState_Melee(Enemy_Melee enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy= enemyBase as Enemy_Melee;
        enemyRagdoll = enemy.GetComponent<Enemy_Ragdoll>();
    }

    public override void Enter()
    {
        base.Enter();
        interactionsDisabled = false;
        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;
        enemyRagdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Exit()
    {
        base.Exit();
        
    }


    public override void Update()
    {
        base.Update();
            // UNCOMMENT THIS CODE TO DISABLE INTERACTIONS AFTER THE ENEMY IS DEAD
        //DisableInteractionIfShould();

    }

    private void DisableInteractionIfShould()
    {
        if (stateTimer <= 0 && interactionsDisabled == false)
        {
            interactionsDisabled = true;
            enemyRagdoll.RagdollActive(false);
            enemyRagdoll.ColliderActive(false);
            // enemy.gameObject.SetActive(false);
        }
    }
}

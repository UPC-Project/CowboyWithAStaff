using UnityEngine;

public class EnemyMeleeAttackSMB : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<MeleeEnemy>();
        enemy?.Attack();
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<MeleeEnemy>();
        enemy?.OnExitAttackState();
    }
}
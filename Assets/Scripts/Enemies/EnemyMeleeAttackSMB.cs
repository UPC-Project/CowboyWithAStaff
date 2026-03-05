using UnityEngine;

public class EnemyMeleeAttackSMB : StateMachineBehaviour
{
    private Enemy _enemy;
    private bool _hasAttacked;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemy = animator.GetComponent<Enemy>();
        _hasAttacked = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.57f)
        {
            _enemy?.Attack();
            _hasAttacked = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemy?.OnExitAttackState();
    }
}

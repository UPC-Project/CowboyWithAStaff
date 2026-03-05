using UnityEngine;

public class EnemyRangedAttackSMB : StateMachineBehaviour
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
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.29f)
        {
            _enemy?.Attack();
            _hasAttacked = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemy?.OnExitAttackState();
    }
}

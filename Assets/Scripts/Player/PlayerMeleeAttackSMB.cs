using Constants;
using UnityEngine;

public class PlayerMeleeAttackSMB : StateMachineBehaviour
{
    private bool _hasAttacked;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hasAttacked = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.37f)
        {
            var player = animator.GetComponentInParent<Player>();
            player?.MeleeAttack();
            _hasAttacked = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponentInParent<Player>();
        player?.OnExitAttackState(PlayerSkill.MeleeAttack);
    }
}

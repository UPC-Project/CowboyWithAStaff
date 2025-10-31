using UnityEngine;

public class PlayerRangedAttackSMB : StateMachineBehaviour
{
    private bool _hasAttacked;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hasAttacked = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.75f)
        {
            var player = animator.GetComponentInParent<Player>();
            player?.RangedAttack();
            _hasAttacked = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponentInParent<Player>();
        player?.OnExitAttackState(false);
    }
}

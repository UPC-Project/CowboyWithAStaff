using Constants;
using UnityEngine;

public class PlayerBlockSkillSMB : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponentInParent<Player>();
        player?.OnExitAttackState(PlayerSkill.Block);
    }
}

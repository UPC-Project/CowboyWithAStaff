using UnityEngine;

public class FinalBossTransitionSMB : StateMachineBehaviour
{
    private FinalBoss _finalBoss;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _finalBoss = animator.GetComponentInParent<FinalBoss>();
        _finalBoss.invulnerable = true;
        _finalBoss.canMove = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _finalBoss.invulnerable = false;
        _finalBoss.canMove = true;
    }
}

using UnityEngine;

public class FinalBossDeathSMB : StateMachineBehaviour
{
    private FinalBoss _finalBoss;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _finalBoss = animator.GetComponentInParent<FinalBoss>();
        _finalBoss.PlayDeathSound();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var entity = animator.GetComponentInParent<Health>();
        entity?.Death();
    }
}

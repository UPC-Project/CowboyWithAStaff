using UnityEngine;

public class DeathSMB : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var entity = animator.GetComponentInParent<Health>();
        entity?.Death();
    }
}

using UnityEngine;

public class EnemyDeathSMB : StateMachineBehaviour
{
    private Enemy _enemy;
    private bool _alphaChanged = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemy = animator.GetComponent<Enemy>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 0.75f && !_alphaChanged)
        {
            _enemy.ChangeAlpha();
            _alphaChanged = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var entity = animator.GetComponentInParent<Health>();
        entity?.Death();
    }
}

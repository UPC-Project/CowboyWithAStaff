using Constants;
using UnityEngine;

public class PlayerBlockSkillSMB : StateMachineBehaviour
{
    private Player _player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = FindAnyObjectByType<Player>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(_player.transform.position, _player.hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Bullet"))
            {
                collider.gameObject.SetActive(false);
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player.OnExitAttackState(PlayerSkill.Block);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class BossFightSceneLogic : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private FinalBoss _finalBoss;
    [SerializeField] private PlayableDirector _director; // Timeline director
    [SerializeField] private Camera _cinematicCamera; // Timeline director
    [SerializeField] private PlayerInput _playerInput;

    private void Start()
    {
        _playerMovement = Player.Instance.gameObject.GetComponent<PlayerMovement>();
        _playerInput = Player.Instance.GetComponent<PlayerInput>();

        _playerMovement.rb.linearVelocity = Vector2.zero;
        _finalBoss.canMove = false; // Boss can't move
        _playerInput.DeactivateInput(); // Disable player input
        _playerMovement.canMove = false;
        Player.Instance.inBossFight = true;

        foreach (var output in _director.playableAsset.outputs)
        {
            if (output.streamName == "Player Animation" || output.streamName == "Player Position")
            {
                _director.SetGenericBinding(output.sourceObject, Player.Instance.gameObject);
            }

            if (output.streamName == "Audio Track")
            {
                _director.SetGenericBinding(output.sourceObject, AudioManager.Instance.sfxSource);
            }
        }
        AudioManager.Instance.StopMusic();
    }

    private void OnEnable()
    {
        _director.stopped += OnTimelineStopped;
    }

    private void OnDisable()
    {
        _director.stopped -= OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        _cinematicCamera.gameObject.SetActive(false); // Disable cinematic camera
        _playerInput.ActivateInput(); // Reactivate player input
        _playerMovement.canMove = true;
        _finalBoss.canMove = true; // Boss can move
        Player.Instance.audioSourceWalk.mute = false;
        AudioManager.Instance.PlayMusic("BossMusic");
    }
}

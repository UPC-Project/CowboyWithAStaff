using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class BossFightSceneLogic : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private FinalBoss _finalBoss;
    [SerializeField] private PlayableDirector _director; // Timeline director
    [SerializeField] private Camera _cinematicCamera; // Timeline director

    private void Start()
    {
        _playerMovement = Player.Instance.gameObject.GetComponent<PlayerMovement>();
        _playerMovement.enabled = false; // Player can't move
        _playerMovement.rb.linearVelocity = Vector2.zero;
        Player.Instance._inBossFight = true;
        _finalBoss.canMove = false; // Boss can't move

        foreach (var output in _director.playableAsset.outputs)
        {
            if (output.streamName == "Player Animation" || output.streamName == "Player Position")
                
            {
                _director.SetGenericBinding(output.sourceObject, Player.Instance.gameObject);
            }
        }
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
        _playerMovement.enabled = true; // Player can move
        _finalBoss.canMove = true; // Boss can move
        _cinematicCamera.gameObject.SetActive(false); // Disable cinematic camera
    }

}

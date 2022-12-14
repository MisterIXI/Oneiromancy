using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    private PlayerSettings _playerSettings;
    private GameManager _gameManager;
    [SerializeField] private Collider _portalCollider;
    [SerializeField] private ParticleSystem _spawnParticles;
    private float _startTime;
    private float _startY;
    private float _targetY;
    private enum state
    {
        Idle,
        Spawning,
        Despawning
    }
    private state animState;
    private void Awake()
    {
        ReferenceManager.PortalDoor = this;
    }

    private void Start()
    {
        _gameManager = ReferenceManager.GameManager;
        _playerSettings = _gameManager.PlayerSettings;
        _gameManager.OnStateChange += this.OnStateChange;
    }
    private void OnDestroy() {
        _gameManager.OnStateChange -= this.OnStateChange;
    }
    private void Update()
    {
        if (animState != state.Idle)
        {
            float t = (Time.time - _startTime) / _playerSettings.PortalAnimDuration;
            _playerSettings.Lerp(t, PlayerSettings.LerpType.EaseInOut);
            float newY = Mathf.Lerp(_startY, _targetY, t);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            if (t >= 1)
            {
                if(animState == state.Spawning)
                    _portalCollider.enabled = true;
                animState = state.Idle;
            }
        }
    }
    public void PlaySpawnAnimation()
    {
        _startTime = Time.time;
        _startY = transform.position.y;
        _targetY = _playerSettings.PortalMaxPosition;
        animState = state.Spawning;
    }


    public void PlayDespawnAnimation()
    {
        if(_playerSettings == null)
            _playerSettings = ReferenceManager.GameManager.PlayerSettings;
        _portalCollider.enabled = false;
        _startTime = Time.time;
        _startY = transform.position.y;
        _targetY = _playerSettings.PortalMinPosition;
        animState = state.Despawning;
    }

    public void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Portal)
        {
            PlaySpawnAnimation();
        }
        else if (oldState == GameManager.GameState.Portal)
        {
            PlayDespawnAnimation();
        }
    }

    private void OnDrawGizmos()
    {
        if (_playerSettings != null && _playerSettings.PortalGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(new Vector3(0, _playerSettings.PortalMinPosition, 0), 3f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(0, _playerSettings.PortalMaxPosition, 0), 3f);
        }
    }
}

using UnityEngine;
using System.Collections;

public abstract class PlayerBase : MonoBehaviour {

    private Player _player;
    public Player Player
    {
        get
        {
            InitializePlayer();
            return _player;
        }
    }

    private PlayerMovement _playerMovement;
    public PlayerMovement PlayerMovement
    {
        get
        {
            InitializePlayerMovement();
            return _playerMovement;
        }
    }

    private PlayerOrientation _playerOrientation;
    public PlayerOrientation PlayerOrientation
    {
        get
        {
            InitializePlayerOrientation();
            return _playerOrientation;
        }
    }

    private PlayerInput _playerInput;
    public PlayerInput PlayerInput
    {
        get
        {
            InitializePlayerInput();
            return _playerInput;
        }
    }
    
    private PlayerProjectileCollision _playerProjectileCollision;
    public PlayerProjectileCollision PlayerProjectileCollision
    {
        get
        {
            InitializePlayerProjectileCollision();
            return _playerProjectileCollision;
        }
    }

    private PlayerShootProjectile _playerShootProjectile;
    public PlayerShootProjectile PlayerShootProjectile
    {
        get
        {
            InitializePlayerShootProjectile();
            return _playerShootProjectile;
        }
    }

    private PhotonView _photonView;
    public PhotonView PhotonView
    {
        get
        {
            InitializePhotonView();
            return _photonView;
        }
    }

    protected virtual void Start()
    {
        InitializePlayer();
        InitializePlayerMovement();
        InitializePlayerOrientation();
        InitializePlayerInput();
        InitializePlayerProjectileCollision();
        InitializePlayerShootProjectile();

        InitializePhotonView();
    }

    private void InitializePlayer()
    {
        if (_player == null)
        {
            _player = GetComponent<Player>();
        }
    }

    private void InitializePlayerMovement()
    {
        if (_playerMovement == null)
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }
    }

    private void InitializePlayerOrientation()
    {
        if (_playerOrientation == null)
        {
            _playerOrientation = GetComponent<PlayerOrientation>();
        }
    }

    private void InitializePlayerInput()
    {
        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }

    private void InitializePlayerProjectileCollision()
    {
        if (_playerProjectileCollision == null)
        {
            _playerProjectileCollision = GetComponent<PlayerProjectileCollision>();
        }
    }

    private void InitializePlayerShootProjectile()
    {
        if (_playerShootProjectile == null)
        {
            _playerShootProjectile = GetComponent<PlayerShootProjectile>();
        }
    }

    private void InitializePhotonView()
    {
        if(_photonView == null)
        {
            _photonView = GetComponent<PhotonView>();
        }
    }

    public virtual void SerializeState(PhotonStream stream, PhotonMessageInfo info) { }
}

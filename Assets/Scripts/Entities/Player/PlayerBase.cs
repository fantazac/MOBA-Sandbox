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

    private PlayerNormalMovement _playerNormalMovement;
    public PlayerNormalMovement PlayerNormalMovement
    {
        get
        {
            InitializePlayerNormalMovement();
            return _playerNormalMovement;
        }
    }

    private PlayerAttackMovement _playerAttackMovement;
    public PlayerAttackMovement PlayerAttackMovement
    {
        get
        {
            InitializePlayerAttackMovement();
            return _playerAttackMovement;
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

    private PlayerMouseSelection _playerMouseSelection;
    public PlayerMouseSelection PlayerMouseSelection
    {
        get
        {
            InitialiseMouseSelection();
            return _playerMouseSelection;
        }
    }

    private PlayerStats _playerStats;
    public PlayerStats PlayerStats
    {
        get
        {
            InitialisePlayerStats();
            return _playerStats;
        }
    }

    private BasicAttack _basicAttack;
    public BasicAttack BasicAttack
    {
        get
        {
            InitialiseBasicAttack();
            return _basicAttack;
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

    private EntityTeam _entityTeam;
    public EntityTeam EntityTeam
    {
        get
        {
            InitializeEntityTeam();
            return _entityTeam;
        }
    }

    protected virtual void Start()
    {
        InitializePlayer();
        InitializePlayerMovement();
        InitializePlayerNormalMovement();
        InitializePlayerAttackMovement();
        InitializePlayerOrientation();
        InitializePlayerInput();
        InitializePlayerProjectileCollision();
        InitializePlayerShootProjectile();
        InitialiseMouseSelection();
        InitialisePlayerStats();
        InitialiseBasicAttack();

        InitializePhotonView();
        InitializeEntityTeam();
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

    private void InitializePlayerNormalMovement()
    {
        if (_playerNormalMovement == null)
        {
            _playerNormalMovement = GetComponent<PlayerNormalMovement>();
        }
    }

    private void InitializePlayerAttackMovement()
    {
        if (_playerAttackMovement == null)
        {
            _playerAttackMovement = GetComponent<PlayerAttackMovement>();
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

    private void InitialiseMouseSelection()
    {
        if(_playerMouseSelection == null)
        {
            _playerMouseSelection = GetComponent<PlayerMouseSelection>();
        }
    }

    private void InitialisePlayerStats()
    {
        if(_playerStats == null)
        {
            _playerStats = GetComponent<PlayerStats>();
        }
    }

    private void InitialiseBasicAttack()
    {
        if(_basicAttack == null)
        {
            _basicAttack = GetComponent<BasicAttack>();
        }
    }

    private void InitializePhotonView()
    {
        if(_photonView == null)
        {
            _photonView = GetComponent<PhotonView>();
        }
    }

    private void InitializeEntityTeam()
    {
        if (_entityTeam == null)
        {
            _entityTeam = GetComponent<EntityTeam>();
        }
        if(_photonView == null)
        {
            InitializePhotonView();
        }
        _entityTeam.SetPhotonView(_photonView);
    }

    public virtual void SerializeState(PhotonStream stream, PhotonMessageInfo info) { }
}

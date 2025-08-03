using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class PlayerMovement : MonoBehaviour
{
    // Takes in damage and position
    public Action<int, Vector2> OnHit;
    public Vector2 FacingDirection => _facingDirection;
    public Vector2 Velocity => _velocity;
    public bool IsInAir => _isInAir;
    public bool IsDead => _isDead;
    public bool IsInvincible => _isInvincibleDamage;
    
    [SerializeField] private int normalDamage;
    [SerializeField] private int dashDamage;
    [SerializeField] private LayerMask invincibilityMask;

    [Header("References")]
    [SerializeField] protected PlatformCollisionTracker ceilingChecker;
    [SerializeField] protected PlatformCollisionTracker groundChecker;
    [SerializeField] protected PlatformCollisionTracker leftWallChecker;
    [SerializeField] protected PlatformCollisionTracker rightWallChecker;
    
    [SerializeField] private ObjectPool shockwavePool;
    
    [SerializeField] private bool isMelee;
    [SerializeField] private ObjectPool projectilePool;
    [SerializeField] private Transform shootOrigin;
    [SerializeField] private TriggerCollisionEvents attackHitBox;
    
    // Read only, for debugging
    [Header("Tracking Variables")]

    [ShowInInspector, ReadOnly] private Vector2 _velocity;

    [ShowInInspector, ReadOnly] private float _lastJumpPressed;
    [ShowInInspector, ReadOnly] private float _lastGroundedTime;
    [ShowInInspector, ReadOnly] private float _lastJumpTime;

    // If the player has released the jump button after jumping
    [ShowInInspector, ReadOnly] private bool _ifReleaseJumpAfterJumping;

    [ShowInInspector, ReadOnly] private bool _isInAir;

    [ShowInInspector, ReadOnly] private float _dashTimeLeft;
    [ShowInInspector, ReadOnly] private float _lastDashTime;
    
    [ShowInInspector, ReadOnly] private float _lastAttackTime;

    // Stops movement if using ability
    [ShowInInspector, ReadOnly] private bool _isDead;
    [ShowInInspector, ReadOnly] private bool _isInvincibleDamage;
    
    private PlayerStats _stats;
    private InputManager _input;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private PlayerVisuals _visuals;
    
    // Only change when there's input
    private Vector2 _facingDirection;
    private bool _isLastFacingRight; // For when _facingDirection.x == 0 && _facingDirection.y == (1 or 0)
    
    private Coroutine _invincibilityCoroutine;
    private WaitForSeconds _invincibilityWait;
    
    // Other variables, all public to save time
    public bool IsAttacking;
    
    #region Public Methods
    public void Init(InputManager input, PlayerStats stats)
    {
        _input = input;
        _stats = stats;
        
        ResetPlayer();
        
        // Reset it, some parts need other stuff assigned (e.g. _input) so it didn't trigger the first time
        OnDisable();
        OnEnable();
        
        _invincibilityWait = new WaitForSeconds(_stats.InvincibilityDuration);
        projectilePool.transform.parent = null;
    }

    public bool IsGrounded() => !_isInAir;

    public void Dash()
    {
        if (Time.time - _lastDashTime < _stats.DashCooldown)
            return;
        
        _lastDashTime = Time.time;
        _dashTimeLeft = _stats.DashTime;
    }

    public void Attack()
    {
        if (Time.time - _lastAttackTime < _stats.AttackCooldown)
            return;
        
        if (isMelee)
            attackHitBox.gameObject.SetActive(true);
        else
        {
            GameObject projectileGO = projectilePool.Get(shootOrigin.position);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            projectile.Init(projectilePool, _facingDirection); 
        }
        IsAttacking = true;
        _lastAttackTime = Time.time;
    }

    public void OnAttackDone()
    {
        attackHitBox.gameObject.SetActive(false);
        IsAttacking = false;
    }

    public void OnAttackHit(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy)
        {
            enemy.TakeDamage(_stats.AttackDamage, enemy.transform.position);
        }
    }
    
    public void OnDeath()
    {
        DisableInput();
        _isDead = true;
    }
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _visuals = GetComponent<PlayerVisuals>();

        shockwavePool.transform.parent = null;
    }

    private void OnEnable()
    {
        if (_input)
        {
            _input.OnDashPressed += Dash;
            _input.OnAttackPressed += Attack;
        }

        attackHitBox.OnTriggerEnter += OnAttackHit;
    }

    private void OnDisable()
    {
        if (_input)
        {
            _input.OnDashPressed -= Dash;
            _input.OnAttackPressed -= Attack;
        }
        
        attackHitBox.OnTriggerEnter -= OnAttackHit;
    }

    private void Update()
    {
        if (_isDead)
            return;
        
        float realtime = Time.realtimeSinceStartup;
        
        if (_input.JumpPressedThisFrame) 
            _lastJumpPressed = realtime;
        else
            _ifReleaseJumpAfterJumping = true;

        if (_input.MoveDir.x != 0f)
            _isLastFacingRight = _input.MoveDir.x > 0f;
        
        if (_input.MoveDir != Vector2.zero)
            _facingDirection = _input.MoveDir;
    }

    private void FixedUpdate()
    {
        // If dead, just apply friction on gravity
        if (_isDead)
            return;
        
        // Updates the velocity for horizontal and vertical movement
        HorizontalMovement();
        VerticalMovement();

        _rb.linearVelocity = Vector2.zero;
        
        Vector2 platformMoveAmt = groundChecker.GetPlatformMoveAmt();
        Vector2 totalMoveAmt = platformMoveAmt + _velocity * Time.deltaTime;
        
        _rb.MovePosition(_rb.position + totalMoveAmt);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // print("Contact point: " + other.contacts.Length + " | " + other.contacts[0].normal);
        // if (other.contacts.Length > 1)
        //     print("Contact point: " + other.contacts.Length + " | " + other.contacts[1].normal);
        
        ContactPoint2D contactPoint = other.contacts[0];
        
        EnemyBase entity = other.gameObject.GetComponent<EnemyBase>();
        if (entity && !_isInvincibleDamage)
        {
            OnHit?.Invoke(entity.Damage, transform.position);
        
            ApplyKnockback(contactPoint.normal, entity.KnockbackSpeed);
            
            ActivateInvincibility();
            return;
        }
        
        Spike spike = other.gameObject.GetComponent<Spike>();
        if (spike && !_isInvincibleDamage)
        {
            ApplyKnockback(contactPoint.normal, spike.KnockbackStrength);
            
            OnHit?.Invoke(spike.Damage, transform.position);
            
            ActivateInvincibility();
            return;
        }
        
        Chaser chaser = other.gameObject.GetComponent<Chaser>();
        if (chaser && !_isInvincibleDamage)
        {
            ApplyKnockback(contactPoint.normal, new Vector2(40, 10));
            
            OnHit?.Invoke(10, transform.position);
            
            ActivateInvincibility();
            return;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Chaser chaser = other.gameObject.GetComponentInParent<Chaser>();
        if (chaser && !_isInvincibleDamage && !IsDead)
        {
            ApplyKnockback(Vector2.right, new Vector2(40, 10));
            
            OnHit?.Invoke(20, transform.position);
            
            ActivateInvincibility();
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Collectible collectible = other.GetComponent<Collectible>();
        if (collectible)
        {
            collectible.Release();
        }
        
        // Spike damageTrigger = other.gameObject.GetComponent<Spike>();
        // if (damageTrigger)
        // {
        //     Vector2 dir = Vector2.one;
        //     if (transform.position.x < other.transform.position.x)
        //         dir.x *= -1f;
        //     
        //     ApplyKnockback(dir, damageTrigger.KnockbackStrength);
        //     
        //     // if (!_isInvincibleDamage)
        //     //     OnHit?.Invoke(damageTrigger.Damage, transform.position);
        //     
        //     ActivateInvincibility();
        // }
    }

    #endregion
    
    #region Private Methods

    // Updates horizontal velocity
    private void HorizontalMovement()
    {
        if (_dashTimeLeft > 0f)
        {
            _dashTimeLeft -= Time.deltaTime;
            
            // On finish dash
            if (_dashTimeLeft < 0f)
            {
                _dashTimeLeft = -1f;
                return;
            }
            
            // Negate 1f - percentage so that the curve goes from left to right.
            Vector2 dir = _facingDirection;
            if (dir.x == 0f)
                dir.x = _isLastFacingRight ? 1f : -1f;
            _velocity = _stats.DashCurve.Evaluate(1f - _dashTimeLeft / _stats.DashTime) * _facingDirection;
            
            return;
        }
        
        float xInput = _input.MoveDir.x;
        
        bool isGrounded = !_isInAir;
        
        // Slow down the player if not pressing any buttons 
        if (xInput == 0f)
        {
            float stopVelocityAmt = _stats.StopAcceleration * Time.deltaTime;
            // negate stopVelocityAmt because stats.stopAcceleration is always < 0 
            if (Mathf.Abs(_velocity.x) > -stopVelocityAmt)
                _velocity.x += stopVelocityAmt * (_velocity.x > 0f ? 1f : -1f);
            else
                _velocity.x = 0f;
        }
        else
        {
            // Comparing input dir and velocity, if...
            // Moving in the same direction
            if (xInput * _velocity.x >= 0)
            {
                _velocity.x += _stats.MoveAcceleration * xInput * Time.deltaTime;
                
                // Clamp horizontal speed depending if it's colliding with the ground
                float maxSpeed = isGrounded ? _stats.MaxSpeed : _stats.AirStrafeMaxSpeed;
                _velocity.x = Mathf.Clamp(_velocity.x, -maxSpeed, maxSpeed);
            }
            // Moving in opposite direction/turning (Grounded)
            else if (isGrounded)
                _velocity.x -= _stats.TurnAcceleration * (xInput) * Time.deltaTime;
            // Moving in opposite direction/turning (In air)
            else
                _velocity.x -= _stats.InAirTurnAcceleration * (xInput) * Time.deltaTime;
        }
    }
    
    // Updates vertical velocity
    private void VerticalMovement()
    {
        HandleLanding();

        if (_dashTimeLeft > 0)
            return;
        
        HandleGravity();
        
        HandleJump();
    }

    private void HandleLanding()
    {
        if (!groundChecker.IsColliding || _velocity.y > 0f) 
            return;
        
        // If player is holding down while falling,
        // Set one-way platform to be flipped (pass through from top)
        if (_input.MoveDir.y < 0)
        {
            PlatformBase platform = groundChecker.GetCollidingPlatformWithType(PlatformBase.Type.OneWay);
            if (!platform) 
                return;
        }
        // Land on ground
        else
        {
            // TODO: Check if need this if statement
            if (_isInAir)
            {
                // landSFX.Play();
            }
            
            _lastJumpTime = float.MinValue;
            _lastGroundedTime = Time.realtimeSinceStartup;
        }
    }

    private void HandleGravity()
    {
        float realtime = Time.realtimeSinceStartup;
        
        _isInAir = true;

        // If moving up
        if (_velocity.y > 0f)
        {
            // If hit ceiling
            if (ceilingChecker.IsCollidingWith(PlatformBase.Type.Everything, PlatformBase.Type.OneWay))
                _velocity.y = 0f;
            // Increased gravity if:
            // - player is moving up and not inputting jump
            // - AND it has jumped more than the minimum time
            else if (!_input.IsJumping && (realtime - _lastJumpPressed) > _stats.MinJumpTime)
                _velocity.y += _stats.Gravity * Time.deltaTime * _stats.GravityMultiplierWhenReleaseCurve.Evaluate(Time.realtimeSinceStartup - _lastJumpTime);
            else 
                _velocity.y += _stats.Gravity * Time.deltaTime;
        }
        // If on ground
        else if (groundChecker.IsGrounded())
        {
            _isInAir = false;
            _velocity.y = 0f;
        }
        // If wall sliding
        else if (leftWallChecker.IsColliding || rightWallChecker.IsColliding)
        {
            _velocity.y = Mathf.Max(_velocity.y + _stats.WallSlideAcceleration * Time.deltaTime, _stats.WallSlideMaxSpeed);
        }
        // Else, player's falling
        else
        {
            _velocity.y = Mathf.Max(_velocity.y + _stats.FallingGravity * Time.deltaTime, _stats.MaxFallVelocity);
        }
    }

    // Returns if was able to jump
    private bool HandleJump()
    {
        float realtime = Time.realtimeSinceStartup;

        bool isJumpBufferActive = realtime - _lastJumpPressed < _stats.JumpBuffer;
        // NOTE: this doesn't include the checks if it has a platform to jump off from
        // ifReleaseJumpAfterJumping - ensures that it doesn't keep jumping if player holds jump and player lands
        bool ifJump = isJumpBufferActive && _ifReleaseJumpAfterJumping;
        
        bool isLeftWallColliding = leftWallChecker.IsColliding;
        bool isRightWallColliding = rightWallChecker.IsColliding;
        
        // Wall jumps
        if (ifJump &&
            // If player is colliding with either walls
            (isLeftWallColliding || isRightWallColliding) &&
            // If player is either
            // - In Air
            // - On Ground AND Horizontal Input != 0
            (_isInAir || _input.MoveDir.x != 0f))
        {
            Jump();
            
            // Add some horizontal input depending on the horizontal input direction and which side the wall is on.
            bool isInputTowardsWall = _input.MoveDir.x != 0 &&
                                      _input.MoveDir.x < 0 ? isLeftWallColliding : !isLeftWallColliding;
            _velocity.x = (isLeftWallColliding ? 1f : -1f) * 
                         (isInputTowardsWall ? _stats.WallJumpHorizontalVelocityTowardsWall : _stats.WallJumpHorizontalVelocity);

            return true;
        }
        // Normal wall jump. 
        // Also checks if player isGrounded and coyoteTime
        else if (ifJump && (realtime - _lastGroundedTime < _stats.CoyoteTime))
        {
            Jump();

            return true;
        }

        return false;
    }

    private void Jump()
    {
        _velocity.y = _stats.JumpVelocity;
        _lastJumpPressed = float.MinValue; // Prevent jump buffer from triggering again
        _lastGroundedTime = float.MinValue;
        _lastJumpTime = Time.realtimeSinceStartup;
        _ifReleaseJumpAfterJumping = false;
    }

    // contactDirection - Direction from contact point to player
    private void ApplyKnockback(Vector2 contactDirection, Vector2 speed)
    {
        _dashTimeLeft = -1f;
        
        _velocity = speed * Vector2.Reflect(_velocity.normalized, contactDirection.normalized); ;
        // _velocity = speed * contactDirection.normalized;
        Debug.DrawRay(transform.position, contactDirection, Color.blue, 1f);
        Debug.DrawRay(transform.position, _velocity, Color.red, 1f);

        if (Mathf.Abs(_velocity.y) < _stats.MinKnockbackVerticalSpeed)
        {
            _velocity.y = _stats.MinKnockbackVerticalSpeed;
        }

        if (_velocity.y < 0f)
            _velocity.y *= 0.5f; // Down knockback is too strong, partly because of increased gravity when going down
        
        Debug.DrawRay(transform.position, _velocity, Color.green, 1f);
        print("Knockback final vel: " + _velocity);
        
        // Vector2 totalMoveAmt = _velocity * Time.fixedDeltaTime;
        //
        // _rb.MovePosition(_rb.position + totalMoveAmt);
    }

    private void ActivateInvincibility()
    {
        if (_invincibilityCoroutine != null)
        {
            StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = null;
        }
        _invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        // _rb.excludeLayers = invincibilityMask;
        _isInvincibleDamage = true;
        _visuals.OnInvincibilityChange(true);

        yield return _invincibilityWait;
        
        _visuals.OnInvincibilityChange(false);
        // _rb.excludeLayers = 0;
        _isInvincibleDamage = false;
        
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(groundChecker.Collider, colliders);
        if (colliders.Count > 0)
        {
            // Re-trigger any OnCollisionEnter
            _collider.enabled = false;
            _collider.enabled = true;
        }

        _invincibilityCoroutine = null;
    }

    private void DisableInput()
    {
        _velocity = Vector2.zero;
        _dashTimeLeft = -1f;
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }
    
    private void ResetPlayer()
    {
        _velocity = Vector2.zero;
        
        _lastJumpPressed = float.MinValue;
        _lastGroundedTime = float.MinValue;
        _lastJumpTime = float.MinValue;

        _ifReleaseJumpAfterJumping = true;

        _isInAir = false;
    }
    #endregion
}
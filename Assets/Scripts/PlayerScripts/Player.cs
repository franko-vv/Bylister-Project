using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Класс игрока
/// </summary>
public class Player : MonoBehaviour, ITakeDamage
{
    private bool _IsFacingRight;
    private CharacterController2D _controller;
    private float normilazeHorizontalSpeed;

    public float MaxSpeed;
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;

    //
    public GameObject HitEffect;
    public Projectile Projectile;
    public float FireRate;
    public Transform ProjectileFireLocation;

    private float _canFireIn;


    // Здоровье игрока
    public int maxHealth = 100;
    public int Health { get; private set; }


    public bool isDead;

    public Animator animator;

    [HeaderAttribute ("Player Sounds")]
    public AudioClip PlayerHitSound;
    public AudioClip PlayerShootSound;
    public AudioClip PlayerDieSound;
    public AudioClip PlayerHealthSound;



    public void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _IsFacingRight = transform.localScale.x > 0;

        Health = maxHealth;
    }

    public void Update()
    {
        _canFireIn -= Time.deltaTime;

        if (!isDead)
            HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        if (isDead)
            _controller.SetHorizontalForce(0);
        else
		    _controller.SetHorizontalForce (Mathf.Lerp(_controller.Velocity.x, normilazeHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));

        animator.SetBool("isGrounded", _controller.State.IsGrounded);
        animator.SetBool("isDead", isDead);
        animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x) / MaxSpeed);
    }

    /// <summary>
    /// Метод окончания текущего уровня
    /// </summary>
    public void FinishLevel()
    {
        enabled = false;
        _controller.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    /// <summary>
    /// Настройка управления
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            normilazeHorizontalSpeed = -1;
            if (_IsFacingRight)
                Flip();
        }

        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            normilazeHorizontalSpeed = 1;
            if (!_IsFacingRight)
                Flip();
        }

        else
        {
            normilazeHorizontalSpeed = 0;
        }

        if (_controller.CanJump && Input.GetKey(KeyCode.Space))
        {
            _controller.Jump();
        }

        if (Input.GetKey(KeyCode.LeftAlt))
            FireProjectile();
    }

    /// <summary>
    /// Стрельба  пулями и задание направления для пули
    /// </summary>
    private void FireProjectile()
    {
        if (_canFireIn > 0)
            return;

        var direction = _IsFacingRight ? Vector2.right : Vector2.left;

        var projectile = (Projectile) Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);

        Debug.Log(direction + "  " + _controller.Velocity);
        projectile.Initialize(gameObject, direction, _controller.Velocity);

        // Если игрок движется влево тогда разворачиваем изображение пули
        projectile.transform.localScale = new Vector3(_IsFacingRight ? 1 : -1, 1, 1);

        _canFireIn = FireRate;

        // Audio
        AudioSource.PlayClipAtPoint(PlayerShootSound, transform.position);

        animator.SetTrigger("Fire");
    }

    /// <summary>
    /// Ударение игрока и уменьшение его здоровья
    /// </summary>
    public void TakeDamage(int damage, GameObject insigator)
    {
        // Вызов еффекта удара игрока
        Instantiate(HitEffect, transform.position, transform.rotation);
        Health -= damage;

        if (Health <= 0)
            LevelManager.Instance.KillPlayer();

        // Audio
        AudioSource.PlayClipAtPoint(PlayerHitSound, transform.position);
    }


    public void GiveHealth(int health, GameObject instigator)
    {
        FloatingText.Show(String.Format("+{0}!", health), "PoinstarText",
            new FromWorldTextPositioner(Camera.main, transform.position, 1.5f, 50));

        // Количестов жизней не больше максимального количества жизней
        Health = Mathf.Min(maxHealth, Health + health);

        // Audio
        AudioSource.PlayClipAtPoint(PlayerHealthSound, transform.position);
    }

    /// <summary>
    /// Разворот персонажа на 180 градусов
    /// </summary>
    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _IsFacingRight = transform.localScale.x > 0;
    }

    /// <summary>
    /// Убийство игрока
    /// </summary>
    public void Kill()
    {
        _controller.HandleCollisions = false;
        // отключаем коллайдер игрока
        GetComponent<Collider2D>().enabled = false;
        isDead = true;

        // При убийстве делаем небольшой прыжок игрока вверх
        _controller.SetVerticalForce(15);

        Health = 0;

        // Audio
        AudioSource.PlayClipAtPoint(PlayerDieSound, transform.position);
    }

    /// <summary>
    /// Появление игрока в позиции
    /// </summary>
    public void RespawnAt(Transform spawnPoint)
    {
        // Если игрок смотрит влево, тогда оборачиваем
        if (!_IsFacingRight)
            Flip();

        // Включаем возможность управления игроком
        GetComponent<Collider2D>().enabled = true;
        _controller.HandleCollisions = true;
        isDead = false;

        Health = maxHealth;

        // Устанавливаем новую позицию игроку
        transform.position = spawnPoint.position;

        // Audio
        AudioSource.PlayClipAtPoint(PlayerHealthSound, transform.position);
    }

}

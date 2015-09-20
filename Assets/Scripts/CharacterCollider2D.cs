using UnityEngine;
using System.Collections;

public class CharacterCollider2D : MonoBehaviour 
{
    // Кожа героя состоит из лучей, который воспринимают коллизии
    private const float SkinWidth = .2f;
    private const int TotalHorizontalRays = 8;
    private const int TotalVerticalRays = 4;

    private static readonly float SlopeLimitTangant = Mathf.Tan(75f * Mathf.Rad2Deg);

    public LayerMask PlatformMask;
    public ControllerParameters2D DefaultParameters;

    public ControllerState2D State { get; private set; }

    public bool CanJump { get; set; }
    public Vector2 Velocity { get; private set; }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localscale;
    private BoxCollider2D _boxCollider2D;

    public bool HandeCollision { get; set; }

    private ControllerParameters2D _overrideParameters;

    public ControllerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }

    float heightDistanceBetweenRays, widthDistanceBetweenHorizontalRays;

    public void Awake()
    {
        State = new ControllerState2D();
        _transform = transform;
        _localscale = transform.localScale;
        _boxCollider2D = GetComponent<BoxCollider2D>();


        // Рассчитываем расстояние между "лучами" кожи.
        // по вертикали - 4 луча
        // по горизонтали - 8 лучей
        var colliderwidth = _boxCollider2D.size.x * Mathf.Abs(transform.localScale.x) - 2 * SkinWidth;
        widthDistanceBetweenHorizontalRays = colliderwidth / (TotalHorizontalRays - 1);
        var colliderheight = _boxCollider2D.size.y * Mathf.Abs(transform.localScale.y) - 2 * SkinWidth;
        heightDistanceBetweenRays = colliderheight / (TotalVerticalRays - 1);    
    }

    public void AddForce(Vector2 force)
    {
        _velocity = force;
    }

    public void SetForce(Vector2 force)
    {
        _velocity += force;
    }

    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }

    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }

    public void Jump()
    {
    }

    public void LateUpdate()
    {
        Move(Velocity * Time.deltaTime);
    }

    private void Move(Vector2 deltaMovement)
    {
        var wasGrounded = State.IsColidingBelow;
        State.Reset();

        if (HandeCollision)
        {
            HandlePlatforms();
            CalculateRayOrigins();

            if (deltaMovement.y < 0 && wasGrounded)
                HandleVerticalSlope(ref deltaMovement);

            if (Mathf.Abs(deltaMovement.x) > 0.001f)
                MoveHorizontally(ref deltaMovement);

            MoveVertically(ref deltaMovement);
        }

        _transform.Translate(deltaMovement, Space.World);

        if (Time.deltaTime > 0)
            _velocity = deltaMovement * Time.deltaTime;

        // TODO: Additional moving platforms

        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        if (State.IsMovingUpSlope)
            _velocity.y = 0;
    }
        
    private void HandlePlatforms()
    {
    }

    private Vector2 _raycastTopLeft, _raycastTopRight, _raycastBottomLeft;

    private void CalculateRayOrigins()
    {
        var size = new Vector2(_boxCollider2D.size.x * Mathf.Abs(_localscale.x), _boxCollider2D.size.y * Mathf.Abs(_localscale.y)) / 2;
        var center = new Vector2(_boxCollider2D.offset.x * _localscale.x, _boxCollider2D.offset.y * _localscale.y);

        _raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        _raycastTopRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);

    }

    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
        // движение влево/вправо
        var isGoingRight = deltaMovement.x > 0;
        // длина перемещения = длина перемещения + толщинне кожи
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        // направление движение влево/вправо
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        // с какой точки отсчет начинать если движение влево/ вправо
        var rayOrigin = isGoingRight ? _raycastTopLeft : _raycastBottomLeft;

        for (var i = 0; i < TotalHorizontalRays; i++)
        {
            // рисуем горизонтальные векторы с шагом по вертикали
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * heightDistanceBetweenRays));
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.green);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);

            if (!raycastHit)
                continue;

            if ( i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(raycastHit.normal, Vector2.up), isGoingRight))
                break;

            deltaMovement.x = raycastHit.point.x - rayVector.x;
            rayDistance = Mathf.Abs(deltaMovement.x);

            if (isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsColidingRight = true;
            }
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsColidingLeft = true;
            }

            if (rayDistance < SkinWidth + .0001f)
                break;
        }
    }

    private void MoveVertically(ref Vector2 deltaMovement)
    {
    }

    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
    }

    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
    {
        return false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
    }

    public void OnTriggerExit2D(Collider2D other)
    {
    }



}

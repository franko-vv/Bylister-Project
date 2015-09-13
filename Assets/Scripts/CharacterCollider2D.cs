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

    public void Awake()
    {
        State = new ControllerState2D();
    }

    public void AddForce(Vector2 force)
    {
    }

    public void SetForce(Vector2 force)
    {
    }

    public void SetHorizontalForce(float x)
    {
    }

    public void SetVerticalForce(float y)
    {
    }

    public void Jump()
    {
    }

    public void LateUpdate()
    {
    }

    public void Move(Vector2 deltaMovement)
    {
    }

    private void HandlePlatforms()
    {
    }

    private void CalculateRayOrigins()
    {
    }

    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
    }

    private void MoveVertically(ref Vector2 deltaMovement)
    {
    }

    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
    }

    private void HandleHorizontalSlope(ref Vector2 deltaMovement)
    {
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
    }

    public void OnTriggerExit2D(Collider2D other)
    {
    }

}

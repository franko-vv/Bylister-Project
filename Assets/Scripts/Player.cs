using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    private bool _IsFacingRight;
    private CharacterCollider2D _controller;
    private float normilazeHorizontalSpeed;

    public float MaxSpeed;
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;

    public void Start()
    {
        _controller = GetComponent<CharacterCollider2D>();
        _IsFacingRight = transform.localScale.x > 0;
    }

    public void Update()
    {
        HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
        _controller.SetHorizontalForce (Mathf.Lerp(_controller.Velocity.x, normilazeHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            normilazeHorizontalSpeed = -1;
            if (_IsFacingRight)
                Flip();
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _IsFacingRight = transform.localScale.x > 0;
    }
}

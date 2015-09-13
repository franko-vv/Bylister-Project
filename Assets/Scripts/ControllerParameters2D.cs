using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ControllerParameters2D
{
    public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CannotJump
    }

    public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    [Range (0, 90)]
    public int SlopeLimit = 30;

    public float Gravity = -25f;

    public JumpBehavior JumpRestriction;

    public float JumpFrequency = .25f;
}

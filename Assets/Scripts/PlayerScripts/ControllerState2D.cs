using UnityEngine;
using System.Collections;

public class ControllerState2D
{
    public bool IsColidingRight { get; set; }
    public bool IsColidingLeft { get; set; }
    public bool IsColidingAbove { get; set; }
    public bool IsColidingBelow { get; set; }
    public bool IsMovingDownSlope { get; set; }
    public bool IsMovingUpSlope { get; set; }
    public bool IsGrounded { get { return IsColidingBelow; } }
    public float SlopeAngle { get; set; }

    public bool SetCollisions { get { return IsColidingRight || IsColidingLeft || IsColidingBelow || IsColidingAbove; } }

    public void Reset()
    {
        IsColidingAbove = IsColidingBelow = IsColidingLeft = IsColidingRight = IsMovingDownSlope = IsMovingUpSlope = false;

        SlopeAngle = 0;
    }

    public override string ToString()
    {
        return string.Format("(controller): right:{0}  left:{1}  above:{2}  below: {3}  down-slope: {4}  up-slope: {5}  angle: {6}",
            IsColidingRight, IsColidingLeft, IsColidingAbove, IsColidingBelow, IsMovingDownSlope, IsMovingUpSlope, SlopeAngle);
    }

}

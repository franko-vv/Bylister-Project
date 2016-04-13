using UnityEngine;
using System.Collections;

/// <summary>
/// Класс для платформы-батута
/// </summary>
public class JumpPlatform : MonoBehaviour
{
    public float _jumpMagnitude = 20f;

    public void ControllerEnter2D(CharacterController2D controller)
    {
        controller.SetVerticalForce(_jumpMagnitude);
    }
}

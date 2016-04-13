using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ControllerParameters2D
{
	public enum JumpBehavior
	{
		CanJumpOnGround,
		CanJumpAnywhere,
		CantJump
	}

	public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    /// <summary>
    /// Максимальный угол наклона под который игрок может выходить
    /// </summary>
	[Range(0, 90)]
	public float SlopeLimit = 30;

    /// <summary>
    /// Гравитация заданая
    /// </summary>
	public float Gravity = -25f;

    /// <summary>
    /// Способность прыгать на земле, в воздухе или невозможность прыгнуть
    /// </summary>
	public JumpBehavior JumpRestrictions;

    /// <summary>
    /// Задержка времени перед следующим прыжком
    /// </summary>
    public float JumpFrequency = .25f;

    /// <summary>
    /// Максимальная высота прыжка
    /// </summary>
	public float JumpMagnitude = 12;
}
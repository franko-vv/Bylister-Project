using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GiveDamageToPlayer : MonoBehaviour

{
    public int damageToPlayer = 10;

    private Vector2 _lastPosition, _velocity;

    public void LateUpdate()
    {
        _velocity = (_lastPosition - (Vector2)transform.position )/ Time.deltaTime;
        _lastPosition = transform.position;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.TakeDamage(damageToPlayer, gameObject);

        var controller = player.GetComponent<CharacterController2D>();
        var totalVelocity = controller.Velocity + _velocity;


        controller.SetForce( new Vector2(
            -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x)* 7, 15, 30),
            -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 2, 0, 20)));

    }
}
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Класс базуки
/// </summary>
public class PathedProjectileSpawner : MonoBehaviour
{
    /// <summary>
    /// Конечная точка траектории пули
    /// </summary>
    public Transform Destination;
    public PathedProjectile Projectile;

    public float speed;
    public float fireRate;

    public AudioClip ProjectileFireSound;
    
    private float nextShotInSeconds;

    public void Start()
    {
        nextShotInSeconds = fireRate;
    }

    public void Update()
    {
        if ((nextShotInSeconds -= Time.deltaTime) > 0)
            return;

        nextShotInSeconds = fireRate;

        var projectiles = Instantiate(Projectile, transform.position, transform.rotation) as PathedProjectile;
        projectiles.Initialize(Destination, speed);

        // Audio
        if (ProjectileFireSound != null)
            AudioSource.PlayClipAtPoint(ProjectileFireSound, transform.position);
    }
    
    public void OnDrawGizmos()
    {
        if (Destination == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Destination.position);
    }
}

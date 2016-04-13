using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// Класс пули
/// </summary>
public class PathedProjectile : MonoBehaviour, ITakeDamage
{
    private Transform _destination;
    private float speed;

    public GameObject DestroyEffect;
    public int PointsToGiveToPlayer;

    public void Initialize(Transform destination, float _speed)
    {
        _destination = destination;
        speed = _speed;
    }

    public void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination.position, speed * Time.deltaTime);

        var distancesquare = (_destination.transform.position - transform.position).sqrMagnitude;

        if (distancesquare > 0.1f * 0.1f)
            return;

        Destroy(gameObject);
    }

    /// <summary>
    /// Удаляем пулю если столкнулась с игроком
    /// </summary>
    public void OnTriggerEnter2D (Collider2D other)
    {
        var somethingCollide = other.GetComponent<Player>();

        if (somethingCollide != null)
            Destroy(gameObject);
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);

        var projectile = instigator.GetComponent<Projectile>();
        if (projectile != null && projectile.Owner.GetComponent<Player>() != null && PointsToGiveToPlayer != 0)
        {
            GameManager.Instance.AddPoints(PointsToGiveToPlayer);
            FloatingText.Show(string.Format("+ {0}!", PointsToGiveToPlayer), "PoinstarText", new FromWorldTextPositioner(Camera.main, transform.position, 1.5f, 100));
        }
    }
}

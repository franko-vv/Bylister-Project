using UnityEngine;
using System;

/// <summary>
/// Класс пули игрока
/// </summary>
public class SimpleProjectile : Projectile, ITakeDamage
{
    public int Damage;
    public GameObject DestroyEffect;
    public int PointsToGiveToPlayer;
    public float TimeToLive;

    void Update()
    {
        if ((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
            return;
        }

#warning Корявое направление можетбіть связано с direction
        transform.Translate((Direction + new Vector2(InitialVelocity.x, 0)) * Speed * Time.deltaTime, Space.World);
        // transform.Translate(Direction.x + ((Mathf.Abs(InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if (PointsToGiveToPlayer != 0)
        {
            var projectile = instigator.GetComponent<Projectile>();
            if (projectile != null && projectile.Owner.GetComponent<Player>() != null)
            {
                GameManager.Instance.AddPoints(PointsToGiveToPlayer);
                FloatingText.Show(string.Format("+ {0}!", PointsToGiveToPlayer), "PoinstarText", new FromWorldTextPositioner(Camera.main, transform.position, 1.5f, 100));
            }
        }
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }



    protected override void OnCollideOther(Collider2D other)
    {
        //DestroyProjectile();
    }

    protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
    {
        takeDamage.TakeDamage(Damage, gameObject);
        DestroyProjectile();
    }
}

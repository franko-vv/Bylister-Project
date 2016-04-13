using UnityEngine;
using System.Collections;
using System;

public class GiveHealth : MonoBehaviour
{
    // Ефект при собирании звезды 
    public GameObject Effect;
    public int healthToGive = 10;

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.GiveHealth(healthToGive, gameObject);
        Instantiate(Effect, transform.position, transform.rotation);

        // Выключаем обьект до след. точки сохранения
        gameObject.SetActive(false);
    }

    // Включаем обьект если игрок умер до следующей точки сохранения после собирания монетки
    public void OnPLayerRespawnOnThisCheckPoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    } 
}

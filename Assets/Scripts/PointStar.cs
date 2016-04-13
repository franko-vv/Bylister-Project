using System;
using System.Collections.Generic;
using UnityEngine;


public class PointStar : MonoBehaviour, IPlayerRespawnListener
{
    // Ефект при собирании звезды 
    public GameObject Effect;
    public int pointsToAdd = 10;

    // Audio
    public AudioClip CollectedAStarSound;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        GameManager.Instance.AddPoints(pointsToAdd);
        Instantiate(Effect, transform.position, transform.rotation);
        
        // Выключаем обьект до след. точки сохранения
        gameObject.SetActive(false);

        //Destroy(gameObject);

        // Надпись + 10 очей
        FloatingText.Show(String.Format("+{0}!", pointsToAdd), "PoinstarText", 
            new FromWorldTextPositioner( Camera.main, transform.position, 1.5f,  50));

        // Audio
        if (CollectedAStarSound !=null)
            AudioSource.PlayClipAtPoint(CollectedAStarSound, transform.position);
    }

    // Включаем обьект если игрок умер до следующей точки сохранения после собирания монетки
    public void OnPLayerRespawnOnThisCheckPoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
}

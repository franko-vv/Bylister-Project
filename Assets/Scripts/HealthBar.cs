using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class HealthBar : MonoBehaviour
{
    public Player player;
    public Transform ForeGroundSprite;

    public Camera mainCamera;

    //  Цвет линии жизни
    public SpriteRenderer ForegroundRenderer;
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);   // 255,63,63
    public Color MinHealthColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);  // 64,137,255

    public void Awake()
    {
    }

    public void Update()
    {
        var healthPercent = player.Health /(float) player.maxHealth;

        ForeGroundSprite.localScale = new Vector3(healthPercent, 1, 1);
        ForegroundRenderer.color = Color.Lerp(MaxHealthColor, MinHealthColor, healthPercent);




        float cameraHalfWidth = mainCamera.orthographicSize * ((float)Screen.width / Screen.height);

        var healthBarPosition = new Vector3(
            mainCamera.transform.position.x - mainCamera.orthographicSize + 1,
            mainCamera.transform.position.y - cameraHalfWidth + 2, 0);

        // Debug.Log(healthBarPosition);

        transform.position = healthBarPosition;
    }


}
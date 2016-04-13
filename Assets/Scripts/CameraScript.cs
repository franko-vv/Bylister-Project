using UnityEngine;
using System.Collections;

/// <summary>
/// Клас для камеры, перемещением за игроком и ограничении в зоне
/// </summary>
public class CameraScript : MonoBehaviour
{
    public Transform player;

    // Границы уровня
    public BoxCollider2D boundsOfTheLevel;

    // Максимальная и минимальная точка границ уровня
    private Vector3 max, min;

    [SerializeField]
    private Vector2 smooth; //задержка движения камеры
    [SerializeField]
    private Vector2 margin; //

    private bool isFollowing;
    public bool IsFollowing { get; set; }

    // Половина высоты обзора камеры
    private float orthographicSize;

    void Start ()
    {
        max = boundsOfTheLevel.bounds.max;
        min = boundsOfTheLevel.bounds.min;

        isFollowing = true;

        //
        orthographicSize = GetComponent<Camera>().orthographicSize;
    }
	
	void Update ()
    {
        // Текущая позиция камеры
        float x = player.position.x;
        float y = player.position.y;

        // Движение камеры с запозданием если игрок выходит за маргин пределы отслеживания камеры
        if (isFollowing)
        {
            if (Mathf.Abs(x - player.position.x) > margin.x)
                Mathf.Lerp(x, player.position.x, smooth.x * Time.deltaTime);

            if (Mathf.Abs(y - player.position.y) > margin.y)
                Mathf.Lerp(y, player.position.y, smooth.y * Time.deltaTime);
        }

        // половина ширины камеры
        float cameraHalfWidth = orthographicSize * ( (float)Screen.width / Screen.height);

        // Исключаем выезд камеры за пределы границ уровня
        x = Mathf.Clamp(x, min.x + cameraHalfWidth, max.x - cameraHalfWidth);
        y = Mathf.Clamp(y, min.y + orthographicSize, max.y - orthographicSize);

        transform.position = new Vector3(x, y, transform.position.z);

        
	}
}

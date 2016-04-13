using UnityEngine;
using System.Collections;

/// <summary>
/// Поверхность-убийца
/// </summary>
public class InstaKill : MonoBehaviour
{

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();

        // Если не игрок то выход
        if (player == null)
            return;
        
        LevelManager.Instance.KillPlayer();
    }

}

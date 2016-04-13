using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Точка сохранения и загрузки
/// </summary>
public class Checkpoint : MonoBehaviour
{
    // Список монеток что зависятотточки сохранения
    private List<IPlayerRespawnListener> listeners;

    void Awake()
    {
        listeners = new List<IPlayerRespawnListener>();
    }

    /// <summary>
    /// Добавить обьект зависимым от точки сохранения
    /// </summary>
    public void AddListenerToCheckpoint(IPlayerRespawnListener listener)
    {
        listeners.Add(listener);
    }

	void Start ()
    {
	
	}

    public void PlayerHitCheckPoint()
    {
            
    }

    public void PlayerLeftCheckPoint()
    {

    }

    /// <summary>
    /// Воскрешение игрока в точке сохранения и возврат всех собранных монеток после этой точки
    /// </summary>
    public void SpawnPlayer(Player player)
    {
        player.RespawnAt(transform);

        foreach (var listener in listeners)
        {
            listener.OnPLayerRespawnOnThisCheckPoint(this, player);
        }
    }
	
    private IEnumerator PlayerHitCheckPOintCo(int bonus)
    {
        yield break;
    }

    void Update ()
    {
	
	}
}

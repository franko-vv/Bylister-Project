using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class GameManager
{
    //Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance ?? (_instance = new GameManager()); } }
    private GameManager() { }


    public int Points { get; private set; }

    /// <summary>
    /// Полное обнуление количества очков
    /// </summary>
    public void Reset()
    {
        Points = 0;
    }

    public void AddPoints(int pointsToAdd)
    {
        Points += pointsToAdd;
    }

    /// <summary>
    /// Удаление бонусных очей собранных после последней сохраненной позиции
    /// </summary>
    public void ResetPoints(int pointsAfterLastCheckPoint)
    {
        Points = pointsAfterLastCheckPoint;

    }


	void Start () {
	
	}
	

	void Update () {
	
	}
}

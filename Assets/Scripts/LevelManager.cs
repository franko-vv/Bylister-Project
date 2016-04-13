using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// 
/// </summary>
public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }

    public Player player { get; private set; }
    public CameraScript  Camera { get; private set; }

    // Список всех точек сохранения в уровне
    public List<Checkpoint> _checkpoints;
    // Последняя точка сохранения
    public int currentCheckpointIndex;
    // Время прохождения уровня
    public TimeSpan RunningTime { get { return DateTime.UtcNow - started; } }

    // для тестов появления в разніх точках
    public Checkpoint DebugSpawn;

    /// <summary>
    /// Количество бонусов за прохождение чекпойнтов
    /// </summary>
    public int CurrentTimeBonus
    {
        get
        {
            int secondsDifference = (int)( BonusCutOffSeconds - RunningTime.TotalSeconds);
            return Math.Max(0, secondsDifference) * BonusSecondsMultiplier;
        }
    }

    // Количество сохраненных очей
    private int savedPoints;

    // Время прохождение от последней точки сохранения
    private int BonusCutOffSeconds;
    // Переменная для увеличения количества бонусов
    private int BonusSecondsMultiplier;

    // Время начала ровня
    private DateTime started;


    void Awake ()
    {
        Instance = this;
        savedPoints = GameManager.Instance.Points;
    }

	void Start ()
    {
        // Сохраняем все точки в списке по координатам Х
        _checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
        // -1 - значит на уровне нет точек сохранения
        currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;

        // Устанавливаем камеру и игрока логике
        player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<CameraScript>();


        IEnumerable<IPlayerRespawnListener> listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach (var listener in listeners)
        {
            for (int i = _checkpoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints[i].transform.position.x;
                if (distance < 0)
                    continue;

                _checkpoints[i].AddListenerToCheckpoint(listener);
            }
        }


        




        // Код для теста
        // Появление игрока в первой точке или последней точке
#if UNITY_EDITOR
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(player);
        else if (currentCheckpointIndex != -1)
            _checkpoints[currentCheckpointIndex].SpawnPlayer(player);
#else
        if (currentCheckpointIndex != -1)
            _checkpoints[currentCheckpointIndex].SpawnPlayer(player);
#endif
    }

    void Update ()
    {
        // Если в последней точке сохранения
        var isLastCheckPoint = currentCheckpointIndex + 1 >= _checkpoints.Count;
        if (isLastCheckPoint)
            return;

        // дистанция до след. точки
        float distanceToNextPoint = _checkpoints[currentCheckpointIndex + 1].transform.position.x - player.transform.position.x;
        // Если есть дистанция то выходим
        if (distanceToNextPoint >= 0)
            return;

        // Код - если стоим в точке сохранения

        // Игрок оставил прошлую точку сохранения
        _checkpoints[currentCheckpointIndex].PlayerLeftCheckPoint();
        currentCheckpointIndex++;
        // Установка новой точки сохранения
        _checkpoints[currentCheckpointIndex].PlayerHitCheckPoint();

        // Добавляем очки
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        savedPoints = GameManager.Instance.Points;
        started = DateTime.UtcNow;
    }

    /// <summary>
    /// Переход на следующий уровень
    /// </summary>
    public void GoToNextLevel(string levelName)
    {
        StartCoroutine(GoToNextLevelCo(levelName));
    }

    IEnumerator GoToNextLevelCo(string levelName)
    {

        player.FinishLevel();
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        // FloatingText.Show(String.Format("+{0}!", CurrentTimeBonus), "PoinstarText");
#warning Centered TextPOsitioner class I miss
        yield return new WaitForSeconds(2f);

        // Если нет следующего уровня возвращаемся в начальный экран
        if (string.IsNullOrEmpty(levelName))
            Application.LoadLevel("StartScene");
        else
            Application.LoadLevel(levelName);
        yield return new WaitForSeconds(5f);
    }

    /// <summary>
    /// Убийство игрока
    /// </summary>
    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());

    }

    IEnumerator KillPlayerCo()
    {
        // убиваем игрока
        player.Kill();
        // отменяем слежение за игроком
        Camera.IsFollowing = false;
        yield return new WaitForSeconds(1f);

        //Если есть на уровне точки сохранения рестарт игрока
        if (currentCheckpointIndex != -1)
            _checkpoints[currentCheckpointIndex].SpawnPlayer(player);

        started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(savedPoints);
    }
}

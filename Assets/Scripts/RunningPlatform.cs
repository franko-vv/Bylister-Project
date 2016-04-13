using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class RunningPlatform : MonoBehaviour {

    // Позиция, масштаб и поворот обьекта (платформы)
    public Transform[] Points;

    // Создаем пользовательский нумератор
    public IEnumerator<Transform> GetPlatformEnumerator()
    {
        if (Points == null || Points.Length < 1)
            yield break;

        int direction = 1;
        int index = 0;

        // цикл для движения платформы от 1 к последней точке и назад
        while (true)
        {
            yield return Points[index];

            if (Points.Length == 1)
                continue;

            if (index <= 0)
                direction = 1;
            else if (index >= Points.Length - 1)
                direction = -1;

            index = index + direction;
        }
    }

    public void OnDrawGizmos()
    {
        // Проверям или количество точек достаточно для отрисовки линии
        if (Points == null || Points.Length < 2)
            return;

        var points = Points.Where(t => t != null).ToList();

        for (int i = 1; i < points.Count; i++)
        {
            // Рисуем линию между точками где
            // Points[i-1].position - позиция точки с координатами хуz
            Gizmos.DrawLine(points[i-1].position, points[i].position);
        }
    }
}

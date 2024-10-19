using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform[] waypoints;  // Массив точек маршрута
    public float speed = 10f;      // Скорость машины
    public float turnSpeed = 5f;   // Скорость поворота
    private int currentWaypoint = 0;  // Индекс текущей точки

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Движение к текущей точке маршрута
        Transform target = waypoints[currentWaypoint];
        Vector3 direction = (target.position - transform.position).normalized;

        // Плавный поворот к цели
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

        // Перемещение к точке
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Переход к следующей точке, если достигнута текущая
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}

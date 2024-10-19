using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianAI : MonoBehaviour
{
    public Transform[] waypoints;  // Точки маршрута.
    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    public float detectionRadius = 50.0f;  // Радиус обнаружения машин.
    public LayerMask carLayer;  // Слой машин для обнаружения.
    public float waitTime = 2.0f;  // Время ожидания перед продолжением движения.

    private bool isWaiting = false;  // Флаг ожидания.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToNextWaypoint();
    }

    void Update()
    {
        // Переходим к следующей точке маршрута.
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
        {
            MoveToNextWaypoint();
        }

        CheckForMovingCars();  // Проверка машин поблизости.
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void CheckForMovingCars()
    {
        // Находим все объекты в радиусе обнаружения.
        Collider[] nearbyCars = Physics.OverlapSphere(transform.position, detectionRadius, carLayer);

        foreach (Collider car in nearbyCars)
        {
            Rigidbody carRb = car.GetComponent<Rigidbody>();

            // Если машина движется, пешеход останавливается.
            if (carRb != null && carRb.velocity.magnitude > 0.1f)
            {
                agent.isStopped = true;
                if (!isWaiting) // Если пешеход не ждет, начинаем ожидание.
                {
                    StartCoroutine(WaitForCar());
                }
                return;  // Достаточно обнаружить одну движущуюся машину.
            }
        }

        // Если машин поблизости нет или они не движутся, продолжаем движение.
        agent.isStopped = false;
        isWaiting = false;  // Сброс флага ожидания.
    }

    IEnumerator WaitForCar()
    {
        isWaiting = true;  // Установить флаг ожидания.
        yield return new WaitForSeconds(waitTime);  // Ждать заданное время.
        isWaiting = false;  // Сброс флага ожидания.
        agent.isStopped = false;  // Продолжить движение.
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            MoveToNextWaypoint();  // Сменить маршрут при столкновении с другим NPC.
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            MoveToNextWaypoint();  // Сменить точку при столкновении.
        }
    }
}

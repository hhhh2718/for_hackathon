using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianAI : MonoBehaviour
{
    public Transform[] waypoints;  // ����� ��������.
    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    public float detectionRadius = 50.0f;  // ������ ����������� �����.
    public LayerMask carLayer;  // ���� ����� ��� �����������.
    public float waitTime = 2.0f;  // ����� �������� ����� ������������ ��������.

    private bool isWaiting = false;  // ���� ��������.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToNextWaypoint();
    }

    void Update()
    {
        // ��������� � ��������� ����� ��������.
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
        {
            MoveToNextWaypoint();
        }

        CheckForMovingCars();  // �������� ����� ����������.
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void CheckForMovingCars()
    {
        // ������� ��� ������� � ������� �����������.
        Collider[] nearbyCars = Physics.OverlapSphere(transform.position, detectionRadius, carLayer);

        foreach (Collider car in nearbyCars)
        {
            Rigidbody carRb = car.GetComponent<Rigidbody>();

            // ���� ������ ��������, ������� ���������������.
            if (carRb != null && carRb.velocity.magnitude > 0.1f)
            {
                agent.isStopped = true;
                if (!isWaiting) // ���� ������� �� ����, �������� ��������.
                {
                    StartCoroutine(WaitForCar());
                }
                return;  // ���������� ���������� ���� ���������� ������.
            }
        }

        // ���� ����� ���������� ��� ��� ��� �� ��������, ���������� ��������.
        agent.isStopped = false;
        isWaiting = false;  // ����� ����� ��������.
    }

    IEnumerator WaitForCar()
    {
        isWaiting = true;  // ���������� ���� ��������.
        yield return new WaitForSeconds(waitTime);  // ����� �������� �����.
        isWaiting = false;  // ����� ����� ��������.
        agent.isStopped = false;  // ���������� ��������.
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            MoveToNextWaypoint();  // ������� ������� ��� ������������ � ������ NPC.
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            MoveToNextWaypoint();  // ������� ����� ��� ������������.
        }
    }
}

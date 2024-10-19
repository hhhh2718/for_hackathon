using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform[] waypoints;  // ������ ����� ��������
    public float speed = 10f;      // �������� ������
    public float turnSpeed = 5f;   // �������� ��������
    private int currentWaypoint = 0;  // ������ ������� �����

    void Update()
    {
        if (waypoints.Length == 0) return;

        // �������� � ������� ����� ��������
        Transform target = waypoints[currentWaypoint];
        Vector3 direction = (target.position - transform.position).normalized;

        // ������� ������� � ����
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

        // ����������� � �����
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // ������� � ��������� �����, ���� ���������� �������
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}

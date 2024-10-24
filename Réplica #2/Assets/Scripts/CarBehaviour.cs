using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    public Transform car;
    public float speed = 5f;        // Car speed
    public float delay = 5f;         // Wait time

    private bool isMoving = false;

    void Start()
    {
        Invoke("StartMovingCar", delay);
    }

    void StartMovingCar()
    {
        isMoving = true;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            car.position += new Vector3(-speed, 0, 0);
        }
    }
}

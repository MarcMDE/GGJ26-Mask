using UnityEngine;
using System.Collections;
using System;
using UnityEngine;

public class RockingChair : MonoBehaviour
{
    [SerializeField] private float rockingSpeed = 1f;
    [SerializeField] private float rockingAngle = 15f;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * rockingSpeed) * rockingAngle;
        transform.rotation = initialRotation * Quaternion.Euler(0, angle, 0);
    }
}

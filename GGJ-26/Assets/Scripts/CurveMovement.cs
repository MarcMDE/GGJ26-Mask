using UnityEngine;

public class CurveMovement : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] private float strength;
    [SerializeField] private float speedFactor;

    private float offset = 0f;
    private Vector3 ogPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ogPosition = transform.position;
        offset = Random.Range(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = ogPosition + transform.forward * strength * curve.Evaluate(Time.time* speedFactor + offset);
    }
}

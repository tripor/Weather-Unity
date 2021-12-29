using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    public float speed = 1.0f;
    public float rotationSpeed = 1.0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float totalDistance;
    private float startingTime;
    private Vector3 rotateAmount;



    // Start is called before the first frame update
    void Start()
    {
        // Generate a new cloud "shape"
        transform.localScale = new Vector3(Random.Range(0.8f, 1.5f), Random.Range(0.8f, 1.5f), Random.Range(0.8f, 1.5f));
        rotateAmount = new Vector3(Random.Range(0f, 1f) * rotationSpeed, Random.Range(0f, 1f) * rotationSpeed, Random.Range(0f, 1f) * rotationSpeed);
        SetNewPosition();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceSoFar = (Time.time - startingTime) * speed;
        float percentageDistanceSoFar = distanceSoFar / totalDistance;
        transform.rotation *= Quaternion.Euler(rotateAmount);
        if (percentageDistanceSoFar >= 1)
            SetNewPosition();
        else
            transform.position = Vector3.Lerp(startPosition, endPosition, percentageDistanceSoFar);
    }
    /// <summary>
    /// Create a random new position for the cloud to move to
    /// </summary>
    private void SetNewPosition()
    {
        startPosition = transform.position;
        endPosition = new Vector3(Random.Range(-2f, 2f), transform.position.y, Random.Range(-2f, 2f));
        totalDistance = Vector3.Distance(startPosition, endPosition);
        startingTime = Time.time;
    }
}

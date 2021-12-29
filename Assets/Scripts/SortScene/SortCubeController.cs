using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortCubeController : MonoBehaviour, IComparable
{
    public float timeToReach = 5.0f;


    private int colorNumber = 1;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float time;
    private bool moving = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, time / timeToReach);
            if (time >= timeToReach) moving = false;
        }
    }
    /// <summary>
    /// Set this cube color
    /// </summary>
    /// <param name="randomColor">The cube color: 1 red, 2 green, 3 blue</param>
    public void SetColor(int randomColor)
    {
        colorNumber = randomColor;
        Color color = Color.red;
        if (randomColor == 2)
        {
            color = Color.green;
        }
        else if (randomColor == 3)
        {
            color = Color.blue;
        }
        var renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_BaseColor", color);
        renderer.material.SetColor("_EmissionColor", color);
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public int GetColorNumber()
    {
        return colorNumber;
    }
    /// <summary>
    /// Set my position to move to
    /// </summary>
    /// <param name="position"></param>
    public void MoveToPosition(Vector3 position)
    {
        time = 0;
        startPosition = this.transform.position;
        endPosition = position;
        moving = true;
    }
    /// <summary>
    /// Compare myself to other cubes to sort
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int CompareTo(object obj)
    {
        int otherNumber = ((SortCubeController)obj).GetColorNumber();
        if (colorNumber == otherNumber) return 0;
        else if (colorNumber < otherNumber) return -1;
        return 1;
    }
}

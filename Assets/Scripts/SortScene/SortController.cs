using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SortController : MonoBehaviour
{
    public GameObject cubePrefab;
    public Button sortButton;

    [SerializeField]
    private int cubeAmount = 64;
    [SerializeField]
    private float spaceBetweenCubes = 2f;
    [SerializeField]
    private float timeBetweenMovement = 0.5f;

    private List<SortCubeController> unsortedCubes;
    private List<SortCubeController> sortedCubes;
    private bool sortingColor = false;
    private int cubeIndex = 0;
    private int[] cubeColorIndex;
    private float time;


    // Start is called before the first frame update
    void Start()
    {
        unsortedCubes = new List<SortCubeController>();
        sortedCubes = new List<SortCubeController>();
        cubeColorIndex = new int[3];
        Array.Clear(cubeColorIndex, 0, 3);
        ResetSort();
    }

    // Update is called once per frame
    void Update()
    {
        if (sortingColor)
        {
            time += Time.deltaTime;
            if (time >= timeBetweenMovement)
            {
                time = 0;
                float x = cubeColorIndex[unsortedCubes[cubeIndex].GetColorNumber() - 1] * spaceBetweenCubes;
                float z = -(unsortedCubes[cubeIndex].GetColorNumber() + 1) * spaceBetweenCubes;
                unsortedCubes[cubeIndex].MoveToPosition(new Vector3(x, 0, z));
                cubeColorIndex[unsortedCubes[cubeIndex].GetColorNumber() - 1]++;
                cubeIndex++;
                if (cubeIndex >= cubeAmount)
                {
                    sortingColor = false;
                    MoveSortedCubes();
                }
            }
        }
    }

    public void SortClick()
    {
        sortButton.interactable = false;
        cubeIndex = 0;
        sortedCubes.Sort();
        sortingColor = true;
    }
    public void ResetClick()
    {
        ResetSort();
    }

    private void MoveSortedCubes()
    {
        for (int i = 0; i < sortedCubes.Count; i++)
        {
            sortedCubes[i].MoveToPosition(new Vector3(spaceBetweenCubes * i, 0, -spaceBetweenCubes));
        }
    }

    private void ResetSort()
    {
        sortingColor = false;
        cubeIndex = 0;
        sortButton.interactable = true;
        for (int i = 0; i < unsortedCubes.Count || i < sortedCubes.Count; i++)
        {
            if (unsortedCubes.Count > i) unsortedCubes[i].DestroySelf();
            if (sortedCubes.Count > i) sortedCubes[i].DestroySelf();
        }
        unsortedCubes = new List<SortCubeController>();
        sortedCubes = new List<SortCubeController>();
        for (int i = 0; i < cubeAmount; i++)
        {
            int randomColor = UnityEngine.Random.Range(1, 4);
            var unsortedCube = Instantiate(cubePrefab, new Vector3(spaceBetweenCubes * i, 0f, 0f), Quaternion.identity);
            var sortedCube = Instantiate(cubePrefab, new Vector3(spaceBetweenCubes * i, 0f, spaceBetweenCubes), Quaternion.identity);
            unsortedCube.GetComponent<SortCubeController>().SetColor(randomColor);
            unsortedCubes.Add(unsortedCube.GetComponent<SortCubeController>());
            sortedCube.GetComponent<SortCubeController>().SetColor(randomColor);
            sortedCubes.Add(sortedCube.GetComponent<SortCubeController>());
        }
        Array.Clear(cubeColorIndex, 0, 3);
    }
}

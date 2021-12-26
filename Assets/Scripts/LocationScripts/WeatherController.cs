using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public GameObject cloudParent;
    public GameObject cloudPrefab;

    [SerializeField]
    private int totalNumberOfClouds = 20;

    private List<GameObject> clouds;

    // Start is called before the first frame update
    void Start()
    {
        GenerateClouds();
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void GenerateClouds()
    {
        List<GameObject> cloudToRemove = new List<GameObject>();
        clouds = new List<GameObject>();
        foreach (Transform item in cloudParent.transform)
        {
            if (item.tag == "Cloud")
            {
                cloudToRemove.Add(item.gameObject);
            }
        }
        foreach (var cloud in cloudToRemove)
        {
            Destroy(cloud);
        }
        for (int i = 0; i < totalNumberOfClouds; i++)
        {
            var newCloud = Instantiate(cloudPrefab);
            newCloud.transform.parent = cloudParent.transform;
            newCloud.transform.position = new Vector3(Random.Range(-2f, 2f), cloudParent.transform.position.y, Random.Range(-2f, 2f));
            clouds.Add(newCloud);
        }
    }
}

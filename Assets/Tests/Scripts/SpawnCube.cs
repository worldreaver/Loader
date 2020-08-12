using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCube : MonoBehaviour
{
    public int numObjects = 100;
    public GameObject prefab;

    private void Start()
    {
        var center = transform.position;
        for (var i = 0; i < numObjects; i++)
        {
            var go = Instantiate(prefab, transform, false);
            go.transform.position = Random.insideUnitSphere * 10;
        }
    }
}
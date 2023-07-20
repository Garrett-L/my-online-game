using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInitialize : MonoBehaviour
{
    [SerializeField] private ObjectDatabase database;
    [SerializeField] private GameObject copy;
    [SerializeField] private PlayerBuilding playerRef;
    void Start()
    {
        for(int i = 0; i < database.objects.Length; i++) Instantiate(copy, transform).transform.GetComponent<BuildSelection>().playerRef = playerRef;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

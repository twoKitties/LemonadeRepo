using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerOnDemand : MonoBehaviour {

    [SerializeField]
    private GameObject prefab;

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}

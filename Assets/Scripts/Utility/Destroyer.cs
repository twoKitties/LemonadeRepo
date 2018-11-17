﻿using UnityEngine;

public class Destroyer : MonoBehaviour {

    [SerializeField]
    private float delay = 1f;

    private void Start()
    {
        Destroy(gameObject, delay);
    }
}

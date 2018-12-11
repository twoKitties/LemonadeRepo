using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInfo : MonoBehaviour
{
    [SerializeField]
    private Transform startPointer;
    [SerializeField]
    private Transform endPointer;
    [SerializeField]
    private float time;
    private Tile start = new Tile();
    private Tile end = new Tile();

    private void Awake()
    {
        start.tileCoords = startPointer.position;
        end.tileCoords = endPointer.position;
    }
    public void ExecuteEffect()
    {
        GetComponent<ParticleEmitterController>().SpawnEmitter(start, end, time);
    }
}

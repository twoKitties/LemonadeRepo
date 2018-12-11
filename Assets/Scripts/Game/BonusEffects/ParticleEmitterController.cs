using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitterController : MonoBehaviour
{
    [SerializeField]
    private GameObject emitterPrefab;

    [SerializeField]
    float time;
	
    public void SpawnEmitter(Tile start, Tile end, float timeMultiplier)
    {
        GameObject spawned = Instantiate(emitterPrefab, start.tileCoords, Quaternion.identity);
        var spawnedEmitter = spawned.GetComponent<BonusParticleEmitter>();
        spawnedEmitter.Init(time * timeMultiplier, end.tileCoords);
    }
}

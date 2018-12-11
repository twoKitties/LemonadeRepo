using UnityEngine;

public class HeadEmitter : MonoBehaviour
{
    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        if (!ps.IsAlive())
            Destroy(gameObject);
    }
}

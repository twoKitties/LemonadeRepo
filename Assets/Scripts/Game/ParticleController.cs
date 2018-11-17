using UnityEngine;

public class ParticleController : MonoBehaviour {

    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void LaunchBubbleWall()
    {
        ps.Play();
    }
}

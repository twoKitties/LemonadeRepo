using UnityEngine;

public class SuperBonusEmitter : BonusParticleEmitter
{
    [SerializeField]
    private GameObject headPrefab;
    private ParticleSystem ps;
    private bool isParticleCreated = false;

    protected override void Start()
    {
        base.Start();
        ps = GetComponent<ParticleSystem>();
    }
    protected override void MoveEmitterTest()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
            currentLerpTime = lerpTime;

        float perc = currentLerpTime / lerpTime;
        Vector2 position = Vector2.Lerp(startPosition, endPosition, perc);
        if (perc >= 1 && !isParticleCreated)
        {
            isParticleCreated = true;
            Instantiate(headPrefab, transform.position, Quaternion.identity);
        }

        transform.position = position;
    }

    protected override void Update()
    {
        MoveEmitterTest();
    }
}

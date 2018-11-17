using UnityEngine;

public class GameTileBurstController : MonoBehaviour {

    [SerializeField]
    private GameObject bubbleEmitterPrefab;

    public void DestroyObject()
    {
        Instantiate(bubbleEmitterPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

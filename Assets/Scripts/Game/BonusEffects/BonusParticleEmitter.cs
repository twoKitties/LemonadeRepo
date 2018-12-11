using UnityEngine;

public class BonusParticleEmitter : MonoBehaviour
{
    #region Test
    protected Vector2 startPosition;
    protected Vector2 endPosition;

    protected float lerpTime;
    protected float currentLerpTime;

    protected BoardController boardController;
    private int xCoords;
    private int yCoords;

    [SerializeField]
    protected float deathTime;

    protected virtual void Start()
    {
        startPosition = transform.position;

        boardController = FindObjectOfType<BoardController>();

        Destroy(gameObject, deathTime);
    }

    public virtual void Init(float time, Vector2 destination)
    {
        lerpTime = time;
        endPosition = destination;
    }

    protected virtual void MoveEmitterTest()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
            currentLerpTime = lerpTime;

        float perc = currentLerpTime / lerpTime;
        Vector2 position = Vector2.Lerp(startPosition, endPosition, perc);

        transform.position = position;
    }
    protected virtual void Update()
    {
        MoveEmitterTest();
        UpdateEffectCoords();
    }
    #endregion
    private void GetTileInfo()
    {
        if(boardController.Tiles[xCoords, yCoords].isSolid)
        {
            boardController.GetGameTile(boardController.Tiles[xCoords, yCoords].tileCoords).PlayHitAnimation();
        }
    }
    private void UpdateEffectCoords()
    {
        float distance = 0.3f;
        Vector2 currentPositionWorld = transform.position;
        for (int x = 0; x < boardController.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < boardController.Tiles.GetLength(1); y++)
            {
                float tmpDistance = Vector2.Distance(currentPositionWorld, boardController.Tiles[x, y].tileCoords);
                if (distance > tmpDistance)
                {
                    if (xCoords != x)
                    {
                        xCoords = x;
                        GetTileInfo();
                    }
                    if (yCoords != y)
                    {
                        yCoords = y;
                        GetTileInfo();
                    }
                }
            }
        }
    }
}

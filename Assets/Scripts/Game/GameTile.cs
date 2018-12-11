using UnityEngine;

public class GameTile : MonoBehaviour
{
    // Position of gametile in TileCoords
    //[HideInInspector]
    public int X, Y;
    //[HideInInspector]
    public TileColor tileColor;
    [HideInInspector]
    public Color GameTileColor;

    private ParticleSystem ps;    
    [SerializeField]
    private GameObject bonusRow;
    [SerializeField]
    private GameObject bonusColumn;
    [SerializeField]
    private GameObject bonusSuper;
    [SerializeField]
    private GameObject controlMark;
    private ParticleSystem psMark;
    [SerializeField]
    private GameObject gameTileBurst;

    private SpriteRenderer spriteRenderer;

    [HideInInspector]
    public float lerpTime = 0.8f;
    [HideInInspector]
    public float currentLerpTime;
    [HideInInspector]
    public int worldX;
    [HideInInspector]
    public int worldY;

    // startpos to place
    private Vector2 startPosition;
    // used in lerp to set transform
    [HideInInspector]
    public Vector2 targetPosition;
    // these two used in a calculation of isMoving
    [HideInInspector]
    public Vector2 currentPosition;
    [HideInInspector]
    public Vector2 previousPosition;

    private BoardController boardGenerator;
    private InputController controller;

    private bool isQuitting;
    //private bool isMerged = false;
    [HideInInspector]
    public bool HasColumnBonus;
    [HideInInspector]
    public bool HasRowBonus;
    [HideInInspector]
    public bool HasSuperBonus;
    [HideInInspector]
    public bool IsMoving;
    [HideInInspector]
    public bool IsControllable;
    [HideInInspector]
    public bool IsFirstRow;
    [HideInInspector]
    public bool isDragged;
    [HideInInspector]
    public bool isResetting;
    [HideInInspector]
    public bool IsStopped;

    private Animator anim;
    [SerializeField]
    private GameObject bubbleEmitter;

    private void Start()
    {
        boardGenerator = FindObjectOfType<BoardController>();
        UpdateTileCoords();
        psMark = controlMark.GetComponent<ParticleSystem>();
        anim = GetComponent<Animator>();

        Initialize();
        if (Y == 7 && boardGenerator.Tiles[X,Y - 1].isSolid)
        {
            StaticEventManager.CallOnGameLose();
        }
        if (IsControllable)
        {
            controlMark.SetActive(true);
            controller = GetComponent<InputController>();
            controller.enabled = true;
        }

        // Test
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (!isQuitting && !isResetting)
        {
            DestroyTile();
        }
    }

    private void DestroyTile()
    {
        int bonusPoints = 0;
        if (HasRowBonus || HasColumnBonus)
            bonusPoints = 1;
        if (HasSuperBonus)
            bonusPoints = 2;
        StaticEventManager.CallOnTileDead(bonusPoints);
        Instantiate(gameTileBurst, transform.position, Quaternion.identity);
        //StaticEventManager.CallOnTileDestroy(this);
        SoundPlayer.Play("tiledeath", 1);
    }
    public void SetTileCoords(int x, int y)
    {
        X = x;
        Y = y;
    }
    private void SetGameTileColor()
    {
        switch (tileColor)
        {
            case TileColor.Red:
                GameTileColor = GameTileColors.Red;
                break;
            case TileColor.Yellow:
                GameTileColor = GameTileColors.Yellow;
                break;
            case TileColor.Green:
                GameTileColor = GameTileColors.Green;
                break;
            case TileColor.LightBlue:
                GameTileColor = GameTileColors.LightBlue;
                break;
            case TileColor.Blue:
                GameTileColor = GameTileColors.Blue;
                break;
            case TileColor.Purple:
                GameTileColor = GameTileColors.Purple;
                break;
        }
    }

    private void Update()
    {
        UpdateTileCoords();
        //if (!IsFirstRow && !isDragged)
        //{
        //    MoveVertically();
        //}
        //SetTileState();
    }

    private void Initialize()
    {
        startPosition = boardGenerator.Tiles[X, Y].tileCoords;
        transform.position = startPosition;
        worldX = (int)boardGenerator.Tiles[X, Y].tileCoords.x;
        worldY = (int)boardGenerator.Tiles[X,Y].tileCoords.y;
        if (IsFirstRow)
            IsStopped = true;

        if (IsControllable)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
        }

        SetGameTileColor();
    }

    public void Move()
    {            
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
            currentLerpTime = lerpTime;

        float perc = currentLerpTime / lerpTime;
        float y = Mathf.Lerp(worldY, (worldY - 1), perc);
        targetPosition.Set(worldX, y);
        transform.position = targetPosition;
        if (perc >= 1)
        {
            //UpdateTileCoords();
            worldY = (int)boardGenerator.Tiles[X, Y].tileCoords.y;
            currentLerpTime = 0;
            IsMoving = false;

            // Remove is there'll be issues with tiles
            boardGenerator.Tiles[X, Y + 1].isFilled = false;
        }
    }

    public void DisableControlledUnit()
    {
        controller.enabled = false;
        IsControllable = false;
        var e = psMark.emission;
        e.rateOverTime = 0;
        spriteRenderer.sortingOrder = 0;
    }
    public void UpdateTileCoords()
    {
        float distance = 0.5f;
        Vector2 currentPositionWorld = transform.position;
        for (int x = 0; x < boardGenerator.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < boardGenerator.Tiles.GetLength(1); y++)
            {
                float tmpDistance = Vector2.Distance(currentPositionWorld, boardGenerator.Tiles[x, y].tileCoords);
                if(distance > tmpDistance)
                {
                    X = x;
                    Y = y;
                }
            }
        }
    }
    public void ActivateRowBonus()
    {
        HasRowBonus = true;
        bonusRow.SetActive(true);
        SoundPlayer.Play("bonus", 1);
    }
    public void ActivateColumnBonus()
    {
        HasColumnBonus = true;
        bonusColumn.SetActive(true);
        SoundPlayer.Play("bonus", 1);
    }
    public void ActivateSuperBonus()
    {
        HasSuperBonus = true;
        bonusSuper.SetActive(true);
        SoundPlayer.Play("bonus", 1);
    }
    public void PlayHitAnimation()
    {
        float r = Random.Range(-90f, 90f);
        Quaternion rotation = Quaternion.Euler(0, 0, r);
        transform.rotation = rotation;
        Debug.Log("animation hit activated");
        Instantiate(bubbleEmitter, transform.position, Quaternion.identity);
    }
    //private void OnMouseDown()
    //{
    //    //StaticEventManager.CallOnTileDestroy(this);
    //    //boardGenerator.Tiles[X, Y].isSolid = false;
    //    //Destroy(gameObject);
    //    ActivateSuperBonus();
    //}
}

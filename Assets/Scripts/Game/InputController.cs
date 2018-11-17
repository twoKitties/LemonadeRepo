using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

    //private int PointerX, PointerY;
    //private float precision = 0.5f;

    private float curLerpTime;
    private float lerpTime = 0.25f;

    private bool isStuck = true;

    private BoardController boardGenerator;
    private ParticleSystem pSystem;
    private GameTile myTile;
    private Camera cam;

    private Vector2 targetPosition;
    private Vector2 pointerPosition;
    private Vector2 curVelocity;
    [SerializeField]
    private float damp;
    [SerializeField]
    private float maxSpeed;

    private void Start()
    {
        boardGenerator = FindObjectOfType<BoardController>();

        cam = Camera.main;

        myTile = GetComponent<GameTile>();

        myTile.GetComponent<SpriteRenderer>().sortingOrder = 1;

        pSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
        var main = pSystem.main;
        main.startColor = myTile.GameTileColor;
    }

    private void Update()
    {
        UpdatePointerPosition();
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0) && !myTile.isDragged && !EventSystem.current.IsPointerOverGameObject())
        {
            myTile.isDragged = true;
            boardGenerator.Tiles[myTile.X, myTile.Y].isSolid = false;
        }

        if (Input.GetMouseButtonUp(0) && myTile.isDragged && !EventSystem.current.IsPointerOverGameObject())
        {
            CalculateOffset();
            //Tile tile = boardGenerator.GetUpperFreeTile(myTile.X);
            isStuck = false;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (Input.GetMouseButton(0) && !myTile.isDragged && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                myTile.isDragged = true;
                boardGenerator.Tiles[myTile.X, myTile.Y].isSolid = false;
            }
        }

        if (Input.touchCount > 0)
        {
            if (Input.GetMouseButtonUp(0) && myTile.isDragged && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                CalculateOffset();
                //Tile tile = boardGenerator.GetUpperFreeTile(myTile.X);
                isStuck = false;
            }
        }
#endif
        if (isStuck && myTile.isDragged)
            DragTile();

        if (!isStuck && myTile.isDragged)
            StickToGrid();
    }

    private void UpdatePointerPosition()
    {
        //float distance = precision;
#if UNITY_EDITOR || UNITY_STANDALONE
        pointerPosition = cam.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            pointerPosition = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif

        //for (int x = 0; x < boardGenerator.Tiles.GetLength(0); x++)
        //{
        //    for (int y = 0; y < boardGenerator.Tiles.GetLength(1); y++)
        //    {
        //        float tmpDistance = Vector2.Distance(pointerPosition, boardGenerator.Tiles[x, y].tileCoords);
        //        if (distance >= tmpDistance)
        //        {
        //            PointerX = x;
        //            PointerY = y;
        //        }
        //    }
        //}
    }

    private void DragTile()
    {
        Vector2 targetPosition = transform.position;
        Vector2 offset = Vector2.zero;
        targetPosition = Vector2.SmoothDamp(targetPosition, pointerPosition, ref curVelocity, damp, maxSpeed, Time.deltaTime);
        transform.position = targetPosition;
        //boardGenerator.FindDistanceToFallingTile(myTile);
    }

    private void CalculateOffset()
    {
        Vector2 currentPosition = transform.position;
        //Vector2 desiredPosition = boardGenerator.FindTilePosition(myTile);
        Vector2 desiredPosition = boardGenerator.FindTilePosition(myTile);
        Tile tile = boardGenerator.FindTileWithCoords(desiredPosition);
        if (boardGenerator.Tiles[tile.x, tile.y + 1].isFilled)
            boardGenerator.Tiles[tile.x, tile.y].isSolid = true;
        float distance = Vector2.Distance(currentPosition, desiredPosition);
        if (distance > 0f)
        {
            targetPosition = desiredPosition;
        }
    }

    private void StickToGrid()
    {
        curLerpTime += Time.deltaTime;
        if (curLerpTime > lerpTime)
            curLerpTime = lerpTime;
        float perc = curLerpTime / lerpTime;
        transform.position = Vector2.Lerp(transform.position, targetPosition, perc);
        if (perc >= 1f)
        {
            curLerpTime = 0f;
            myTile.previousPosition = transform.position;
            //Debug.Log("Ended lerp, drag is false now");           
            //myTile.UpdateTileCoords();
            myTile.worldX = (int)boardGenerator.Tiles[myTile.X, myTile.Y].tileCoords.x;
            myTile.worldY = (int)boardGenerator.Tiles[myTile.X, myTile.Y].tileCoords.y;
            myTile.currentLerpTime = 0f;
            myTile.isDragged = false;
            isStuck = true;
            myTile.IsMoving = false;
        }
    }
}

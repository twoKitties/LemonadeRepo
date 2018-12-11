﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public int x, y;
    public bool isFilled;       // This is representation of GameTile position in 2D Grid
    public bool isSolid;        // True if the tile can't be swaped by the controlled tile and will later be calculated for destruction
    public Vector2 tileCoords;
    public TileColor color;

    public Tile(int x, int y, bool isFilled, bool isSolid, Vector2 tileCoords)
    {
        this.x = x;
        this.y = y;
        this.isFilled = isFilled;
        this.isSolid = isSolid;
        this.tileCoords = tileCoords;
    }
    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Tile()
    {
    }
}
public enum TileColor
{
    Red = 0,
    Yellow = 1,
    Green = 2,
    LightBlue = 3,
    Blue = 4,
    Purple = 5,
    Blank = 6
}
public class BoardController : MonoBehaviour
{
    public GameStateStatus GameState;
    public bool IsPlaying;

    private GameObject spawnPrefab;

    [SerializeField]
    private GameObject[] gameTileRed;
    [SerializeField]
    private GameObject[] gameTileYellow;
    [SerializeField]
    private GameObject[] gameTileGreen;
    [SerializeField]
    private GameObject[] gameTileLightBlue;
    [SerializeField]
    private GameObject[] gameTileBlue;
    [SerializeField]
    private GameObject[] gameTilePurple;


    private int amountToSpawn = 3;
    private int difficulty = 0;
    private float gameTileSpeed = 0.8f;
    private float deltaSpeed = 0.1f;
    private Vector2 gridSize = new Vector2(5, 8);

    [HideInInspector]
    public List<GameTile> allGameTiles = new List<GameTile>();
    private List<GameTile> tmpAllGameTiles = new List<GameTile>(); 
    // Here go all matching joops
    private List<Tile> collector = new List<Tile>();
    // Stores info of current tile when looking for matches
    private Tile currentTile;

    //private TileColor[] startDifficultyColors =
    //{
    //    TileColor.Red,
    //    TileColor.Yellow,
    //    TileColor.Green,
    //    TileColor.Blue
    //};
    //private TileColor[] midDifficultyColors =
    //{
    //    TileColor.Red,
    //    TileColor.Yellow,
    //    TileColor.Green,
    //    TileColor.LightBlue,
    //    TileColor.Blue
    //};
    //private TileColor[] endDifficultyColors =
    //{
    //    TileColor.Red,
    //    TileColor.Yellow,
    //    TileColor.Green,
    //    TileColor.LightBlue,
    //    TileColor.Blue,
    //    TileColor.Purple
    //};
    //private List<TileColor> gameTileColorList = new List<TileColor>();
    private List<GameObject[]> gameTilesToSpawn = new List<GameObject[]>();
    // All possible spots for tiles in Tile Coords
    private int[] spawnSpotsArray = { 0, 1, 2, 3, 4 };
    private List<int> spawnSpotsList = new List<int>();
    private List<GameTile> fallingTiles = new List<GameTile>();

    private List<GameTile> tilesToDestroy = new List<GameTile>();

    private GameTile controlledUnit;

    [HideInInspector]
    public Tile[,] Tiles = new Tile[5, 8];
    // Copies tileboard here before checking for matches
    private Tile[,] tilesCopy = new Tile[5, 8];

    [SerializeField]
    private EffectsHolder effectsHolder;
    #region Current Add/Burn System
    private List<int> row = new List<int>();
    private List<int> column = new List<int>();
    private List<Vector2> superCoords = new List<Vector2>();
    private Dictionary<Vector2, List<Vector2>> coordsPair = new Dictionary<Vector2, List<Vector2>>();
    #endregion
    [SerializeField]
    private ParticleEmitterController activeBonusEmitter;

    private bool isBonusCharged = false;
    private bool isAddingRowBonusCount = false;
    private int rowBonusCount = 0;
    private int rowBonusAmount = 3;
    public int RowBonusAmount
    {
        get
        {
            if (PlayerPrefsHelper.HasKey("rowBonusAmount"))
            {
                int r = PlayerPrefsHelper.GetInt("rowBonusAmount");
                return r;
            }
            return rowBonusAmount;
        }
        set
        {
            PlayerPrefsHelper.SetInt("rowBonusAmount", value);
            rowBonusAmount = value;
        }
    }
    private int columnBonusAmount = 3;
    public int ColumnBonusAmount
    {
        get
        {
            if (PlayerPrefsHelper.HasKey("columnBonusAmount"))
            {
                int r = PlayerPrefsHelper.GetInt("columnBonusAmount");
                return r;
            }
            return columnBonusAmount;
        }
        set
        {
            PlayerPrefsHelper.SetInt("columnBonusAmount", value);
            columnBonusAmount = value;
        }
    }

    #region Tutorial
    private BonusButtons bonusButtons;
    #region StageSwipe
    public void InitializeTutorial()
    {
        Debug.Log("Tutorial Started");
        bonusButtons = FindObjectOfType<BonusButtons>();
        GenerateGrid();
        //StartCoroutine(CheckTileState());
    }
    public void SpawnForSwipeStage()
    {
        StartCoroutine(SpawnCoroutine());
    }
    private IEnumerator SpawnCoroutine()
    {
        ClearAllTiles();
        yield return new WaitForSeconds(0.5f);
        GameObject[] gameTiles = { gameTileRed[1], gameTileGreen[0], gameTileBlue[3], gameTileBlue[2], gameTileGreen[0] };
        int[] coords = { 0, 1, 2, 3, 4 };
        int rowNum = 0;
        SpawnRowOnDemand(rowNum, coords, gameTiles, true);
        yield return new WaitForSeconds(0.25f);
        fallingTiles.Clear();
        SpawnFallingOnDemand(1, gameTileBlue[1], false, false);
        SpawnFallingOnDemand(2, gameTileRed[0], true, false);
        SpawnFallingOnDemand(3, gameTileGreen[3], false, false);
        yield return new WaitForSeconds(0.25f);
        GameState = GameStateStatus.TilesMoving;
        StartCoroutine(MoveCheckCoroutine());
    }
    private IEnumerator MoveCheckCoroutine()
    {
        while (TilesMove())
            yield return null;
        yield return new WaitForSeconds(0.5f);
        int count = allGameTiles.Count;
        CalculateMatches();
        yield return new WaitForSeconds(1f);
        if (count != allGameTiles.Count)
            TutorialEventList.CallOnGameTilesBurned();
        else
            TutorialEventList.CallOnGameTilesReachedBottom();
        Debug.Log("fail event called");
    }
    #endregion
    #region StageBurn
    public void SpawnForBurnStage()
    {
        StartCoroutine(SpawnForBurnStageCoroutine());
    }
    private IEnumerator SpawnForBurnStageCoroutine()
    {
        ClearAllTiles();
        yield return new WaitForSeconds(0.5f);
        GameObject[] gameTiles = { gameTileRed[0], gameTileYellow[0], gameTileYellow[1], gameTileGreen[1] };
        int[] coords = { 0, 1, 3, 4 };
        int rowNum = 0;
        SpawnRowOnDemand(rowNum, coords, gameTiles, true);
        yield return new WaitForSeconds(0.25f);
        fallingTiles.Clear();
        SpawnFallingOnDemand(0, gameTileRed[2], false, false);
        SpawnFallingOnDemand(1, gameTileYellow[3], true, false);
        SpawnFallingOnDemand(4, gameTileRed[0], false, false);
        yield return new WaitForSeconds(0.25f);
        GameState = GameStateStatus.TilesMoving;
        StartCoroutine(MoveCheckCoroutine());
    }
    #endregion
    #region StageHorizontal
    public void SpawnForHorizontal()
    {
        StartCoroutine(SpawnForHorizontalCoroutine());
    }
    private IEnumerator SpawnForHorizontalCoroutine()
    {
        ClearAllTiles();
        yield return new WaitForSeconds(0.5f);
        GameObject[] gameTiles = { gameTileRed[0], gameTileGreen[2], gameTileGreen[1], gameTileGreen[1] };
        int[] coords = { 0, 1, 2, 4 };
        int rowNum = 0;
        SpawnRowOnDemand(rowNum, coords, gameTiles, true);
        GameObject[] gameTiles1 = { gameTileGreen[1], gameTileBlue[0], gameTileRed[1] };
        int[] coords1 = { 0, 1, 4 };
        int rowNum1 = 1;
        SpawnRowOnDemand(rowNum1, coords1, gameTiles1, false);
        GameObject[] gameTiles2 = { gameTileRed[3] };
        int[] coords2 = { 0 };
        int rowNum2 = 2;
        SpawnRowOnDemand(rowNum2, coords2, gameTiles2, false);
        yield return new WaitForSeconds(0.25f);
        GameTile gameTileWithBonus = allGameTiles.Find(item => item.X == 2 && item.Y == 0);
        gameTileWithBonus.ActivateRowBonus();
        fallingTiles.Clear();
        SpawnFallingOnDemand(0, gameTileBlue[2], false, false);
        SpawnFallingOnDemand(1, gameTileGreen[1], true, false);
        SpawnFallingOnDemand(4, gameTileBlue[0], false, false);
        yield return new WaitForSeconds(0.25f);
        GameState = GameStateStatus.TilesMoving;
        StartCoroutine(MoveCheckCoroutine());
    }
    #endregion
    #region StageVertical
    public void SpawnForVertical()
    {
        StartCoroutine(SpawnForVerticalCoroutine());
    }
    private IEnumerator SpawnForVerticalCoroutine()
    {
        ClearAllTiles();
        yield return new WaitForSeconds(0.5f);
        GameObject[] gameTiles = { gameTileRed[0], gameTileBlue[2], gameTileBlue[1], gameTileGreen[1], gameTileYellow[1] };
        int[] coords = { 0, 1, 2, 3, 4 };
        int rowNum = 0;
        SpawnRowOnDemand(rowNum, coords, gameTiles, true);
        GameObject[] gameTiles1 = { gameTileRed[1]};
        int[] coords1 = { 1};
        int rowNum1 = 1;
        SpawnRowOnDemand(rowNum1, coords1, gameTiles1, false);
        GameObject[] gameTiles2 = { gameTileYellow[3] };
        int[] coords2 = { 1 };
        int rowNum2 = 2;
        SpawnRowOnDemand(rowNum2, coords2, gameTiles2, false);
        yield return new WaitForSeconds(0.25f);
        GameTile gameTileWithBonus = allGameTiles.Find(item => item.X == 1 && item.Y == 0);
        gameTileWithBonus.ActivateColumnBonus();
        fallingTiles.Clear();
        SpawnFallingOnDemand(0, gameTileBlue[2], true, false);
        SpawnFallingOnDemand(2, gameTileYellow[1], false, false);
        SpawnFallingOnDemand(3, gameTileGreen[0], false, false);
        yield return new WaitForSeconds(0.25f);
        GameState = GameStateStatus.TilesMoving;
        StartCoroutine(MoveCheckCoroutine());
    }
    #endregion
    #region StageSuper
    public void SpawnForBonus()
    {
        StartCoroutine(SpawnForBonusCoroutine());
    }
    private IEnumerator SpawnForBonusCoroutine()
    {
        ClearAllTiles();
        yield return new WaitForSeconds(0.5f);
        GameObject[] gameTiles = { gameTileRed[0], gameTileBlue[2], gameTileBlue[1], gameTileYellow[1], gameTileGreen[1] };
        int[] coords = { 0, 1, 2, 3, 4 };
        int rowNum = 0;
        SpawnRowOnDemand(rowNum, coords, gameTiles, true);
        GameObject[] gameTiles1 = { gameTileYellow[1], gameTileGreen[0], gameTileGreen[3], gameTileRed[0], gameTileBlue[2] };
        int[] coords1 = { 0, 1, 2, 3, 4 };
        int rowNum1 = 1;
        SpawnRowOnDemand(rowNum1, coords1, gameTiles1, false);
        GameObject[] gameTiles2 = { gameTileRed[3], gameTileBlue[1], gameTileRed[0], gameTileYellow[3], gameTileGreen[2] };
        int[] coords2 = { 0, 1, 2, 3, 4 };
        int rowNum2 = 2;
        SpawnRowOnDemand(rowNum2, coords2, gameTiles2, false);
        GameObject[] gameTiles3 = { gameTileRed[2] };
        int[] coords3 = { 0 };
        int rowNum3 = 3;
        SpawnRowOnDemand(rowNum3, coords3, gameTiles3, false);
        yield return new WaitForSeconds(0.25f);
        fallingTiles.Clear();
        SpawnFallingOnDemand(0, gameTileGreen[2], false, false);
        SpawnFallingOnDemand(2, gameTileRed[1], true, true);
        SpawnFallingOnDemand(4, gameTileBlue[0], false, false);
        yield return new WaitForSeconds(0.25f);
        GameState = GameStateStatus.TilesMoving;
        StartCoroutine(MoveCheckCoroutine());
    }
    #endregion
    #region StageButtonBonusHorizontal
    public void SpawnForButtonHorizontal()
    {
        StartCoroutine(SpawnForButtonHorizontalCoroutine());
    }
    private IEnumerator SpawnForButtonHorizontalCoroutine()
    {
        ClearAllTiles();
        yield return new WaitForSeconds(0.5f);
        GameObject[] gameTiles = { gameTileRed[0], gameTileRed[2], gameTileBlue[1], gameTileYellow[1], gameTileBlue[1] };
        int[] coords = { 0, 1, 2, 3, 4 };
        int rowNum = 0;
        SpawnRowOnDemand(rowNum, coords, gameTiles, true);
        GameObject[] gameTiles1 = { gameTileGreen[1], gameTileGreen[2] };
        int[] coords1 = { 0, 1 };
        int rowNum1 = 1;
        SpawnRowOnDemand(rowNum1, coords1, gameTiles1, false);
        GameObject[] gameTiles2 = { gameTileRed[0] };
        int[] coords2 = { 1 };
        int rowNum2 = 2;
        SpawnRowOnDemand(rowNum2, coords2, gameTiles2, false);
        GameObject[] gameTiles3 = { gameTileBlue[1] };
        int[] coords3 = { 1 };
        int rowNum3 = 3;
        SpawnRowOnDemand(rowNum3, coords3, gameTiles3, false);
        yield return new WaitForSeconds(0.25f);
        fallingTiles.Clear();
        SpawnFallingOnDemand(2, gameTileGreen[2], true, false);
        SpawnFallingOnDemand(3, gameTileGreen[1], false, false);
        SpawnFallingOnDemand(4, gameTileYellow[0], false, false);
        yield return new WaitForSeconds(0.25f);
        GameState = GameStateStatus.TilesMoving;
        // Change this to another coroutine which gonna call button horizontal
        // Instead of completing tutorial
        // And after clicking button tutorial will be complete
        StartCoroutine(MoveCheckLoopCoroutine(0));
    }
    #endregion
    #region StageButtonBonusVertical
    public void SpawnForButtonVertical()
    {
        StartCoroutine(SpawnForButtonVerticalCoroutine());
    }
    private IEnumerator SpawnForButtonVerticalCoroutine()
    {
        ClearAllTiles();
        yield return new WaitForSeconds(0.5f);
        GameObject[] gameTiles = { gameTileBlue[0], gameTileBlue[2], gameTileYellow[1], gameTileGreen[1], gameTileGreen[1] };
        int[] coords = { 0, 1, 2, 3, 4 };
        int rowNum = 0;
        SpawnRowOnDemand(rowNum, coords, gameTiles, true);
        GameTile gameTileWithBonus = allGameTiles.Find(item => item.X == 0 && item.Y == 0);
        gameTileWithBonus.ActivateRowBonus();
        GameObject[] gameTiles1 = { gameTileBlue[1] };
        int[] coords1 = { 4 };
        int rowNum1 = 1;
        SpawnRowOnDemand(rowNum1, coords1, gameTiles1, false);
        GameObject[] gameTiles2 = { gameTileRed[2] };
        int[] coords2 = { 4 };
        int rowNum2 = 2;
        SpawnRowOnDemand(rowNum2, coords2, gameTiles2, false);
        GameObject[] gameTiles3 = { gameTileBlue[2] };
        int[] coords3 = { 4 };
        int rowNum3 = 3;
        SpawnRowOnDemand(rowNum3, coords3, gameTiles3, false);
        yield return new WaitForSeconds(0.25f);
        fallingTiles.Clear();
        SpawnFallingOnDemand(1, gameTileBlue[2], true, false);
        SpawnFallingOnDemand(2, gameTileRed[1], false, false);
        SpawnFallingOnDemand(3, gameTileRed[0], false, false);
        yield return new WaitForSeconds(0.25f);
        GameState = GameStateStatus.TilesMoving;
        // Change this to another coroutine which gonna call button horizontal
        // Instead of completing tutorial
        // And after clicking button tutorial will be complete
        StartCoroutine(MoveCheckLoopCoroutine(1));
    }
    #endregion
    private void SpawnRowOnDemand(int rowNumber, int[] xCoord, GameObject[] GameTiles, bool firstRow)
    {
        List<GameObject> rowObjects = new List<GameObject>();
        for (int i = 0; i < GameTiles.Length; i++)
            rowObjects.Add(GameTiles[i]);
        for (int x = 0; x < GameTiles.Length; x++)
        {
            GameObject candyGO = Instantiate(rowObjects[x], Tiles[xCoord[x], rowNumber].tileCoords, Quaternion.identity);
            candyGO.transform.parent = transform;
            GameTile candyTile = candyGO.GetComponent<GameTile>();
            if (firstRow)
            {
                candyTile.IsMoving = false;
                candyTile.IsFirstRow = true;
            }
            Tiles[xCoord[x], rowNumber].color = candyTile.tileColor;
            Tiles[xCoord[x], rowNumber].isSolid = true;
            allGameTiles.Add(candyTile);
        }
    }
    private void SpawnFallingOnDemand(int xCoord, GameObject GameTile, bool controllable, bool hasSuperBonus)
    {
        GameObject candyGO = Instantiate(GameTile, Tiles[xCoord, Tiles.GetLength(1) - 1].tileCoords, Quaternion.identity);
        candyGO.transform.parent = transform;
        GameTile candyTile = candyGO.GetComponent<GameTile>();
        candyTile.lerpTime = gameTileSpeed;
        candyTile.IsControllable = controllable;
        if (hasSuperBonus)
            candyTile.ActivateSuperBonus();
        if (controllable)
            controlledUnit = candyTile;
        else
            fallingTiles.Add(candyTile);

        //candyTile.SetTileCoords(xCoord, Tiles.GetLength(1) - 1);
        allGameTiles.Add(candyTile);
    }
    private IEnumerator MoveCheckLoopCoroutine(int bonusNumber)
    {
        while (TilesMove())
            yield return null;
        yield return new WaitForSeconds(0.5f);
        int count = allGameTiles.Count;
        CalculateMatches();
        yield return new WaitForSeconds(1f);
        while (TilesMove())
            yield return null;
        CalculateMatches();
        yield return new WaitForSeconds(0.5f);
        if (count - allGameTiles.Count > 5)
        {
            if (bonusNumber == 0)
            {
                if (BonusButtons.OnRowBonusActivation != null)
                    BonusButtons.OnRowBonusActivation();
            }
            else if (bonusNumber == 1)
            {
                if (BonusButtons.OnColumnBonusActivation != null)
                    BonusButtons.OnColumnBonusActivation();
            }
        }
        else
            TutorialEventList.CallOnGameTilesReachedBottom();
        Debug.Log("fail event called");
    }
    #endregion

    public void Initialize()
    {
        Debug.Log("Normal Game Started");
        bonusButtons = FindObjectOfType<BonusButtons>();
        //Reset();
        GenerateGrid();
        SpawnInitialRow();
        StartCoroutine(CheckTileState());
    }
    /// <summary>
    /// Clears all tiles and gametiles
    /// </summary>
    /// <param name="isRestart">If the game is restarted manually, set to true</param>
    public void ResetBoard(bool isRestart)
    {
        StopAllCoroutines();
        if (isRestart)
        {
            for (int i = 0; i < allGameTiles.Count; i++)
            {
                allGameTiles[i].isResetting = true;
                Destroy(allGameTiles[i].gameObject);
            }
            allGameTiles.Clear();
            collector.Clear();
            IsPlaying = true;
            gameTileSpeed = 0.8f;
            difficulty = 0;
        }
        else if (!isRestart)
            StartCoroutine(OnBoardReset());
    }

    public void PlayAgain()
    {
        ClearAllTiles();
        SpawnInitialRow();
        StartCoroutine(CheckTileState());
    }

    private IEnumerator OnBoardReset()
    {
        int count = allGameTiles.Count;
        for (int i = 0; i < count; i++)
        {
            allGameTiles[i].isResetting = true;
            Destroy(allGameTiles[i].gameObject);
            yield return new WaitForEndOfFrame();
        }

        allGameTiles.Clear();
        collector.Clear();
        IsPlaying = true;
        gameTileSpeed = 0.8f;
        difficulty = 0;
    }

    private IEnumerator CheckTileState()
    {
        var wait = new WaitForSeconds(0.3f);
        while (IsPlaying)
        {
            yield return wait;
            if (TilesMove())
                continue;
            else
            {
                CalculateMatches();
                if (rowBonusCount >= 2 && RowBonusAmount > 0 && !bonusButtons.RowBonusButton.activeSelf)
                {
                    if (BonusButtons.OnRowBonusActivation != null)
                        BonusButtons.OnRowBonusActivation();
                    rowBonusCount = 0;
                    isAddingRowBonusCount = false;
                }
                else if (isAddingRowBonusCount)
                    isAddingRowBonusCount = false;

                yield return wait;
                if (TilesMove() || GameState == GameStateStatus.EffectsWorking)
                    continue;
                else
                {
                    yield return wait;

                    rowBonusCount = 0;
                    isAddingRowBonusCount = false;

                    ClearFilledTiles();
                    AssignBonus();
                    SpawnRow();
                }
            }
        }
    }

    private bool TilesMove()
    {
        int movingCount = 0;
        for (int i = 0; i < allGameTiles.Count; i++)
        {
            if (!allGameTiles[i].IsStopped)
                movingCount++;
        }
        return (movingCount > 0) ? true : false;
    }

    public void SpawnRow()
    {
        fallingTiles.Clear();

        spawnSpotsList.Clear();
        spawnSpotsList = FillList(spawnSpotsArray, 1);
        PickTilesToSpawn();

        int bonusIndex = Random.Range(0, amountToSpawn);

        for (int x = 0; x < amountToSpawn; x++)
        {
            int spotIndex = Random.Range(0, spawnSpotsList.Count);

            int colorStack = Random.Range(0, gameTilesToSpawn.Count);
            GameObject[] pickedGameTiles = gameTilesToSpawn[colorStack];
            int randomGameTile = Random.Range(0, pickedGameTiles.Length);
            spawnPrefab = pickedGameTiles[randomGameTile];

            GameObject newGameObject = Instantiate(spawnPrefab, Tiles[spawnSpotsList[spotIndex], Tiles.GetLength(1) - 1].tileCoords, Quaternion.identity);
            newGameObject.transform.parent = transform;
            spawnSpotsList.RemoveAt(spotIndex);

            GameTile newGameTile = newGameObject.GetComponent<GameTile>();
            newGameTile.lerpTime = gameTileSpeed;
            if (x == amountToSpawn - 1)
            {
                newGameTile.IsControllable = true;
                controlledUnit = newGameTile;
            }
            else
                fallingTiles.Add(newGameTile);

            if(isBonusCharged && x == bonusIndex)
            {
                newGameTile.ActivateSuperBonus();
                isBonusCharged = false;
            }

            gameTilesToSpawn.RemoveAt(colorStack);
            allGameTiles.Add(newGameTile);
        }
    }

    private void SpawnInitialRow()
    {
        PickTilesToSpawn();

        for (int x = 0; x < gridSize.x; x++)
        {
            int colorStack = Random.Range(0, gameTilesToSpawn.Count);
            GameObject[] pickedGameTiles = gameTilesToSpawn[colorStack];
            int randomGameTile = Random.Range(0, pickedGameTiles.Length);
            spawnPrefab = pickedGameTiles[randomGameTile];

            GameObject newGameObject = Instantiate(spawnPrefab, Tiles[x, 0].tileCoords, Quaternion.identity);
            newGameObject.transform.parent = transform;

            GameTile newGameTile = newGameObject.GetComponent<GameTile>();
            newGameTile.IsMoving = false;
            newGameTile.IsFirstRow = true;
            Tiles[x, 0].color = newGameTile.tileColor;
            Tiles[x, 0].isSolid = true;

            gameTilesToSpawn.RemoveAt(colorStack);
            allGameTiles.Add(newGameTile);
        }
        //for (int x = 0; x < gridSize.x; x++)
        //{
        //    int joopsInColumn = Random.Range(0, 4);
        //    for (int y = 0; y < joopsInColumn; y++)
        //    {
        //        GameObject newJoop = Instantiate(joopPrefab, Tiles[x, y].tileCoords, Quaternion.identity);
        //        newJoop.transform.parent = transform;
        //        GameTile newGameTile = newJoop.GetComponent<GameTile>();
        //        int colorIndex = Random.Range(0, joopsColorsList.Count);
        //        newGameTile.SetGameTileColor(joopsColorsList[colorIndex]);
        //        newGameTile.IsMoving = false;
        //        joopsColorsList.RemoveAt(colorIndex);
        //        if (y == 0)
        //            newGameTile.IsFirstRow = true;
        //        Tiles[x, y].color = newGameTile.tileColor;
        //        Tiles[x, y].isSolid = true;
        //        allTiles.Add(newGameTile);

        //        if (joopsColorsList.Count < 1)
        //        {
        //            for (int i = 1; i < 4; i++)
        //            {
        //                TileColor lastColor = allTiles[allTiles.Count - i].tileColor;
        //                lastColor = RevertColor(lastColor);
        //                joopsColorsList.Add(lastColor);
        //            }
        //        }
        //    }
        //}
    }

    private void PickTilesToSpawn()
    {
        gameTilesToSpawn.Clear();

        switch (difficulty)
        {
            case 0:
                gameTilesToSpawn.Add(gameTileRed);
                gameTilesToSpawn.Add(gameTileRed);
                gameTilesToSpawn.Add(gameTileYellow);
                gameTilesToSpawn.Add(gameTileYellow);
                gameTilesToSpawn.Add(gameTileGreen);
                gameTilesToSpawn.Add(gameTileGreen);
                gameTilesToSpawn.Add(gameTileBlue);
                gameTilesToSpawn.Add(gameTileBlue);
                break;
            case 1:
                gameTilesToSpawn.Add(gameTileRed);
                gameTilesToSpawn.Add(gameTileRed);
                gameTilesToSpawn.Add(gameTileYellow);
                gameTilesToSpawn.Add(gameTileYellow);
                gameTilesToSpawn.Add(gameTileGreen);
                gameTilesToSpawn.Add(gameTileGreen);
                gameTilesToSpawn.Add(gameTileLightBlue);
                gameTilesToSpawn.Add(gameTileLightBlue);
                gameTilesToSpawn.Add(gameTileBlue);
                gameTilesToSpawn.Add(gameTileBlue);
                break;
            case 2:
                gameTilesToSpawn.Add(gameTileRed);
                gameTilesToSpawn.Add(gameTileRed);
                gameTilesToSpawn.Add(gameTileYellow);
                gameTilesToSpawn.Add(gameTileYellow);
                gameTilesToSpawn.Add(gameTileGreen);
                gameTilesToSpawn.Add(gameTileGreen);
                gameTilesToSpawn.Add(gameTileLightBlue);
                gameTilesToSpawn.Add(gameTileLightBlue);
                gameTilesToSpawn.Add(gameTileBlue);
                gameTilesToSpawn.Add(gameTileBlue);
                gameTilesToSpawn.Add(gameTilePurple);
                gameTilesToSpawn.Add(gameTilePurple);
                break;
        }
    }

    private TileColor RevertColor(TileColor color)
    {
        TileColor result = TileColor.Blank;
        switch ((int)color)
        {
            case (int)TileColor.Red:
                result = TileColor.Purple;
                break;
            case (int)TileColor.Yellow:
                result = TileColor.Blue;
                break;
            case (int)TileColor.Green:
                result = TileColor.LightBlue;
                break;
            case (int)TileColor.LightBlue:
                result = TileColor.Green;
                break;
            case (int)TileColor.Blue:
                result = TileColor.Yellow;
                break;
            case (int)TileColor.Purple:
                result = TileColor.Red;
                break;
        }
        return result;
    }

    public void SetDifficulty()
    {
        if (difficulty < 2)
            difficulty++;
        Debug.Log("boardController.difficulty: " + difficulty);
    }

    public void SetGameTilesSpeed()
    {
        if (gameTileSpeed > 0.4f)
        {
            gameTileSpeed -= deltaSpeed;
            for (int i = 0; i < allGameTiles.Count; i++)
            {
                allGameTiles[i].lerpTime = gameTileSpeed;
            }
        }
    }
    private List<T> FillList<T>(T[] arr, int iterations)
    {
        List<T> listToFill = new List<T>();
        for (int i = 0; i < iterations; i++)
        {
            listToFill.AddRange(arr);
        }
        return listToFill;
    }
    private List<T> FillList<T>(T[,] arr)
    {
        List<T> listToFill = new List<T>();
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                listToFill.Add(arr[i, j]);
            }
        }
        return listToFill;
    }
    /// <summary>
    /// While generic version ain't done, use this
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private List<Vector2> FillList(List<GameTile> list)
    {
        List<Vector2> listToFill = new List<Vector2>();
        for (int i = 0; i < list.Count; i++)
        {
            listToFill.Add(list[i].transform.position);
        }
        return listToFill;
    }

    public Tile FindTileWithCoords(Vector2 coords)
    {
        Tile tileToFind = new Tile();
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                if ((coords.x == Tiles[x, y].tileCoords.x) && (coords.y == Tiles[x, y].tileCoords.y))
                    tileToFind = Tiles[x, y];
            }
        }
        return tileToFind;
    }

    /// <summary>
    /// Finds GameTile with world coords
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public GameTile GetGameTile(Vector2 coords)
    {
        return allGameTiles.Find(item => (Vector2)item.transform.position == coords);
    }

    public Tile GetUpperFreeTile(int column)
    {
        int row = 7;
        for (int y = 0; y < Tiles.GetLength(1); y++)
        {
            if (!Tiles[column, y].isSolid && row > y)
                row = y;
        }
        Tile highestTile = new Tile(column, row);
        return highestTile;
    }
    /// <summary>
    /// Returns index of highest tile is a column
    /// </summary>
    /// <param name="gameTile"></param>
    /// <returns></returns>
    public int FindIndexOfUpperTile(GameTile gameTile)
    {
        tmpAllGameTiles.Clear();
        tmpAllGameTiles = allGameTiles.FindAll(item => item.X == gameTile.X);
        int y = -1;
        for (int i = 0; i < tmpAllGameTiles.Count; i++)
        {
            if (y < tmpAllGameTiles[i].Y && !ReferenceEquals(gameTile, tmpAllGameTiles[i]) && !tmpAllGameTiles[i].isDragged)
            {
                y = tmpAllGameTiles[i].Y;
                Debug.Log(tmpAllGameTiles[i].GameTileColor);
            }
        }
        return y;
    }    
    /// <summary>
    /// Returns closest free tile, also makes check whether there's a tile in range less than 0.5f 
    /// </summary>
    /// <param name="gameTile"></param>
    /// <returns></returns>
    public Vector2 FindTilePosition(GameTile gameTile)
    {
        float distance = 0.5f;
        for (int i = 0; i < fallingTiles.Count; i++)
        {
            float tmpDistance = Vector2.Distance(gameTile.transform.position, fallingTiles[i].transform.position);
            if (tmpDistance < distance/* && IsMixable(gameTile, fallingTiles[i])*/)
            {
                //Debug.Log("Mix allowed");
                //ChangeColor(gameTile, fallingTiles[i]);
                //allGameTiles.Remove(gameTile);
                //Destroy(gameTile.gameObject, 0.3f);
                //return fallingTiles[i].transform.position;
                return FindClosestFreeTile(Tiles, gameTile);
            }
        }
        return FindClosestFreeTile(Tiles, gameTile);
    }
    /// <summary>
    /// Returns world coords of closest free tile
    /// </summary>
    /// <param name="tiles">Grid of tiles</param>
    /// <param name="gameTile"></param>
    /// <returns></returns>
    public Vector2 FindClosestFreeTile(Tile[,] tiles, GameTile gameTile)
    {
        List<Tile> freeTiles = new List<Tile>();
        Tile[,] tmpTiles = tiles;
        for (int x = 0; x < tmpTiles.GetLength(0); x++)
        {
            for (int y = 0; y < tmpTiles.GetLength(1) - 1; y++)
            {
                if (tmpTiles[x, y].isFilled || tmpTiles[x, y].isSolid)
                    continue;
                else
                    freeTiles.Add(tmpTiles[x, y]);
            }
        }
        Vector2 closestTarget = Vector2.zero;
        float closesDstSqr = Mathf.Infinity;
        Vector2 currentPosition = gameTile.transform.position;

        foreach (Tile potentialTarget in freeTiles)
        {
            Vector2 directionToTarget = potentialTarget.tileCoords - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closesDstSqr)
            {
                closesDstSqr = dSqrToTarget;
                closestTarget = potentialTarget.tileCoords;
            }
        }
        //Debug.Log(closestTarget);
        return closestTarget;
    }

    private void ClearFilledTiles()
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                if (Tiles[x, y].isFilled && !Tiles[x, y].isSolid)
                {
                    Tiles[x, y].isFilled = false;
                    Tiles[x, y].color = TileColor.Blank;
                }
            }
        }
    }

    private void ClearAllTiles()
    {
        int width = Tiles.GetLength(0);
        int height = Tiles.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tiles[x, y].isFilled = false;
                Tiles[x, y].isSolid = false;
                Tiles[x, y].color = TileColor.Blank;
            }
        }
    }

    private void AssignBonus()
    {
        
        int r = Random.Range(0, allGameTiles.Count);
        if (r > 0)
        {
            if (Random.value > 0.5f)
            {
                if (!allGameTiles[r].HasRowBonus && !allGameTiles[r].HasColumnBonus && !allGameTiles[r].HasSuperBonus)
                {
                    List<GameTile> rowTiles = allGameTiles.FindAll(item => item.Y == allGameTiles[r].Y);
                    int count = rowTiles.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (rowTiles[i].HasRowBonus)
                            return;
                    }
                    allGameTiles[r].ActivateRowBonus();
                }
            }
            if (Random.value > 0.5f)
            {
                if (!allGameTiles[r].HasColumnBonus && !allGameTiles[r].HasRowBonus && !allGameTiles[r].HasSuperBonus)
                {
                    List<GameTile> columnTiles = allGameTiles.FindAll(item => item.X == allGameTiles[r].X);
                    int count = columnTiles.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (columnTiles[i].HasColumnBonus)
                            return;
                    }
                    allGameTiles[r].ActivateColumnBonus();
                }
            }
        }
    }

    private void AssignSuperBonus(List<Vector2> spots)
    {
        //Debug.Log("Asigning bonus");
        int r = Random.Range(0, spots.Count);

        PickTilesToSpawn();
        int colorStack = Random.Range(0, gameTilesToSpawn.Count);
        GameObject[] pickedGameTiles = gameTilesToSpawn[colorStack];
        int randomGameTile = Random.Range(0, pickedGameTiles.Length);
        spawnPrefab = pickedGameTiles[randomGameTile];
        GameObject superBonusObject = Instantiate(spawnPrefab, spots[r], Quaternion.identity);

        Tile tile = FindTileWithCoords(spots[r]);
        tile.isSolid = true;
        superBonusObject.transform.parent = transform;
        GameTile bonusTile = superBonusObject.GetComponent<GameTile>();
        bonusTile.ActivateSuperBonus();

        allGameTiles.Add(bonusTile);
    }

    private void GenerateGrid()
    {
        Vector2 startPosition = new Vector2(-gridSize.x / 2 + 0.5f,
                                            Camera.main.ViewportToWorldPoint(Vector2.zero).y + 1f);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 spawnPosition = new Vector2(startPosition.x + x, startPosition.y + y);
                Tile tile = new Tile(x, y, false, false, spawnPosition);
                tile.color = TileColor.Blank;
                Tiles[x, y] = tile;
            }
        }
    }

    private void CalculateMatches()
    {
        CopyBoard(Tiles, tilesCopy);
        collector.Clear();
        currentTile = null;

        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                TestTile(x, y);
                if(collector.Count>=3)
                {
                    GameState = GameStateStatus.EffectsWorking;
                    MatchLists();
                    // Row bonus counter
                    if (!isAddingRowBonusCount)
                    {
                        isAddingRowBonusCount = true;
                        rowBonusCount++;
                    }
                }
                currentTile = null;
                collector.Clear();
            }
        }
    }

    private void TestTile(int x, int y)
    {
        if (tilesCopy[x, y] == null)
        {
            return;
        }

        if (currentTile == null)
        {
            currentTile = tilesCopy[x, y];
            tilesCopy[x, y] = null;
            if (currentTile.isSolid)
                collector.Add(currentTile);
        }
        else if (currentTile.color != tilesCopy[x, y].color)
        {
            return;
        }
        else
        {
            if (tilesCopy[x, y].isSolid)
                collector.Add(tilesCopy[x, y]);
            tilesCopy[x, y] = null;
        }

        if (x > 0)
            TestTile(x - 1, y);
        if (y > 0)
            TestTile(x, y - 1);
        if (x < Tiles.GetLength(0) - 1)
            TestTile(x + 1, y);
        if (y < Tiles.GetLength(1) - 1)
            TestTile(x, y + 1);
    }

    private void MatchLists()
    {
        for (int i = 0; i < collector.Count; i++)
        {
            for (int j = 0; j < allGameTiles.Count; j++)
            {
                if ((collector[i].x == allGameTiles[j].X) && (collector[i].y == allGameTiles[j].Y))
                {
                    if (!tilesToDestroy.Contains(allGameTiles[j]))
                    {
                        tilesToDestroy.Add(allGameTiles[j]);
                        CheckForBonuses(allGameTiles[j]);
                    }
                }
            }
        }
        // Condition of playing animation
        // Play animation
        StartCoroutine(PlayDestroyAnimation());
    }
    #region Current Add/Burn System
    private void CheckForBonuses(GameTile gameTile)
    {
        if (gameTile.HasRowBonus)
        {
            if (!row.Contains(gameTile.Y))
                row.Add(gameTile.Y);

            List<GameTile> newListOfGameTiles = allGameTiles.FindAll(item => item.Y == gameTile.Y);
            for (int i = 0; i < newListOfGameTiles.Count; i++)
            {
                if (!tilesToDestroy.Contains(newListOfGameTiles[i]))
                {
                    tilesToDestroy.Add(newListOfGameTiles[i]);
                    CheckForBonuses(newListOfGameTiles[i]);
                }
                else
                    continue;
            }
        }
        else if (gameTile.HasColumnBonus)
        {
            if (!column.Contains(gameTile.X))
                column.Add(gameTile.X);

            List<GameTile> newList = allGameTiles.FindAll(item => item.X == gameTile.X);
            for (int i = 0; i < newList.Count; i++)
            {
                if (!tilesToDestroy.Contains(newList[i]))
                {
                    tilesToDestroy.Add(newList[i]);
                    CheckForBonuses(newList[i]);
                }
                else
                    continue;
            }
        }
        else if (gameTile.HasSuperBonus)
        {
            List<GameTile> newList = allGameTiles.FindAll(item => item.GameTileColor == gameTile.GameTileColor);

            if (!coordsPair.ContainsKey(gameTile.transform.position))
            {
                superCoords.Add(gameTile.transform.position);
                List<Vector2> coords = new List<Vector2>();
                for (int i = 0; i < newList.Count; i++)
                    coords.Add(newList[i].transform.position);

                coordsPair.Add(gameTile.transform.position, coords);
            }

            for (int i = 0; i < newList.Count; i++)
            {
                if (!tilesToDestroy.Contains(newList[i]))
                {
                    tilesToDestroy.Add(newList[i]);
                    CheckForBonuses(newList[i]);
                }
                else
                    continue;
            }
        }
        Debug.Log("CheckForBonuses() done");
    }
    private IEnumerator PlayDestroyAnimation()
    {
        // Show super
        for (int i = 0; i < coordsPair.Count; i++)
        {
            List<GameObject> sEffects = new List<GameObject>();
            for (int j = 0; j < coordsPair[superCoords[i]].Count; j++)
            {
                GameObject sEffect = Instantiate(effectsHolder.super, new Vector2(superCoords[i].x, superCoords[i].y), Quaternion.identity);
                sEffects.Add(sEffect);
            }
            yield return new WaitForSeconds(0.1f);

            int eCount = sEffects.Count;
            for (int k = 0; k < eCount; k++)
            {
                sEffects[k].transform.position = coordsPair[superCoords[i]][k];
            }

            SoundPlayer.Play("sfx_01", 1f);
            yield return new WaitForEndOfFrame();
            SoundPlayer.Play("sfx_01", 1f);
        }
        // Show horizontal
        for (int i = 0; i < row.Count; i++)
        {
            GameObject hEffect = Instantiate(effectsHolder.horizontal, Tiles[0, row[i]].tileCoords, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            SoundPlayer.Play("sfx_01", 1f);
            Debug.Log("row[" + i + "]" + "value is " + row[i]);
            hEffect.transform.position = Tiles[4, row[i]].tileCoords;
            Debug.Log("effect moved");
        }
        // Show vertical
        for (int i = 0; i < column.Count; i++)
        {
            GameObject vEffect = Instantiate(effectsHolder.vertical, Tiles[column[i], 0].tileCoords, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            SoundPlayer.Play("sfx_01", 1f);
            Debug.Log("column[" + i + "]" + "value is " + column[i]);
            vEffect.transform.position = Tiles[column[i], 6].tileCoords;
            Debug.Log("effect moved");
        }

        yield return new WaitForSeconds(0.45f);
        Debug.Log("PlayAnimation() done");
        DestroyTiles(tilesToDestroy);
    }
    #endregion
    #region New Add/Burn System
    List<GameTile> rowEffectStartPoints = new List<GameTile>();
    List<GameTile> columnEffectStartPoints = new List<GameTile>();
    List<GameTile> superEffectStartPoints = new List<GameTile>();
    private void CheckForBonusesAltered(GameTile gameTile)
    {
        if (gameTile.HasRowBonus)
        {
            if (!rowEffectStartPoints.Contains(gameTile))
                rowEffectStartPoints.Add(gameTile);

            List<GameTile> newlyAdded = allGameTiles.FindAll(item => item.Y == gameTile.Y);
            int count = newlyAdded.Count;
            for (int i = 0; i < count; i++)
            {
                if (!tilesToDestroy.Contains(newlyAdded[i]))
                {
                    tilesToDestroy.Add(newlyAdded[i]);
                    CheckForBonusesAltered(newlyAdded[i]);
                }
                else
                    continue;
            }
        }
        else if (gameTile.HasColumnBonus)
        {
            if (!columnEffectStartPoints.Contains(gameTile))
                columnEffectStartPoints.Add(gameTile);

            List<GameTile> newlyAdded = allGameTiles.FindAll(item => item.X == gameTile.X);
            int count = newlyAdded.Count;
            for (int i = 0; i < count; i++)
            {
                if (!tilesToDestroy.Contains(newlyAdded[i]))
                {
                    tilesToDestroy.Add(newlyAdded[i]);
                    CheckForBonusesAltered(newlyAdded[i]);
                }
                else
                    continue;
            }
        }
        else if (gameTile.HasSuperBonus)
        {
            if (!superEffectStartPoints.Contains(gameTile))
                superEffectStartPoints.Add(gameTile);
            List<GameTile> newlyAdded = allGameTiles.FindAll(item => item.GameTileColor == gameTile.GameTileColor);
            int count = newlyAdded.Count;
            for (int i = 0; i < count; i++)
            {
                if (!tilesToDestroy.Contains(newlyAdded[i]))
                {
                    tilesToDestroy.Add(newlyAdded[i]);
                }
            }
        }
    }
    #endregion
    private void DestroyTiles(List<GameTile> destroyTiles)
    {
        if (destroyTiles.Count > 5)
        {
            if (BonusButtons.OnColumnBonusActivation != null && ColumnBonusAmount > 0 && !bonusButtons.ColumnBonusButton.activeSelf && !GameController.IsTutorActive)
            {
                BonusButtons.OnColumnBonusActivation();
            }
            isBonusCharged = true;
        }

        for (int i = 0; i < destroyTiles.Count; i++)
        {
            Tiles[destroyTiles[i].X, destroyTiles[i].Y].isSolid = false;
            Tiles[destroyTiles[i].X, destroyTiles[i].Y].color = TileColor.Blank;

            allGameTiles.Remove(destroyTiles[i]);
            Destroy(destroyTiles[i].gameObject);
        }

        row.Clear();
        column.Clear();
        superCoords.Clear();
        coordsPair.Clear();
        destroyTiles.Clear();
        GameState = GameStateStatus.TilesMoving;
        Debug.Log("DestroyTiles() done");
    }
    
    public void BurnHorizontalManually()
    {
        TutorialEventList.CallOnBonusButtonHorizontalUsed();
        RowBonusAmount--;
        // Add row to the list
        List<GameTile> row = allGameTiles.FindAll(item => item.Y == 0);
        // Check only existing rows
        if (row.Count == 0)
            return;
        tilesToDestroy.AddRange(row);
        // Get spawn and end points for bonus effect
        // Activate effect
        int yCoord = row[0].Y;
        Tile[] effectPoints = { Tiles[4, yCoord], Tiles[0, yCoord] };
        activeBonusEmitter.SpawnEmitter(effectPoints[1], effectPoints[0], 0.6f);
        // Check for bonuses
        for (int i = 0; i < row.Count; i++)
            CheckForBonuses(row[i]);
        // Burn them 
        StartCoroutine(PlayDestroyAnimation());
    }
    public void BurnVerticalManually()
    {
        TutorialEventList.CallOnBonusButtonVerticalUsed();
        ColumnBonusAmount--;
        // Looking for solid Tiles and their representative in GameTiles
        List<GameTile> solidTiles = new List<GameTile>();
        for (int i = 0; i < Tiles.GetLength(0); i++)
        {
            for (int j = 0; j < Tiles.GetLength(1); j++)
            {
                if (Tiles[i, j].isSolid)
                {
                    GameTile gameTile = allGameTiles.Find(x => x.X == Tiles[i, j].x && x.Y == Tiles[i, j].y);
                    solidTiles.Add(gameTile);
                }
            }
        }
        if (solidTiles.Count == 0)
            return;
        // Find the highest GameTile
        int heighest = 0;
        for (int i = 0; i < solidTiles.Count; i++)
        {
            if(solidTiles[i].Y > heighest)
            {
                heighest = solidTiles[i].Y;
            }
        }
        List<GameTile> heighestTiles = solidTiles.FindAll(item => item.Y == heighest);
        GameTile heighestTile = solidTiles.Find(item => item.Y == heighest);
        // Find all tiles in the heighest column and add them to destroy list
        List<GameTile> columnToBurn = solidTiles.FindAll(item => item.X == heighestTiles[0].X);
        tilesToDestroy.AddRange(columnToBurn);

        // Get spawn and end points for bonus effect
        // Activate effect
        int xCoord = columnToBurn[0].X;
        Tile[] effectPoints = { Tiles[xCoord, 0], Tiles[xCoord, 7] };
        activeBonusEmitter.SpawnEmitter(effectPoints[1], effectPoints[0], 1f);

        for (int i = 0; i < columnToBurn.Count; i++)
        {
            CheckForBonuses(columnToBurn[i]);
        }
        StartCoroutine(PlayDestroyAnimation());
    }
    private void CopyBoard(Tile[,] origin, Tile[,] fake)
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                fake[x, y] = origin[x, y];
            }
        }
    }
}


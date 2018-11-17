using System.Collections;
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
    private List<int> row = new List<int>();
    private List<int> column = new List<int>();
    private List<Vector2> superCoords = new List<Vector2>();
    private Dictionary<Vector2, List<Vector2>> coordsPair = new Dictionary<Vector2, List<Vector2>>();

    private bool isBonusCharged = false;

    public void Initialize()
    {
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
                yield return wait;
                if (TilesMove() || GameState == GameStateStatus.EffectsWorking)
                    continue;
                else
                {
                    yield return wait;
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
    ///// <summary>
    ///// Returns true if two GameTiles can be mixed
    ///// </summary>
    ///// <param name="controllable"></param>
    ///// <param name="other"></param>
    ///// <returns></returns>
    //public bool IsMixable(GameTile controllable, GameTile other)
    //{
    //    if (controllable.GameTileColor == GameTileColors.Red &&
    //        (other.GameTileColor == GameTileColors.Green ||
    //         other.GameTileColor == GameTileColors.Blue))
    //        return true;
    //    else if (controllable.GameTileColor == GameTileColors.Blue &&
    //            (other.GameTileColor == GameTileColors.Red ||
    //             other.GameTileColor == GameTileColors.Green))
    //        return true;
    //    else if (controllable.GameTileColor == GameTileColors.Green &&
    //            (other.GameTileColor == GameTileColors.Red ||
    //             other.GameTileColor == GameTileColors.Blue))
    //        return true;
    //    else
    //        return false;
    //}

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
                    allGameTiles[r].ActivateRowBonus();
            }
            if (Random.value > 0.5f)
            {
                if (!allGameTiles[r].HasColumnBonus && !allGameTiles[r].HasRowBonus && !allGameTiles[r].HasSuperBonus)
                    allGameTiles[r].ActivateColumnBonus();
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
                        tilesToDestroy.Add(allGameTiles[j]);
                    CheckForBonuses(allGameTiles[j]);
                }
            }
        }
        // Condition of playing animation
        // Play animation
        StartCoroutine(PlayDestroyAnimation());
    }

    private void CheckForBonuses(GameTile gameTile)
    {
        if (gameTile.HasRowBonus)
        {
            if (!row.Contains(gameTile.Y))
                row.Add(gameTile.Y);

            List<GameTile> newListOfGameTiles = allGameTiles.FindAll(item => item.Y == gameTile.Y);
            int length = newListOfGameTiles.Count;
            for (int i = 0; i < length; i++)
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
            int length = newList.Count;
            for (int i = 0; i < length; i++)
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
                int coordsLength = newList.Count;
                for (int i = 0; i < coordsLength; i++)
                    coords.Add(newList[i].transform.position);

                coordsPair.Add(gameTile.transform.position, coords);
            }

            int length = newList.Count;
            for (int i = 0; i < length; i++)
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
    }

    private void DestroyTiles(List<GameTile> destroyTiles)
    {
        if (destroyTiles.Count > 5)
            isBonusCharged = true;

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
        }
        // Show vertical
        for (int i = 0; i < column.Count; i++)
        {
            GameObject vEffect = Instantiate(effectsHolder.vertical, Tiles[column[i], 0].tileCoords, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            SoundPlayer.Play("sfx_01", 1f);
            vEffect.transform.position = Tiles[column[i], 6].tileCoords;
        }

        yield return new WaitForSeconds(0.45f);
        DestroyTiles(tilesToDestroy);
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


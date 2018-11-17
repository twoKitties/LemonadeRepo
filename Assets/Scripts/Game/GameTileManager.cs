using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileManager : MonoBehaviour
{
    private BoardController boardController;
    private List<GameTile> gameTiles = new List<GameTile>();

    private void Start()
    {
        boardController = GetComponent<BoardController>();
        gameTiles = boardController.allGameTiles;
    }

    private void Update()
    {
        //if(boardController.GameState == GameStateStatus.TilesMoving)
        MoveTiles();
    }

    private void MoveTiles()
    {
        int count = gameTiles.Count;
        foreach (var item in gameTiles)
        {
            if(!item.IsFirstRow && !item.isDragged && !item.isResetting)
            {
                GameTile lowerTile = gameTiles.Find(coords => coords.X == item.X && coords.Y == item.Y - 1 && !coords.isDragged);
                if (lowerTile == null)
                {
                    if (item.Y == 0 && item.currentLerpTime == 0)
                    {
                        item.IsMoving = false;
                        item.IsFirstRow = true;
                        boardController.Tiles[item.X, item.Y].color = item.tileColor;
                        boardController.Tiles[item.X, item.Y].isSolid = true;
                        item.IsStopped = true;
                        if (item.IsControllable)
                            item.DisableControlledUnit();
                    }
                    else
                    {
                        item.IsMoving = true;
                        boardController.Tiles[item.X, item.Y].isFilled = true;
                        if (item.Y > 0)
                            boardController.Tiles[item.X, item.Y - 1].isFilled = true;
                        if (item.Y < 7)
                            boardController.Tiles[item.X, item.Y + 1].isFilled = false;
                        boardController.Tiles[item.X, item.Y].isSolid = false;
                        boardController.Tiles[item.X, item.Y].color = TileColor.Blank;
                        item.IsStopped = false;
                    }
                }
                else
                {

                    if ((Vector2)item.transform.position == boardController.Tiles[item.X, item.Y].tileCoords && item.currentLerpTime == 0)
                    {
                        if (item.IsControllable && !lowerTile.IsMoving)
                        {
                            item.DisableControlledUnit();
                        }
                        if (item.Y < 7 && item.IsMoving)
                            boardController.Tiles[item.X, item.Y + 1].isFilled = false;
                        boardController.Tiles[item.X, item.Y].color = item.tileColor;
                        boardController.Tiles[item.X, item.Y].isSolid = true;
                        item.IsStopped = true;
                    }
                }

                if (item.IsMoving)
                    item.Move();
            }
        }
    }
}

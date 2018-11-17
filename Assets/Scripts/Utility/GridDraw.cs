using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDraw : MonoBehaviour {

    [SerializeField]
    private BoardController boardController;

    private Tile[,] tiles;

    private void Start()
    {
        tiles = boardController.Tiles;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && boardController.enabled)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1) - 1; y++)
                {
                    Gizmos.color = DefineColor(tiles[x, y].color);

                    if (tiles[x, y].isFilled)
                        Gizmos.DrawWireSphere(tiles[x, y].tileCoords, 0.5f);

                    if (tiles[x, y].isSolid)
                        Gizmos.DrawWireCube(tiles[x, y].tileCoords, Vector3.one * 0.2f);

                    Gizmos.DrawWireCube(tiles[x, y].tileCoords, Vector3.one * 0.95f);
                }
            }
        }
    }

    private Color DefineColor(TileColor color)
    {
        Color resultColor;
        switch ((int)color)
        {
            case (int)TileColor.Red:
                resultColor = Color.red;
                break;
            case (int)TileColor.Yellow:
                resultColor = Color.yellow;
                break;
            case (int)TileColor.Green:
                resultColor = Color.green;
                break;
            case (int)TileColor.LightBlue:
                resultColor = Color.cyan;
                break;
            case (int)TileColor.Blue:
                resultColor = Color.blue;
                break;
            case (int)TileColor.Purple:
                resultColor = Color.magenta;
                break;
            default:
                resultColor = Color.white;
                break;
        }
        return resultColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows;
    public TileCell[] cells;
    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size/height;

    private void Awake() 
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start() 
    {
        for(int y = 0; y < rows.Length; y++)
        {
            for(int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    public TileCell RandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length - 1);
        int startIndex = index;
        while(! cells[index].empty)
        {
            index++;
            if(index >= cells.Length)
            {
                index = 0;
            }

            if(index == startIndex)
            {
                return null;
            }
        }
        return cells[index];
    }

    public TileCell GetCell(int x, int y)
    {
        if(x >= 0 && x < width && y >= 0 && y < height)
        {
            return rows[y].cells[x];
        } 
        else
        {
            return null;
        }
    }
    
    public TileCell GetAdjacentCell(TileCell currentCell, Vector2Int direction)
    {
        int x = currentCell.coordinates.x + direction.x;
        int y = currentCell.coordinates.y - direction.y;
        if(x >= 0 && x < width && y >= 0 && y < height)
        {
            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }

}

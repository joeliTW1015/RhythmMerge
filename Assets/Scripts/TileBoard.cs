using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] Tile tilePrefab;
    [HideInInspector] public List<Tile> tiles;
    [SerializeField] TileState[] tileStates;
    TileGrid grid;
    public bool wait;
    GameManager gameManager;
    MusicGameManager musicGameManager;
    bool doubleScore;
    int scoreSum;
    private void Awake() 
    {
        gameManager = FindAnyObjectByType<GameManager>();
        musicGameManager = FindAnyObjectByType<MusicGameManager>();
        grid = GetComponentInChildren<TileGrid>();  
        wait = false;  
        doubleScore = false;
        scoreSum = 0;
    }

    public void CreateTile()
    {
        TileState tileState;
        int number;

        if(Random.Range(0f,1f) >= 0.9f)
        {
            tileState = tileStates[1];
            number = 4;
        }
        else
        {
            tileState = tileStates[0];
            number = 2;
        }

        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileState, number);
        tile.SetSpawnPos(grid.RandomEmptyCell());
        tiles.Add(tile);
    }

    private void Update() 
    {
        if(!wait && !gameManager.paused && MusicGameManager.isPlaying)
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveBoard(Vector2Int.up, 0, 1, 1, 1);
            }
            else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveBoard(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveBoard(Vector2Int.left, 1, 1, 0, 1);
            }
            else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveBoard(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }

    }

    public void MoveBoard(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool triggerCD = false;
        //Debug.Log("Get a moving input ");
        for(int y = startY; y >= 0 && y < grid.height; y+= incrementY)
        {
            for(int x = startX; x >= 0 && x < grid.width; x += incrementX)
            {
                TileCell tileCell = grid.GetCell(x, y);
                if(!tileCell.empty)
                {
                    triggerCD |= MoveTile(direction, tileCell.tile);
                }
            }
        }
        if(triggerCD)
        {
            doubleScore = false;
            doubleScore = musicGameManager.ButtonPressed();
            if(doubleScore)
            {
                scoreSum *= 2;
            }
            gameManager.AddScore(scoreSum);
            scoreSum = 0;
            StartCoroutine(MoveCD());
        }
    }

    IEnumerator MoveCD()
    {
        wait = true;
        yield return new WaitForSeconds(Tile.maxMoveDuration);

        for(int i = 0; i < tiles.Count; i++)
        {
            tiles[i].haveMerged = false;
        }
        
        if(tiles.Count <= grid.size && GameManager.gameMod != 4)
        {
            CreateTile();
        }

        if(CheckGameOver())
        {
            gameManager.GameOver();
        }
        
        wait = false;
    }

    bool CheckGameOver()
    {
        if(tiles.Count < grid.size)
        {
            return false;
        }
        
        foreach(Tile tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if(up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if(down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if(left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if(right!= null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }

    public bool MoveTile(Vector2Int direction, Tile tile)
    {
        TileCell targetCell = null;
        TileCell adjacentCell = grid.GetAdjacentCell(tile.cell, direction);
        while(adjacentCell != null)
        {
            if(!adjacentCell.empty)
            {
                if(CanMerge(tile, adjacentCell.tile))
                {
                    MergeTile(tile, adjacentCell.tile);
                    return true;
                }
                break;
            }
            targetCell = adjacentCell;
            adjacentCell = grid.GetAdjacentCell(targetCell, direction);
        }

        if(targetCell!= null)
        {
            tile.MoveToCell(targetCell);
            return true;
        }
        return false;
    }

    bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.haveMerged;
    }

    void MergeTile(Tile a, Tile b)
    {
        scoreSum += b.number;
        tiles.Remove(a);
        a.Merge(b);
        b.haveMerged = true;
        int originalStateIndex = IndexOfState(b.state);
        b.SetState(tileStates[originalStateIndex == tileStates.Length - 1 ? originalStateIndex : originalStateIndex + 1], b.number * 2);
        //TODO : call merge animation function
    }

    int IndexOfState(TileState state)
    {
        for(int i = 0; i < tileStates.Length; i++)
        {
            if(tileStates[i] == state)
            {
                return i;
            }
        }
        return -1;
    }

    public void ClearBoard()
    {
        //TODO: 遊戲結束清除效果
        //temp!

        foreach(TileCell cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (Tile tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }
    
}

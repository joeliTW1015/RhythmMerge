using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    public static float maxMoveDuration = 0.1f;
    public TileCell cell;
    public TileState state;
    Image image;
    TextMeshProUGUI text;
    public int number;
    public bool haveMerged;
    private void Awake() 
    {
        haveMerged = false;
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
    }
    
    public void SetState(TileState _state, int _number)
    {
        this.state = _state;
        this.number = _number;
        image.color = state.tileColor;
        text.color = state.tileColor;
        text.text = number.ToString();
    }

    public void SetSpawnPos(TileCell _cell)
    {
        if(this.cell != null)
        {
            this.cell.tile= null;
        }
        
        cell = _cell;
        cell.tile = this;
        transform.position = cell.transform.position;
    }

    public void MoveToCell(TileCell targetCell)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }
        
        this.cell = targetCell;
        cell.tile = this;
        StartCoroutine(MovingAnimation(targetCell.transform.position, false));
        //Debug.Log("a tile has move to target cell");
    }

    public void Merge(Tile targetTile)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }
        StartCoroutine(MovingAnimation(targetTile.transform.position, true));
    }

    IEnumerator MovingAnimation(Vector3 to, bool isToMerge)
    {
        Vector3 startPos = transform.position;
        float moveDuration;
        if(Mathf.Abs(startPos.x - to.x) > 10)
        {
            moveDuration = maxMoveDuration * (Mathf.Abs(startPos.x - to.x) / 517.5f);
        }
        else
        {
            moveDuration = maxMoveDuration * (Mathf.Abs(startPos.y - to.y) / 517.5f);
        }
        
        float elapsed = 0;
        while(elapsed <= moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, to, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
        
        if(isToMerge)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates;
    public Tile tile;
    public bool empty => tile == null;
    
    /*Debug
    Image image;
    private void Start() 
    {
        image = GetComponent<Image>();
    }
    private void Update() 
    {
        if(empty)
        {
            image.color = Color.black;
        }
        else
        {
            image.color = Color.white;
        }
    }
    */
}

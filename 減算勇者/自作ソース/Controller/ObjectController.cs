using System;
using UnityEngine;


public class ObjectController : MonoBehaviour
{
    public enum ObjectType { Tree,TreasureBox,Fire};
    public ObjectType objectType;
    [NonSerialized]
    public Vector2Int position = Vector2Int.zero;
    [NonSerialized]
    public int areaNumber;

    private void Awake()
    {
        position = new Vector2Int((int)transform.position.x,(int)transform.position.z);
    }
}

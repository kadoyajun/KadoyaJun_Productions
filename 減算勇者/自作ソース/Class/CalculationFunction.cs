using UnityEngine;

namespace Genzan
{
    public static class CalFn
    {
        public static Vector2Int TrimCoordinate(Vector2Int vector2Int)
        {
            Vector2Int trimPos = new Vector2Int(vector2Int.x % CNum.ROW, vector2Int.y % CNum.COLUMN); 
            return trimPos;
        }
    }
}
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.visible = false;
    }
}

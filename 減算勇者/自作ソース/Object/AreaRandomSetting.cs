using UnityEngine;

public class AreaRandomSetting : MonoBehaviour
{
    public GameObject firstArea;

    [SerializeField]
    int firstAreaNumber = 0;

    [Tooltip("ランダム配置するエリア")]
    [SerializeField]
    GameObject[] area;

    [SerializeField]
    GameObject safeZone;

    [SerializeField]
    GameObject bossArea;
    void Awake()
    {
        bool[] isSet = new bool[area.Length]; //配置済みはtrue
        for (int i = 0; i < area.Length + 1; i++)
        {
             while (true)
             {
                   if(i == firstAreaNumber)
                   {
                       break;
                   }

                    //ランダム配置の実行
                    int randomValue = UnityEngine.Random.Range(0, area.Length);
                    if (!isSet[randomValue])
                    {
                        isSet[randomValue] = true;
                        int x = i / 3 * 10;
                        int y = 0;
                        int z = i % 3 * 10;
                        GameObject gameObject = Instantiate(area[randomValue], new Vector3(x, y, z), Quaternion.identity);
                        gameObject.name = area[randomValue].name;
                        gameObject.transform.parent = transform;
                        break;
                    }
             }
        }
        {
            int x = firstAreaNumber / 3 * 10;
            int y = 0;
            int z = firstAreaNumber % 3 * 10;
            GameObject gameObject = Instantiate(firstArea, new Vector3(x, y, z), Quaternion.identity);
            gameObject.name = firstArea.name;
            gameObject.transform.parent = transform;
            gameObject.transform.SetSiblingIndex(firstAreaNumber);
        }
        {
            GameObject gameObject = Instantiate(safeZone, new Vector3(10, 0, 30), Quaternion.identity);
            gameObject.name = safeZone.name;
            gameObject.transform.parent = transform;
        }
        {
            GameObject gameObject = Instantiate(bossArea, new Vector3(10, 0, 40), Quaternion.identity);
            gameObject.name = bossArea.name;
            gameObject.transform.parent = transform;
        }
    }
}

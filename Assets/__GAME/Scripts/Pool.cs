using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    static Pool instance;

    [SerializeField]
    GameObject prefab;

    static readonly List<GameObject> poolList = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        poolList.Clear();
    }

    public static GameObject GetObject(Vector2 position)
    {
        GameObject result = null;
        foreach (GameObject item in poolList)
        {
            if (item.activeInHierarchy)
                continue;

            result = item;
            break;
        }

        if (result == null)
        {
            result = Instantiate(instance.prefab, instance.transform);
            poolList.Add(result);
        }

        result.transform.position = position;   
        result.SetActive(true);

        return result;
    }
}

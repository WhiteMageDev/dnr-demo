using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public List<GameObject> Objects = new List<GameObject>();
    public static List<PoolList> Pools = new List<PoolList>();
    public int DefaultCount;

    public void CreatePool()
    {
        foreach (GameObject prefab in Objects)
        {
            string name = prefab.name;

            List<GameObject> list = new();
            for (int i = 0; i < DefaultCount; i++)
            {
                GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
                obj.SetActive(false);
                list.Add(obj);
            }
            PoolList poolList = new PoolList(name, list);
            Pools.Add(poolList);
        }
    }
    public static GameObject GetObject(string name)
    {
        PoolList outputList = null;
        foreach (var list in Pools)
        {
            if (list.Name != name) continue;
            outputList = list;
        }

        if(outputList != null)
        {
            List<GameObject> list = outputList.Objects;

            foreach (var obj in list)
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }

            GameObject newObj = Instantiate(outputList.Objects[0], outputList.Objects[0].transform.position, Quaternion.identity);
            newObj.SetActive(false);
            outputList.Objects.Add(newObj);

            return newObj;
        }
        else
        {
            throw new Exception($"PoolList Name: {name} not found.");
        }
    }
}
public static class GameObjectExtensions
{
    public static GameObject InitializeObject(this GameObject gameObject, Vector3 position)
    {
        gameObject.transform.position = position;
        gameObject.SetActive(true);
        return gameObject;
    }
    public static void UIPanelDisable(this GameObject gameObject, string trigger)
    {
        gameObject.GetComponent<Animator>().SetTrigger(trigger);
    }
}
public class PoolList
{
    public string Name {  get; set; }
    public List<GameObject> Objects {  get; set; }

    public PoolList(string name, List<GameObject> objects)
    {
        Name = name;
        Objects = objects;
    }
}

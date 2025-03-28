using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates objects during start up to prevent object instantiation during runtime, which prevents lags during runtime.
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    // Pooled object
    public GameObject pooledObject;

    // Amount of objects that will be pooled
    public int pooledAmount;

    // Parent object of the pooled objects
    public GameObject parentObject;

    // List of pooled objects
    private List<GameObject> pooledObjects;

    // For initialization
    void Start()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.transform.SetParent(parentObject.transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    /// <summary>
    /// Searchs in the list of pooled objects and returns the next available pooled object.
    /// </summary>
    /// <returns>Available pooled object.</returns>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //Is object not active in scene?
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        GameObject obj = Instantiate(pooledObject);
        obj.transform.SetParent(parentObject.transform);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }
}

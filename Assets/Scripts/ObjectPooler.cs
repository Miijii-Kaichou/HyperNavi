using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    private static ObjectPooler Instance;

    [System.Serializable]
    public class ObjectPoolItem
    {
        public string name;
        public int size;
        public GameObject prefab;
        public bool expandPool;
    }

    public List<ObjectPoolItem> itemsToPool;

    public static List<GameObject> pooledObjects;

    public bool spawnInParent = false;

    // Start is called before the first frame update

    public int poolIndex;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitObjectPooler();
    }


    void InitObjectPooler()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.size; i++)
            {
                GameObject newMember = Instantiate(item.prefab, spawnInParent ? transform.parent : null);
                newMember.SetActive(false);
                item.prefab.name = item.name;
                pooledObjects.Add(newMember);
            }
        }
    }

    public static GameObject GetMember(string name)
    {
        #region Iteration
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            GameObject pooledObject = pooledObjects[i];

            if (pooledObject != null &&
                !pooledObject.activeInHierarchy &&
                (name + "(Clone)") == pooledObject.name)
            {
                return pooledObject;
            }
        }
        #endregion

        foreach (ObjectPoolItem item in Instance.itemsToPool)
        {
            if (name == item.prefab.name && item.expandPool)
            {

                GameObject newMember = Instantiate(item.prefab);
                newMember.SetActive(false);
                pooledObjects.Add(newMember);
                return newMember;
            }
        }
        Debug.LogWarning("We couldn't find a prefab of this name " + name);
        return null;
    }

    public static GameObject GetMember<T>(string name, out T result) where T : Component
    {
        GameObject pooledObject = GetMember(name);
        result = (T)pooledObject.GetComponent(typeof(T));
        return pooledObject;
    }
}

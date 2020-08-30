using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    private static ObjectPooler Instance;

    [SerializeField]
    private bool IsSingleton = true;

    [System.Serializable]
    public class ObjectPoolItem
    {
        public string name;
        public int size;
        public GameObject prefab;
        public bool expandPool;
    }

    public List<ObjectPoolItem> itemsToPool;

    public static GameObject[] pooledObjects;

    public bool spawnInParent = false;

    public static Transform Parent;

    public static int size;

    static GameObject obj;

    // Start is called before the first frame update

    public int poolIndex;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (IsSingleton)
                DontDestroyOnLoad(Instance);
        }
        else
        {
            if (IsSingleton)
                Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitObjectPooler();
    }


    void InitObjectPooler()
    {
        Parent = transform.parent;
        size = itemsToPool.Count;
        pooledObjects = new GameObject[size];
        for (int iter = 0; iter < size; iter++)
        { 
            ObjectPoolItem item = itemsToPool[iter];
            item.prefab.name = item.name;
            for (int i = 0; i < item.size; i++)
            {
                GameObject newMember = Instantiate(item.prefab, spawnInParent ? Parent : null);
                newMember.SetActive(false);
                pooledObjects[iter] = newMember;
            }
        }
    }

    public static GameObject GetMember(string name)
    {
        #region Iteration
        GameObject pooledObject;
        for (int iter = 0; iter < pooledObjects.Length; iter++)
        {
            pooledObject = pooledObjects[iter];

            if (pooledObject != null &&
                !pooledObject.Active() &&
                (name + "(Clone)") == pooledObject.name)
            {
                return pooledObject;
            }
        }
        #endregion

        GameObject newMember;
        for(int iter = 0; iter < Instance.itemsToPool.Count; iter++)
        {
                ObjectPoolItem item = Instance.itemsToPool[iter];
            if (name == item.prefab.name && item.expandPool)
            {
                //We'll have to rewrite array!

                newMember = Instantiate(item.prefab);
                newMember.SetActive(false);
                size++;
                Array.Resize(ref pooledObjects, size);
                pooledObjects[size-1] = newMember;
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

    public static void ClearPool()
    {
        for (int iter = 0; iter < pooledObjects.Length; iter++)
        {
            obj = pooledObjects[iter];
            obj.transform.position = Vector2.zero;
            obj.transform.parent = Parent;
            obj.transform.rotation = Quaternion.identity;
            obj.SetActive(false);
        }
    }

    public static Vector3 Position() => Instance.transform.localPosition;

    public static bool Exists() => Instance != null;
}

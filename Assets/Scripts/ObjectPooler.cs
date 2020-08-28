using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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

    public static Transform Parent;

    // Start is called before the first frame update

    public int poolIndex;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
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
        Parent = transform.parent;
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.size; i++)
            {
                GameObject newMember = Instantiate(item.prefab, spawnInParent ? Parent : null);
                newMember.SetActive(false);
                item.prefab.name = item.name;
                pooledObjects.Add(newMember);
            }
        }
    }

    public static GameObject GetMember(string name)
    {
        #region Iteration
        GameObject pooledObject;
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            pooledObject = pooledObjects[i];

            if (pooledObject != null &&
                !pooledObject.activeInHierarchy &&
                (name + "(Clone)") == pooledObject.name)
            {
                return pooledObject;
            }
        }
        #endregion

        GameObject newMember;
        foreach (ObjectPoolItem item in Instance.itemsToPool)
        {
            if (name == item.prefab.name && item.expandPool)
            {

                newMember = Instantiate(item.prefab);
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

    public static void ClearPool()
    {
        for (int iter = 0; iter < pooledObjects.Count; iter++)
        {
            GameObject obj = pooledObjects[iter];
            obj.transform.position = Vector2.zero;
            obj.transform.parent = Parent;
            obj.transform.rotation = Quaternion.identity;
            obj.SetActive(false);
        }
    }

    public static Vector3 Position() => Instance.transform.localPosition;
}

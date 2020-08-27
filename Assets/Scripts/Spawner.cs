using UnityEngine;
public abstract class Spawner : MonoBehaviour
{
    [SerializeField]
    protected string objectIdentifier;

    [SerializeField]
    protected Transform spawnUnder;

    protected virtual void OnEnable()
    {

    }

    public virtual void OnInit()
    {
        InstantiateObject();
    }

    void InstantiateObject()
    {
        //Uses object pooler instead
        GameObject _object = ObjectPooler.GetMember(objectIdentifier);

        if (_object.NotUsed())
        {
            _object.SetActive(true);

            //Object Placement
            _object.transform.position = transform.position;
            _object.transform.rotation = Quaternion.identity;
        }
    }
}

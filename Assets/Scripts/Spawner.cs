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

    public virtual void InstantiateObject()
    {
        //Uses object pooler instead
        GameObject _object = ObjectPooler.GetMember(objectIdentifier);

        if (_object.NotUsed())
        {
            _object.SetActive(true);
            _object.transform.parent = spawnUnder;
            //Object Placement
            _object.transform.localPosition = transform.localPosition;
            _object.transform.rotation = Quaternion.identity;
        }
    }
}

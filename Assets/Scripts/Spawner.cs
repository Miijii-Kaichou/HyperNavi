using UnityEngine;
public abstract class Spawner : MonoBehaviour
{
    [SerializeField]
    protected GameObject _object;

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
        _object = ObjectPooler.GetMember(_object.name + "(Clone)");

        if (_object.NotUsed())
        {
            _object.SetActive(true);

            //Object Placement
            _object.transform.position = transform.position;
            _object.transform.rotation = Quaternion.identity;
        }
    }
}

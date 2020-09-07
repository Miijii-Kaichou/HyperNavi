using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(DistanceCheck))]
public class PickUp : MonoBehaviour, IRange
{
    [SerializeField]
    private UnityEvent @onPickUp = new UnityEvent();

    [SerializeField]
    private string pickIdentifier;

    //Existing Pickup
    private const string GEM_PICKUP = "GEM_PICKUP";
    private const string BOOST_PICKUP = "BOOST_PICKUP";

    void Start()
    {
        ValidatePreExstingIdentifier(pickIdentifier);
    }

    void ValidatePreExstingIdentifier(string input)
    {
        if(input == GEM_PICKUP)
        {
            //Add listener to event
            onPickUp.AddListener(() =>
            {
                CurrencySystem.AddToBalance(1);
                BoostMeter.ReplenishBoost(10f);
                transform.position = Vector2.zero;
                transform.parent = ObjectPooler.Parent;
                transform.rotation = Quaternion.identity;
                gameObject.SetActive(false);
            });
        }
    }

    public void OnRangeEnter()
    {
        if(GameManager.player != null)
            onPickUp.Invoke();
    }

    public void OnRangeExit()
    {
        
    }
}

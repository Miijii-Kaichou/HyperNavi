using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PickUp : MonoBehaviour
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            onPickUp.Invoke();
    }

    void ValidatePreExstingIdentifier(string input)
    {
        if(input == GEM_PICKUP)
        {
            //Add listener to event
            onPickUp.AddListener(() =>
            {
                CurrencySystem.AddToBalance(1);
                transform.position = Vector2.zero;
                transform.parent = ObjectPooler.Parent;
                transform.rotation = Quaternion.identity;
                gameObject.SetActive(false);
            });
        }
    }
}

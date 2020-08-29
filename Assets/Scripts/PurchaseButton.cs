using UnityEngine;

public class PurchaseButton : MonoBehaviour
{
    public enum PurchaseType
    {
        GEMS_1000,
        GEMS_5000,
        GEMS_10000
    }

    [SerializeField]
    private PurchaseType purchaseType;

    /// <summary>
    /// Even for Clicking on a Button for Puchasing a Product
    /// </summary>
    public void ClickPurchaseButton()
    {
        /*Checking Purchasing Type*/
        switch (purchaseType)
        {
            //Purchasing 1000 Gems
            case PurchaseType.GEMS_1000:
                IAPManager.Buy1000Gems();
                break;

            //Purchasing 5000 Gems
            case PurchaseType.GEMS_5000:
                IAPManager.Buy5000Gems();
                break;

            //Purchasing 10000 Gems
            case PurchaseType.GEMS_10000:
                IAPManager.Buy10000Gems();
                break;

            default:
                break;
        }
    }
}

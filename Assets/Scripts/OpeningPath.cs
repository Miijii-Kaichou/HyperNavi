using UnityEngine;

public class OpeningPath : MonoBehaviour
{
    [SerializeField]
    SignalLayoutSpawn signalTrigger;
    
    // A component used to specify what paths are open
    // This is used to connect layout types
    // For example, if we have a layout with a left opening, we need to
    // enable a layout with a right opening, and thus connect the two
    // based on how much from the x or y position it has to be enabled
    // relatived to the enabled layout.

    [SerializeField]
    private bool leftOpen, rightOpen, topOpen, bottomOpen;

    [SerializeField]
    private float xUnit, yUnit;

    public bool IsLeftOpen() => leftOpen;
    public bool IsRightOpen() => rightOpen;
    public bool IsTopOpen() => topOpen;
    public bool IsBottomOpen() => bottomOpen;
    public Vector3 GetSignalTriggerPosition()
    {
        return signalTrigger.transform.localPosition;
    }
    public SignalLayoutSpawn GetSignal() => signalTrigger;
    public float GetXUnit() => xUnit;
    public float GetYUnit() => yUnit;
}

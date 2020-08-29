using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TouchDetection : MonoBehaviour
{
    public enum InputType
    {
        Tap,
        DoubleTap,
        Slide
    }

    [SerializeField]
    private bool debug;

    [SerializeField]
    Camera mainCamera;

    public int touchId = 0;

    [SerializeField]
    TextMeshProUGUI testMeshProUGUI;

    [SerializeField]
    private Collider2D touchArea;

    [SerializeField]
    private InputType touchInputType;

    [SerializeField, Range(0.1f, 1f)]
    private float slideDeltaThreshold = 0.1f;

    [SerializeField]
    private UnityEvent onTap, onDoubleTap, onSlide;

    //Current position
    private float touchX, touchY;

    //Previous position
    private float previousTouchX, previousTouchY;

    //Difference between previous and current touch
    private float diffTouchX, diffTouchY;

    private float time = 0;
    private float updateDeltaTime = 0;
    private float lastTimeSinceTouch;
    private float timeDiff;

    private bool doubleTapDetected;
    private bool horizontalSlideDetected;
    private bool verticalSlideDetected;
    private bool tapDetected;

    private bool touchIsDown = false;

    public const float RESET = 0;

    Vector3 viewportPoint;
    Vector2 touchPoint;
    LayerMask layerMask;

    private void Awake()
    {
        //Set the layer mask to detect
        // Shift by 11 bits to the left
        layerMask = 1 << 11; //This represents the Touch Input Layer
    }

    // Start is called before the first frame update
    void Start()
    {
        //Start the coroutine of receiving touch input from the user
        StartCoroutine(InputCycle());

        //Initialize
        Init();
    }

    /// <summary>
    /// The cycle of receiving input from the user
    /// </summary>
    /// <returns></returns>
    IEnumerator InputCycle()
    {
        while (true)
        {
            switch (touchInputType)
            {
                case InputType.Tap:
                    TapDetection();
                    break;
                case InputType.DoubleTap:
                    DoubleTapDetection();
                    break;
                case InputType.Slide:
                    TouchPositionDeltaDetection();
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Check if player is touching the specified area
    /// </summary>
    /// <returns></returns>
    bool OnTouchArea()
    {
        if (Input.touchCount > 0)
        {
            for (int iter = 0; iter < Input.touches.Length; iter++)
            {
                Touch touch = Input.touches[iter];
                viewportPoint = mainCamera.ScreenToViewportPoint(touch.position);
                touchX = viewportPoint.x;
                touchY = viewportPoint.y;
                touchPoint = new Vector2(touchX, touchY);

                if (touchArea == Physics2D.OverlapPoint(mainCamera.ViewportToWorldPoint(touchPoint), layerMask))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Calculate the touch delta for indicating sliding movement
    /// </summary>
    void TouchPositionDeltaDetection()
    {

        UpdatePreviousTouch();

        if (!touchIsDown)
            touchIsDown = true;

        if (OnTouchArea())
        {
            updateDeltaTime += Time.deltaTime;
            CalculateTouchDiff();
        }

        if (!OnTouchArea())
        {
            touchIsDown = false;
            ResetTouchPosition();
        }

    }

    /// <summary>
    /// Record the timing between each tap to indicate
    /// if it's a double tap or not
    /// </summary>
    void DoubleTapDetection()
    {
        //Record time
        time += Time.deltaTime;

        //If receiving touch on touch area and is not down
        if (OnTouchArea() && !touchIsDown)
        {
            //Calculate the difference between taps
            CalculateTimeDiff();

            //Update the last time a touch has been received
            lastTimeSinceTouch = time;

            //Mark that a touch has been detected
            touchIsDown = true;
        }

        if (!OnTouchArea())
            touchIsDown = false;

    }

    /// <summary>
    /// Reset touch position
    /// </summary>
    void ResetTouchPosition()
    {
        touchX = RESET;
        touchY = touchX;
        previousTouchX = touchX;
        previousTouchY = touchY;
        diffTouchX = touchX;
        diffTouchY = touchX;
    }

    /// <summary>
    /// Detect normal tapping
    /// </summary>
    void TapDetection()
    {
        //If on the touch area and touch is not down
        if (OnTouchArea() && !touchIsDown)
        {
            //Set it to true
            touchIsDown = true;
            tapDetected = touchIsDown;
        }
        //Otherwise... if not on touching area
        if (!OnTouchArea())
            //No one is touching the screen
            touchIsDown = false;

    }

    /// <summary>
    /// Set the dead time for tapping
    /// </summary>
    /// <returns></returns>
    IEnumerator TapDeadTime()
    {
        float time = 0;
        float deadTime = 0.1f;
        while (true)
        {
            //If a tap has been detected 
            //record time
            if (tapDetected)
                time += Time.deltaTime;

            //If the time is greater than our dead
            //time, turn off tap
            if (time >= deadTime)
            {
                tapDetected = false;
                time = 0;
            }

            yield return null;
        }
    }

    //Update previous touch
    void UpdatePreviousTouch()
    {
        previousTouchX = touchX;
        previousTouchY = touchY;
    }

    //Calculate difference in touch positions
    void CalculateTouchDiff()
    {
        diffTouchX = touchX - previousTouchX;
        diffTouchY = touchY - previousTouchY;

        horizontalSlideDetected = (Mathf.Abs(diffTouchX) >= slideDeltaThreshold / 100);
        verticalSlideDetected = (Mathf.Abs(diffTouchY) >= slideDeltaThreshold / 100);
    }

    //Calculate difference in time since a touch was detected
    void CalculateTimeDiff()
    {
        timeDiff = time - lastTimeSinceTouch;

        if (!touchIsDown)
        {
            doubleTapDetected = (timeDiff < 0.2f && timeDiff > 0.1f);

            if (doubleTapDetected)
                touchIsDown = true;
        }
    }

    /// <summary>
    /// Initialize Coroutines based on enumerator
    /// </summary>
    void Init()
    {
        if (debug)
            StartCoroutine(UpdateDebugUI());

        //Set previous touch to current
        UpdatePreviousTouch();

        if (touchInputType == InputType.Tap)
            StartCoroutine(TapDeadTime());
    }

    IEnumerator UpdateDebugUI()
    {
        while (true)
        {
            if (testMeshProUGUI != null)
            {
                testMeshProUGUI.text = string.Format("TouchX: {0}\n" +
                    "TouchY: {1}\n" +
                    "PreviousTouchX: {2}\n" +
                    "PreviousTouchY: {3}\n" +
                    "diffTouchX: {4}\n" +
                    "diffTouchY: {5}\n", touchX, touchY, previousTouchX, previousTouchY, diffTouchX, diffTouchY);
            }
            yield return new WaitForSeconds(1 / 64);
        }
    }

    public bool Tap() => tapDetected;
    public bool SlideLeft() => (horizontalSlideDetected && !verticalSlideDetected && diffTouchX < -slideDeltaThreshold / 100);
    public bool SlideRight() => (horizontalSlideDetected && !verticalSlideDetected && diffTouchX > slideDeltaThreshold / 100);
    public bool SlideUp() => (verticalSlideDetected && !horizontalSlideDetected && diffTouchY > slideDeltaThreshold / 100);
    public bool SlideDown() => (verticalSlideDetected && !horizontalSlideDetected && diffTouchY < -slideDeltaThreshold / 100);
    public bool DoubleTap() => doubleTapDetected;
}

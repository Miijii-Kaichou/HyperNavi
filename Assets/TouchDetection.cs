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
    private float lastTimeSinceTouch;
    private float timeDiff;

    private bool validTouch = false;
    private bool doubleTapDetected;
    private bool horizontalSlideDetected;
    private bool verticalSlideDetected;
    private bool tapDetected;

    private bool touchIsDown = false;

    public const float RESET = 0;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (touchInputType == InputType.DoubleTap)
            time += Time.deltaTime;
    }

    bool OnTouchArea()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                Vector3 viewportPoint = Camera.main.ScreenToViewportPoint(touch.position);
                touchX = viewportPoint.x;
                touchY = viewportPoint.y;
                Vector2 touchPoint = new Vector2(touchX, touchY);

                if (touchArea == Physics2D.OverlapPoint(Camera.main.ViewportToWorldPoint(touchPoint)))
                    return true;
            }
        }

        return false;
    }

    IEnumerator TouchPositionDeltaDetection()
    {
        //Set previous touch to current
        UpdatePreviousTouch();
        while (true)
        {
            UpdatePreviousTouch();

            if (!touchIsDown)
                touchIsDown = true;

            if (OnTouchArea())
                CalculateTouchDiff();

            if (!OnTouchArea())
            {
                touchIsDown = false;
                ResetTouchPosition();
            }

            yield return null;
        }
    }

    IEnumerator DoubleTapDetection()
    {
        while (true)
        {
            if (OnTouchArea() && !touchIsDown)
            {
                CalculateTimeDiff();
                lastTimeSinceTouch = time;
                touchIsDown = true;
            }

            if (!OnTouchArea())
                touchIsDown = false;
            yield return null;
        }
    }

    void ResetTouchPosition()
    {
        touchX = RESET;
        touchY = touchX;
        previousTouchX = touchX;
        previousTouchY = touchY;
        diffTouchX = touchX;
        diffTouchY = touchX;
    }

    IEnumerator TapDetection()
    {
        while (true)
        {
            if (OnTouchArea() && !touchIsDown)
            {
                touchIsDown = true;
                tapDetected = true;
            }

            if (!OnTouchArea())
                touchIsDown = false;

            yield return null;
        }
    }

    IEnumerator TapDeadTime()
    {
        float time = 0;
        float deadTime = 0.1f;
        while (true)
        {

            if (tapDetected)
                time += Time.deltaTime;

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

        horizontalSlideDetected = (Mathf.Abs(diffTouchX) >= slideDeltaThreshold / 25);
        verticalSlideDetected = (Mathf.Abs(diffTouchY) >= slideDeltaThreshold / 25);
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
        StartCoroutine(UpdateDebugUI());
        switch (touchInputType)
        {
            case InputType.Tap:
                StartCoroutine(TapDetection());
                StartCoroutine(TapDeadTime());
                break;
            case InputType.DoubleTap:
                StartCoroutine(DoubleTapDetection());
                break;
            case InputType.Slide:
                StartCoroutine(TouchPositionDeltaDetection());
                break;
            default:
                break;
        }

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
    public bool SlideLeft() => (horizontalSlideDetected &&  !verticalSlideDetected && diffTouchX < -slideDeltaThreshold / 25);
    public bool SlideRight() => (horizontalSlideDetected && !verticalSlideDetected && diffTouchX > slideDeltaThreshold / 25);
    public bool SlideUp() => (verticalSlideDetected && !horizontalSlideDetected && diffTouchY > slideDeltaThreshold / 25);
    public bool SlideDown() => (verticalSlideDetected && !horizontalSlideDetected && diffTouchY < -slideDeltaThreshold / 25);
    public bool DoubleTap() => doubleTapDetected;
}

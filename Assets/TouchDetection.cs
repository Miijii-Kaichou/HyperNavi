using System.Collections;
using UnityEditor.ShaderGraph.Internal;
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

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(touchInputType == InputType.DoubleTap)
            time += Time.deltaTime;
    }

    bool OnTouchArea()
    {
        if (Input.touchCount > 0)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchX = worldPoint.x;
            touchY = worldPoint.y;
            Vector2 touchPoint = new Vector2(touchX, touchY);

            if (touchArea == Physics2D.OverlapPoint(touchPoint))
            {
                validTouch = true;
            }
            else
            {
                validTouch = false;
            }

            return validTouch;
        }

        return false;
    }

    IEnumerator TouchPositionDeltaDetection()
    {
        //Set previous touch to current
        UpdatePreviousTouch();
        while (true)
        {
            if (OnTouchArea() && !touchIsDown)
                CalculateTouchDiff();

            if (!OnTouchArea())
                touchIsDown = false;

            yield return new WaitForSecondsRealtime(1 / 4);
            UpdatePreviousTouch();
        }
    }

    IEnumerator DoubleTapDetection()
    {
        while (true)
        {
            if (OnTouchArea() && Input.touchCount > 0 && !touchIsDown || Input.GetMouseButtonDown(0)) {
                CalculateTimeDiff();
                lastTimeSinceTouch = time;
                touchIsDown = true;
            }

            if (!OnTouchArea())
                touchIsDown = false;

            yield return null;
        }
    }

    IEnumerator TapDetection()
    {
        while (true)
        {
            if (OnTouchArea() && !touchIsDown && Input.touchCount > 0)
            {
                touchIsDown = true;
                AudioManager.Play("Awesome");
            }
            
            if(!OnTouchArea())
                touchIsDown = false;

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

        horizontalSlideDetected = (Mathf.Abs(diffTouchX) > 0.25f);
        verticalSlideDetected = (Mathf.Abs(diffTouchY) > 0.25f);
    }

    //Calculate difference in time since a touch was detected
    void CalculateTimeDiff()
    {
        timeDiff = time - lastTimeSinceTouch;
        Debug.Log(timeDiff);
        if (!touchIsDown)
        {
            doubleTapDetected = (timeDiff > 0.5f);

            if (doubleTapDetected)
            {
                Debug.Log("Tap Detected");
                touchIsDown = true;
            } else
            {
                touchIsDown = false;
            }
        }
    }

    /// <summary>
    /// Initialize Coroutines based on enumerator
    /// </summary>
    void Init()
    {
        switch (touchInputType)
        {
            case InputType.Tap:
                StartCoroutine(TapDetection());
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

    public bool IsTapping() => tapDetected;
    public bool SlideLeft() => (diffTouchX <= -slideDeltaThreshold);
    public bool SlideRight() => (diffTouchX >= slideDeltaThreshold);
    public bool SlideUp() => (diffTouchY >= slideDeltaThreshold);
    public bool SlideDown() => (diffTouchY <= -slideDeltaThreshold);
    public bool IsDoubleTapping => doubleTapDetected;
}

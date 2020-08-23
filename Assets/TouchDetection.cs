using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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

    private bool beginDetectingTap = false;
    private bool beginDetectingDoubleTap = false;
    private bool beginDetectingSlide = false;

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
        if(Input.touchCount == 1)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchX = worldPoint.x;
            touchY = worldPoint.y;
            Vector2 touchPoint = new Vector2(touchX, touchY);

            if(touchArea == Physics2D.OverlapPoint(touchPoint)){
                //Execute event based on enumerator
                switch (touchInputType)
                {
                    case InputType.Tap:
                        DetectTap();
                        break;
                    case InputType.DoubleTap:
                        DetectDoubleTap();
                        break;
                    case InputType.Slide:
                        DetectSlide();
                        break;
                    default:
                        break;
                }
            } else
            {
                beginDetectingTap = false;
                beginDetectingDoubleTap = false;
                beginDetectingSlide = false;
                touchIsDown = false;
            }
        }
    }

    void DetectTap()
    {
        onTap.Invoke();
        AudioManager.Play("Awesome", 100);
        beginDetectingTap = true;
    }

    void DetectDoubleTap()
    {
        onDoubleTap.Invoke();
        beginDetectingDoubleTap = true;
    }

    void DetectSlide()
    {
        onSlide.Invoke();
        beginDetectingSlide = true;
    }

    IEnumerator TouchPositionDeltaDetection()
    {
        //Set previous touch to current
        UpdatePreviousTouch();
        while (true)
        {
            if(beginDetectingSlide)
                CalculateTouchDiff();
            yield return new WaitForSecondsRealtime(1 / 4);
            UpdatePreviousTouch();
        }
    }

    IEnumerator DoubleTapDetection()
    {
        while (true)
        {
            if(
                (beginDetectingTap && Input.touchCount > 0) || 
                (beginDetectingDoubleTap && Input.touchCount > 0)
              )
                lastTimeSinceTouch = time;

            CalculateTimeDiff();

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
        if (!touchIsDown)
        {
            doubleTapDetected = (timeDiff > 0.5f);
            tapDetected = (timeDiff < 0.5f);
            touchIsDown = true;
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
                StartCoroutine(DoubleTapDetection());
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
    public bool SlideLeft() => (Mathf.Sign(diffTouchX) == -1);
    public bool SlideRight() => (Mathf.Sign(diffTouchX) == 1);
    public bool SlideUp() => (Mathf.Sign(diffTouchY) == -1);
    public bool SlideDown() => (Mathf.Sign(diffTouchY) == 1);
    public bool IsDoubleTapping => doubleTapDetected;
}

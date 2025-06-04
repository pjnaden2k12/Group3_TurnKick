using UnityEngine;
using UnityEngine.EventSystems;

public class PivotX : Pivot
{
    public Sprite xSprite;

    private bool isXState = false;

    protected override void Start()
    {
        base.Start();

        if (controller != null)
        {
            controller.OnRotationStarted += OnRotationStartedHandler;
        }
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnRotationStarted -= OnRotationStartedHandler;
        }
    }

    private void OnRotationStartedHandler()
    {
        if (isXState) return;

        GameObject[] dots = GameObject.FindGameObjectsWithTag("Dot");
        bool dotOnPivot = false;

        foreach (GameObject dot in dots)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, dot.transform.position))
            {
                dotOnPivot = true;
                break;
            }
        }

        if (!dotOnPivot)
        {
            isXState = true;
            pivotImage.sprite = xSprite;
            gameObject.tag = "Untagged";
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || controller.isRotating) return;

        if (isXState)
        {
            bool dotOnPivot = false;
            GameObject[] dots = GameObject.FindGameObjectsWithTag("Dot");

            foreach (GameObject dot in dots)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, dot.transform.position))
                {
                    dotOnPivot = true;
                    break;
                }
            }

            if (!dotOnPivot)
            {
                isXState = false;
                pivotImage.sprite = normalSprite;
                gameObject.tag = "Pivot";
            }
        }
        else
        {
            base.OnPointerClick(eventData);
        }
    }

    protected override void Update()
    {
        if (isXState) return;
        base.Update();
    }
}

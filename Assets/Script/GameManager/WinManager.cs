using UnityEngine;

public class DotTriggerDetector : MonoBehaviour
{
    public bool isTouchingStop = false;
    private Transform currentPivot;

    public void SetCurrentPivot(Transform pivot)
    {
        currentPivot = pivot;
        isTouchingStop = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentPivot == null) return;

        if (other.CompareTag("Stop") && other.transform.IsChildOf(currentPivot))
        {
            isTouchingStop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentPivot == null) return;

        if (other.CompareTag("Stop") && other.transform.IsChildOf(currentPivot))
        {
            isTouchingStop = false;
        }
    }

}

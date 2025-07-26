using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MapCameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mapCamera;

    public LogPathRenderer logPathRenderer;

    private int currentIndex = 0;

    public void MoveCameraToPosition(Vector3 targetPosition, float duration)
    {
        mapCamera.transform.DOMove(targetPosition, duration);
    }

    public void NextIndex()
    {
        if (currentIndex < logPathRenderer.lineRenderer.positionCount - 1)
        {
            currentIndex++;
            Vector3 nextPosition = logPathRenderer.lineRenderer.GetPosition(currentIndex);
            nextPosition.y = 10;

            float duration = 1.0f;

            float distance = Vector3.Distance(mapCamera.transform.position, nextPosition);
            print($"<color=blue>{distance}</color>");
            MoveCameraToPosition(nextPosition, 1.0f);
        }
    }

    public void PreviousIndex()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            Vector3 previousPosition = logPathRenderer.lineRenderer.GetPosition(currentIndex);
            previousPosition.y = 10;

            float duration = 1.0f;

            float distance = Vector3.Distance(mapCamera.transform.position, previousPosition);
            print(distance);
            MoveCameraToPosition(previousPosition, 1.0f);
        }
    }
}
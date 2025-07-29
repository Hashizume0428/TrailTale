using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class MapCameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mapCamera;

    public LogPathRenderer logPathRenderer;

    private int currentIndex = 0;

    public async UniTask MoveCameraToPosition(Vector3 targetPosition, float duration)
    {
        await mapCamera.transform.DOMove(targetPosition, duration);
    }

    public async UniTask NextIndex()
    {
        if (currentIndex < logPathRenderer.lineRenderer.positionCount - 1)
        {
            currentIndex++;
            Vector3 nextPosition = logPathRenderer.lineRenderer.GetPosition(currentIndex);
            nextPosition.y = 10;

            float duration = 1.0f;

            float distance = Vector3.Distance(mapCamera.transform.position, nextPosition);
            print($"<color=blue>{distance}</color>");
            await MoveCameraToPosition(nextPosition, 1.0f);
        }
    }

    public async UniTask PreviousIndex()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            Vector3 previousPosition = logPathRenderer.lineRenderer.GetPosition(currentIndex);
            previousPosition.y = 10;

            float duration = 1.0f;

            float distance = Vector3.Distance(mapCamera.transform.position, previousPosition);
            print(distance);
            await MoveCameraToPosition(previousPosition, 1.0f);
        }
    }
}
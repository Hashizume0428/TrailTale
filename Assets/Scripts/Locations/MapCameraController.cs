using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class MapCameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mapCamera;

    public LogPathRenderer logPathRenderer;

    public int currentIndex = 0;

    public async UniTask MoveCameraToPosition(Vector3 targetPosition, float duration)
    {
        targetPosition.y = 10;
        await mapCamera.transform.DOMove(targetPosition, duration);
    }

    public async UniTask NextIndex()
    {
        bool isEventPoint = false;

        while (!isEventPoint)
        {
            if (currentIndex < logPathRenderer.lineRenderer.positionCount - 1)
            {
                currentIndex++;
                Vector3 nextPosition = logPathRenderer.lineRenderer.GetPosition(currentIndex);

                float duration = 1.0f;

                float distance = Vector3.Distance(mapCamera.transform.position, nextPosition);
                print($"<color=blue>{distance}</color>");
                await MoveCameraToPosition(nextPosition, 1.0f);

                if (logPathRenderer.eventPointList.Contains(currentIndex))
                {
                    isEventPoint = true;
                }
            }
            else
            {
                break;
            }
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLineRenderer : MonoBehaviour
{
    [SerializeField]
    private Line line;

    private List<Line> lines = new List<Line>();

    public void WriteLine(List<Vector2> points, Transform parent)
    {
        if (points.Count < 2)
        {
            Debug.LogWarning("Not enough points to draw a line.");
            return;
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            Line newLine = Instantiate(line, parent);
            newLine.SetPosition(points[i], points[i + 1]);
            lines.Add(newLine);
        }
    }

    public void ClearLines()
    {
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
    }
}
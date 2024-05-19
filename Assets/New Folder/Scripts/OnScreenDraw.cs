using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnScreenDraw : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public RectTransform areaRect;
    public Camera cam;

    private bool drawing;

    private float minDistance = 0.1f;

    Vector3 prevPosition;

    public Vector3[] positions;

    public static event Action<Vector3[]> OnNewShapeDraw;
    public float tolerance;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseInsideArea())
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0) && drawing)
        {
            UpdateLine();
        }
        else if (Input.GetMouseButtonUp(0) && drawing)
        {
            StopDrawing();
        }
    }

    void StartDrawing()
    {
        Vector3 position = GetMouseWorldPosition();
        drawing = true;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, position);
        prevPosition = position;
    }

    void UpdateLine()
    {
        Vector3 position = GetMouseWorldPosition();
        if (Vector3.Distance(position, prevPosition) > minDistance)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
            prevPosition = position;
        }
    }

    void StopDrawing()
    {
        drawing = false;

        List<Vector3> pos = SimplifyCurve();
        positions = new Vector3[pos.Count];

        positions = pos.ToArray();

        lineRenderer.positionCount = 0;
        OnNewShapeDraw?.Invoke(positions);
    }

    bool IsMouseInsideArea()
    {
        Vector2 mousePosition = Input.mousePosition;
        return RectTransformUtility.RectangleContainsScreenPoint(areaRect, mousePosition);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mousePosition);
    }
    List<Vector3> SimplifyCurve()
    {
        // need to refactor
        int desiredCount = 10;
        int pointsBefore = lineRenderer.positionCount;
        var points = new Vector3[pointsBefore];
        lineRenderer.GetPositions(points);

        List<Vector3> simplifiedPoints = new List<Vector3>(points);

        int r = 0;
        while (pointsBefore > desiredCount)
        {
            LineUtility.Simplify(points.ToList(), 0.01f, simplifiedPoints);
            pointsBefore = simplifiedPoints.Count;
            r++;
            if (r > 1) break;
        }


        lineRenderer.positionCount = simplifiedPoints.Count;
        lineRenderer.SetPositions(simplifiedPoints.ToArray());
        return simplifiedPoints;
    }
}

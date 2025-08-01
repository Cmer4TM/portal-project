using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISoftShadow : BaseMeshEffect
{
    [SerializeField] private Color shadowColor = new(0, 0, 0, 0.5f);
    [SerializeField] private Vector2 shadowDistance;
    [SerializeField] private int iterations = 5;
    [SerializeField] private Vector2 shadowSpread;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (IsActive() == false) return;

        List<UIVertex> output = new();
        vh.GetUIVertexStream(output);

        List<UIVertex> vertsCopy = new(output);
        output.Clear();

        for (float x = 0; x < iterations; x++)
        {
            for (int y = 0; y < vertsCopy.Count; y++)
            {
                UIVertex vertex = vertsCopy[y];
                Vector3 position = vertex.position;
                float fac = x / iterations;

                position.x *= 1 + shadowSpread.x * fac * 0.01f;
                position.y *= 1 + shadowSpread.y * fac * 0.01f;

                position.x += shadowDistance.x * fac;
                position.y += shadowDistance.y * fac;

                vertex.position = position;

                Color32 color = shadowColor;

                color.a /= (byte)iterations;
                vertex.color = color;

                output.Add(vertex);
            }
        }

        foreach (UIVertex vertex in vertsCopy)
        {
            output.Add(vertex);
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(output);
    }
}

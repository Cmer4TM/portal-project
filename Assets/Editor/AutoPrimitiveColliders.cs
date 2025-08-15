// Assets/Editor/AutoPrimitiveColliders.cs
using UnityEditor;
using UnityEngine;

public static class AutoPrimitiveColliders
{
    // Настройки "похожести на трубу"
    const float ELONGATED_RATIO = 1.8f; // насколько длиннее длинная ось, чем средняя
    const float SEGMENT_ASPECT = 2.2f; // ориентир: длина сегмента ≈ радиус*SEGMENT_ASPECT
    const int MIN_SEGMENTS = 2;
    const int MAX_SEGMENTS = 8;

    [MenuItem("Tools/Auto Colliders/Boxes + Capsules %#g", true)]
    static bool ValidateGen() => Selection.activeGameObject != null;

    [MenuItem("Tools/Auto Colliders/Boxes + Capsules %#g")]
    public static void GenerateCapsulePreferred()
    {
        GenerateInternal(preferCapsules: true, segmentedBoxes: false);
    }

    [MenuItem("Tools/Auto Colliders/Boxes (Segmented)")]
    public static void GenerateSegmentedBoxes()
    {
        GenerateInternal(preferCapsules: false, segmentedBoxes: true);
    }

    static void GenerateInternal(bool preferCapsules, bool segmentedBoxes)
    {
        var root = Selection.activeGameObject;
        int made = 0;

        foreach (var mf in root.GetComponentsInChildren<MeshFilter>(true))
        {
            var mesh = mf.sharedMesh; if (!mesh) continue;

            // создаём локальный контейнер под конкретный меш (важно для ориентации)
            var bucket = new GameObject("Colliders");
            Undo.RegisterCreatedObjectUndo(bucket, "Create Colliders");
            bucket.transform.SetParent(mf.transform, false);

            // размеры в ЛОКАЛЕ меша (без поворота/масштаба мира)
            var b = mesh.bounds;
            Vector3 size = b.size;

            // какая ось самая длинная
            int axis = 0; // 0-X,1-Y,2-Z
            if (size.y > size.x && size.y >= size.z) axis = 1;
            else if (size.z > size.x && size.z >= size.y) axis = 2;

            float max = Mathf.Max(size.x, size.y, size.z);
            float min = Mathf.Min(size.x, size.y, size.z);
            float mid = size.x + size.y + size.z - max - min;
            bool elongated = max > ELONGATED_RATIO * mid; // трубо-подобная форма

            // сколько сегментов сделать
            int n = Mathf.Clamp(Mathf.RoundToInt(max / Mathf.Max(min * SEGMENT_ASPECT, 0.0001f)), MIN_SEGMENTS, MAX_SEGMENTS);
            float step = max / n;
            Vector3 axisVec = (axis == 0) ? Vector3.right : (axis == 1) ? Vector3.up : Vector3.forward;

            if (elongated && preferCapsules)
            {
                // ЦЕПОЧКА КАПСУЛ вдоль длинной оси
                for (int i = 0; i < n; i++)
                {
                    var go = new GameObject($"Capsule_{mf.name}_{i}");
                    Undo.RegisterCreatedObjectUndo(go, "Create Capsule");
                    go.transform.SetParent(bucket.transform, false);
                    go.transform.localPosition = b.center + axisVec * ((i + 0.5f - n * 0.5f) * step);

                    var cc = go.AddComponent<CapsuleCollider>();
                    cc.direction = axis; // ось вдоль которой растягиваем
                    cc.radius = min * 0.5f;
                    cc.height = Mathf.Max(step, min * 1.02f);
                }
                made += n;
            }
            else
            {
                if (segmentedBoxes && elongated)
                {
                    // РАЗБИТИЕ НА БОКСЫ вдоль длинной оси
                    for (int i = 0; i < n; i++)
                    {
                        var go = new GameObject($"Box_{mf.name}_{i}");
                        Undo.RegisterCreatedObjectUndo(go, "Create Box");
                        go.transform.SetParent(bucket.transform, false);
                        go.transform.localPosition = b.center + axisVec * ((i + 0.5f - n * 0.5f) * step);

                        var bc = go.AddComponent<BoxCollider>();
                        Vector3 s = size;
                        if (axis == 0) s.x = step;
                        else if (axis == 1) s.y = step;
                        else s.z = step;
                        bc.size = s;
                    }
                    made += n;
                }
                else
                {
                    // Обычный одиночный box (не труба/короткий элемент)
                    var go = new GameObject($"Box_{mf.name}");
                    Undo.RegisterCreatedObjectUndo(go, "Create Box");
                    go.transform.SetParent(bucket.transform, false);
                    go.transform.localPosition = b.center;
                    var bc = go.AddComponent<BoxCollider>();
                    bc.size = size;
                    made++;
                }
            }
        }

        Debug.Log($"[Auto Colliders] Created: {made} colliders. (Подсказка: для труб используй Boxes + Capsules)");
    }

    [MenuItem("Tools/Auto Colliders/Delete All In Selection")]
    public static void DeleteAll()
    {
        var root = Selection.activeGameObject; if (!root) return;
        foreach (var c in root.GetComponentsInChildren<Collider>(true))
            Undo.DestroyObjectImmediate(c);
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == "Colliders") Undo.DestroyObjectImmediate(t.gameObject);
        Debug.Log("[Auto Colliders] Deleted all in selection.");
    }
}

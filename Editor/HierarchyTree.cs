using UnityEditor;
using UnityEngine;

namespace DyrdaDev.ForUnity.Hierarchy
{
    [InitializeOnLoad]
    internal class HierarchyTree
    {
        private static float edgeWidth = 1.0f;
        private static Color edgeColor = new Color(0.46f, 0.46f, 0.46f);
        private static Color highlightedEdgeColor = new Color(0.239f, 0.5f, 0.874f);

        private enum EdgeType
        {
            middleChild,
            lastChild,
            sibling
        }

        static SceneGraphTree()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyTree;
        }

        private static Color GetColor(bool highlighted)
        {
            if (highlighted)
            {
                return highlightedEdgeColor;
            }
            else
            {
                return edgeColor;
            }
        }

        private static float CalculateRectXValue(Rect rect, int graphDistance)
        {
            return rect.x - 21.5f - graphDistance * (rect.height - 2);
        }

        private static void DrawFullVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y, edgeWidth, rect.height),
                GetColor(highlighted));
        }

        private static void DrawHalfVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y, edgeWidth, rect.height / 2),
                GetColor(highlighted));
        }

        private static void DrawHorizontalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(
                new Rect(CalculateRectXValue(rect, graphDistance), rect.y + rect.height / 2, rect.height / 2,
                    edgeWidth),
                GetColor(highlighted));
        }

        private static void DrawHierarchyEdge(EdgeType edgeType, bool highlighted, int graphDistance, Rect rect)
        {
            switch (edgeType)
            {
                case EdgeType.sibling:
                    DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                    break;
                case EdgeType.lastChild:
                    DrawHalfVerticalEdgeSegment(rect, graphDistance, highlighted);
                    DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                    break;
                case EdgeType.middleChild:
                    DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                    DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                    break;
            }
        }

        private static void DrawHierarchyTree(int instanceID, Rect selectionRect)
        {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go != null)
            {
                // Draw children (
                if (go.transform.parent != null)
                {
                    if (go.transform.GetSiblingIndex() < go.transform.parent.childCount - 1)
                    {
                        if (Selection.activeTransform != null &&
                            go.transform.parent.IsChildOf(Selection.activeTransform))
                        {
                            DrawHierarchyEdge(EdgeType.middleChild, true, 0, selectionRect);
                        }
                        else
                        {
                            DrawHierarchyEdge(EdgeType.middleChild, false, 0, selectionRect);
                        }
                    }
                    else
                    {
                        if (Selection.activeTransform != null &&
                            go.transform.parent.IsChildOf(Selection.activeTransform))
                        {
                            DrawHierarchyEdge(EdgeType.lastChild, true, 0, selectionRect);
                        }
                        else
                        {
                            DrawHierarchyEdge(EdgeType.lastChild, false, 0, selectionRect);
                        }
                    }

                    var referenceTransform = go.transform.parent;
                    var currentDistance = 1;

                    // Draw ancestors with open sibling relations
                    while (referenceTransform.parent != null)
                    {
                        if (referenceTransform.GetSiblingIndex() < referenceTransform.parent.childCount - 1)
                        {
                            if (Selection.activeTransform != null &&
                                referenceTransform.parent.IsChildOf(Selection.activeTransform))
                            {
                                DrawHierarchyEdge(EdgeType.sibling, true, currentDistance, selectionRect);
                            }
                            else
                            {
                                DrawHierarchyEdge(EdgeType.sibling, false, currentDistance, selectionRect);
                            }
                        }

                        referenceTransform = referenceTransform.parent;
                        currentDistance++;
                    }
                }
            }
        }
    }
}
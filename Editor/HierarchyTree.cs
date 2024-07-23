using UnityEditor;
using UnityEngine;

namespace DyrdaDev.ForUnity.Hierarchy
{
    [InitializeOnLoad]
    internal class HierarchyTree
    {
        private enum EdgeType
        {
            MiddleChild,
            LastChild,
            Sibling,
            Root
        }

        static HierarchyTree()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyTree;
        }

        private static Color GetColor(bool highlighted)
        {
            if (highlighted)
            {
                return HierarchyTreeSettings.instance.HighlightedEdgeColor;
            }
            else
            {
                return HierarchyTreeSettings.instance.EdgeColor;
            }
        }

        private static float CalculateRectXValue(Rect rect, int graphDistance)
        {
            return rect.x - 21.5f - graphDistance * (rect.height - 2);
        }

        private static void DrawFullVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y,
                HierarchyTreeSettings.instance.EdgeWidth, rect.height), GetColor(highlighted));
        }

        private static void DrawHalfVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y,
                HierarchyTreeSettings.instance.EdgeWidth, rect.height / 2), GetColor(highlighted));
        }

        private static void DrawHorizontalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(
                new Rect(CalculateRectXValue(rect, graphDistance), rect.y + rect.height / 2, rect.height / 2,
                    HierarchyTreeSettings.instance.EdgeWidth),
                GetColor(highlighted));
        }

        private static void DrawExtendingHorizontalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(
                new Rect(CalculateRectXValue(rect, graphDistance) - (rect.height - 2) / 2, rect.y + rect.height / 2,
                    rect.height / 2, HierarchyTreeSettings.instance.EdgeWidth),
                GetColor(highlighted));
        }

        private static void DrawHierarchyEdge(EdgeType edgeType, bool highlighted, bool extendedLine, int graphDistance, Rect rect)
        {
            switch (edgeType)
            {
                case EdgeType.Sibling:
                    DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                    break;
                case EdgeType.LastChild:
                    DrawHalfVerticalEdgeSegment(rect, graphDistance, highlighted);
                    DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                    if(extendedLine){
                        DrawExtendingHorizontalEdgeSegment(rect, graphDistance-1, highlighted);
                    }
                    break;
                case EdgeType.MiddleChild:
                    DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                    DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                    if(extendedLine){
                        DrawExtendingHorizontalEdgeSegment(rect, graphDistance-1, highlighted);
                    }
                    break;
                case EdgeType.Root:
                    DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                    if(extendedLine){
                        DrawExtendingHorizontalEdgeSegment(rect, graphDistance-1, highlighted);
                    }
                    break;
            }
        }

        private static void DrawHierarchyTree(int instanceID, Rect selectionRect)
        {
            if (!HierarchyTreeSettings.instance.Enabled)
            {
                return;
            }
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go != null)
            {
                bool hasChildren = go.transform.childCount > 0;
                
                // Draw children
                if (go.transform.parent != null)
                {
                    bool isHighlighted = Selection.activeTransform != null
                        && go.transform.parent.IsChildOf(Selection.activeTransform);
                    EdgeType edgeType = go.transform.GetSiblingIndex() < go.transform.parent.childCount - 1 
                        ? EdgeType.MiddleChild : EdgeType.LastChild;

                    DrawHierarchyEdge(edgeType, isHighlighted, !hasChildren, 0, selectionRect);


                    var currentDistance = 1;
                    var referenceTransform = go.transform.parent;
                    // Draw ancestors with open sibling relations
                    while (referenceTransform.parent != null)
                    {
                        if (referenceTransform.GetSiblingIndex() < referenceTransform.parent.childCount - 1)
                        {
                            isHighlighted = Selection.activeTransform != null
                                && referenceTransform.parent.IsChildOf(Selection.activeTransform);

                            DrawHierarchyEdge(EdgeType.Sibling, isHighlighted, false, currentDistance, selectionRect);
                        }

                        referenceTransform = referenceTransform.parent;
                        currentDistance++;
                    }
                    
                }
                // Draw root nodes
                else if (HierarchyTreeSettings.instance.RenderRootEdge)
                {
                    bool isHighlighted = Selection.activeTransform != null
                        && (Selection.activeTransform.IsChildOf(go.transform) || go.transform == Selection.activeTransform);
                    DrawHierarchyEdge(EdgeType.Root, isHighlighted, !hasChildren, 0, selectionRect);
                }
            }
        }
    }
}

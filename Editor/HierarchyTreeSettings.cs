using System;
using UnityEditor;
using UnityEngine;

namespace DyrdaDev.ForUnity.Hierarchy
{
    [FilePath("Hierarchy/HierarchyTree/Settings.asset", FilePathAttribute.Location.PreferencesFolder)]
    internal class HierarchyTreeSettings : ScriptableSingleton<HierarchyTreeSettings>
    {
        [field: SerializeField]
        public bool Enabled {get; private set;} = true;

        [field: SerializeField]
        public float EdgeWidth {get; private set;} = 1.0f;
        [field: SerializeField]
        public Color EdgeColor {get; private set;} =  new Color(0.46f, 0.46f, 0.46f);
        [field: SerializeField]
        public Color HighlightedEdgeColor {get; private set;} = new Color(0.49f, 0.678f, 0.952f);

        [SettingsProvider]
        public static SettingsProvider CreateHierarchyTreeSettingsProvider()
        {
            var keywords = new string[]{"Hierarchy", "Tree", "Edge Width", "Edge Color", "Highlighted Edge Color"};

            return new SettingsProvider("Preferences/Hierarchy/Hierarchy Tree", SettingsScope.User, keywords)
            {
                guiHandler = (searchContext) => {
                    instance.SetEnabled(EditorGUILayout.Toggle("Active", instance.Enabled));
                    instance.SetEdgeWidth(EditorGUILayout.Slider("Edge Width", instance.EdgeWidth, 1f, 6f));
                    instance.SetEdgeColor(EditorGUILayout.ColorField("Edge Color", instance.EdgeColor));
                    instance.SetHighlightEdgeColor(EditorGUILayout.ColorField(
                        "Highlighted Edge Color", instance.HighlightedEdgeColor));
                }
            };
        }
        
        private void SaveWhenDifferent<T>(T oldValue, T newValue)
        {
            if (!oldValue.Equals(newValue))
            {
                Save(true);
            }
        }

        private void SetEnabled(bool enabled)
        {
            var old = Enabled;
            Enabled = enabled;
            SaveWhenDifferent(old, Enabled);
        }

        private void SetEdgeWidth(float edgeWidth)
        {
            var old = EdgeWidth;
            EdgeWidth = edgeWidth;
            SaveWhenDifferent(old, EdgeWidth);
        }

        private void SetEdgeColor(Color edgeColor)
        {
            var old = EdgeColor;
            EdgeColor = edgeColor;
            SaveWhenDifferent(old, EdgeColor);
        }

        private void SetHighlightEdgeColor(Color highlightedEdgeColor)
        {
            var old = HighlightedEdgeColor;
            HighlightedEdgeColor = highlightedEdgeColor;
            SaveWhenDifferent(old, HighlightedEdgeColor);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FloristGame.Core.Arrangements;

namespace FloristGame.Gameplay
{
    // Spawns one toggle per ArrangementStyle into a shared ToggleGroup;
    // selecting one applies it to the workspace's in-progress arrangement.
    // There's no ScriptableObject catalog for styles (no cost/season data
    // needed), so the options are just labels/icons on the enum itself.
    public class StyleSelector : MonoBehaviour
    {
        [Serializable]
        public struct StyleOption
        {
            public ArrangementStyle style;
            public string displayName;
            public Sprite icon;
        }

        public WorkspaceController workspace;

        public List<StyleOption> options = new List<StyleOption>
        {
            new StyleOption { style = ArrangementStyle.Round, displayName = "Round" },
            new StyleOption { style = ArrangementStyle.Cascade, displayName = "Cascade" },
            new StyleOption { style = ArrangementStyle.Minimalist, displayName = "Minimalist" },
            new StyleOption { style = ArrangementStyle.Wild, displayName = "Wild" },
        };

        public SelectableOptionToggle togglePrefab;
        public Transform container;
        public ToggleGroup toggleGroup;

        private void Start()
        {
            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                var instance = Instantiate(togglePrefab, container);
                instance.SetContent(option.displayName, option.icon);
                instance.Toggle.group = toggleGroup;

                instance.Toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn) workspace.SetStyle(option.style);
                });

                instance.Toggle.isOn = i == 0;
            }

            if (options.Count > 0)
            {
                workspace.SetStyle(options[0].style);
            }
        }
    }
}

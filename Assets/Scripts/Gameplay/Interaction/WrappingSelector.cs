using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FloristGame.Data;

namespace FloristGame.Gameplay
{
    // Spawns one toggle per WrappingDefinition into a shared ToggleGroup;
    // selecting one applies it to the workspace's in-progress arrangement.
    public class WrappingSelector : MonoBehaviour
    {
        public WorkspaceController workspace;
        public List<WrappingDefinition> options = new List<WrappingDefinition>();
        public SelectableOptionToggle togglePrefab;
        public Transform container;
        public ToggleGroup toggleGroup;

        private void Start()
        {
            for (int i = 0; i < options.Count; i++)
            {
                var definition = options[i];
                var instance = Instantiate(togglePrefab, container);
                instance.SetContent(definition.displayName, definition.icon);
                instance.Toggle.group = toggleGroup;

                instance.Toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn) workspace.SetWrapping(definition.type);
                });

                instance.Toggle.isOn = i == 0;
            }

            if (options.Count > 0)
            {
                workspace.SetWrapping(options[0].type);
            }
        }
    }
}

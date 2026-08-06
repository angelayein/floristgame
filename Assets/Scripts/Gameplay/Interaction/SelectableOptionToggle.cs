using UnityEngine;
using UnityEngine.UI;

namespace FloristGame.Gameplay
{
    // Visual content for one entry in a wrapping/vase/style toggle group.
    // The selector scripts instantiate this prefab once per option and
    // fill in its label/icon, then wire its Toggle into a shared ToggleGroup.
    [RequireComponent(typeof(Toggle))]
    public class SelectableOptionToggle : MonoBehaviour
    {
        public Text label;
        public Image icon;

        public Toggle Toggle { get; private set; }

        private void Awake()
        {
            Toggle = GetComponent<Toggle>();
        }

        public void SetContent(string text, Sprite sprite)
        {
            if (label != null) label.text = text;
            if (icon != null && sprite != null) icon.sprite = sprite;
        }
    }
}

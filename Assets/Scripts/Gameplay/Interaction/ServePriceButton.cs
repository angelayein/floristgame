using UnityEngine;
using UnityEngine.UI;

namespace FloristGame.Gameplay
{
    // Button.onClick's Inspector-wired persistent listeners can only call a
    // parameterless method (or one with a fixed value typed into the
    // Inspector) - they can't read a live UI value at click time. This is
    // the seam that reads the price InputField and forwards it to
    // WorkspaceController.ServeCustomer(float). Wire the Serve button's
    // OnClick() to this component's Serve() method.
    public class ServePriceButton : MonoBehaviour
    {
        public WorkspaceController workspace;
        public InputField priceInput;

        public void Serve()
        {
            if (!float.TryParse(priceInput.text, out float price) || price < 0f)
            {
                Debug.LogWarning("Enter a valid price before serving.");
                return;
            }

            workspace.ServeCustomer(price);
        }
    }
}

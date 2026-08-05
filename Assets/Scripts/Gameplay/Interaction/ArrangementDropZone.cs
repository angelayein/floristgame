using UnityEngine;
using UnityEngine.EventSystems;

namespace FloristGame.Gameplay
{
    // The workspace surface: dropping a DraggableFlowerChip here adds one
    // stem of that species to the WorkspaceController's ArrangementBuilder.
    public class ArrangementDropZone : MonoBehaviour, IDropHandler
    {
        public WorkspaceController workspace;

        public void OnDrop(PointerEventData eventData)
        {
            var dragged = eventData.pointerDrag;
            if (dragged == null) return;

            var chip = dragged.GetComponent<DraggableFlowerChip>();
            if (chip == null || chip.flower == null) return;

            workspace.AddStemToArrangement(chip.flower.id);
        }
    }
}

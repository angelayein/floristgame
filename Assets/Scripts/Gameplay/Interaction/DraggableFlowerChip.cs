using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FloristGame.Data;

namespace FloristGame.Gameplay
{
    // A single flower-stock chip in the tray. Drag it onto an
    // ArrangementDropZone to add one stem of this species to the
    // in-progress bouquet.
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class DraggableFlowerChip : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public FlowerDefinition flower;
        public Image icon;

        [Header("Optional stock display")]
        public Text stockCountLabel;
        public Image freshnessFill; // Image Type = Filled; fillAmount driven by average freshness (0..1)

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Canvas _rootCanvas;
        private Vector2 _originalAnchoredPosition;
        private Transform _originalParent;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _rootCanvas = GetComponentInParent<Canvas>();
            RefreshIcon();
        }

        // Used by StockTraySpawner when instantiating chips from the
        // catalog at runtime rather than hand-placing one per species.
        public void SetFlower(FlowerDefinition newFlower)
        {
            flower = newFlower;
            RefreshIcon();
        }

        public void RefreshStock(int availableStems, float freshness)
        {
            if (stockCountLabel != null)
            {
                stockCountLabel.text = availableStems.ToString();
            }
            if (freshnessFill != null)
            {
                freshnessFill.fillAmount = freshness;
            }
        }

        private void RefreshIcon()
        {
            if (icon != null && flower != null && flower.icon != null)
            {
                icon.sprite = flower.icon;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
            _originalParent = transform.parent;
            _canvasGroup.blocksRaycasts = false;

            if (_rootCanvas != null)
            {
                transform.SetParent(_rootCanvas.transform, true);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_rootCanvas == null) return;
            _rectTransform.anchoredPosition += eventData.delta / _rootCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            transform.SetParent(_originalParent, true);
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }
    }
}

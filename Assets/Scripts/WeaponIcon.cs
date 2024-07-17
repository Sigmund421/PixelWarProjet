using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int weaponIndex;
    public int slot;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is missing on " + gameObject.name);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f; // Rendre l'icône semi-transparente
            canvasGroup.blocksRaycasts = false; // Permettre à l'icône d'être déplacée
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta; // Déplacer l'icône avec le curseur
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // Restaurer l'opacité complète
            canvasGroup.blocksRaycasts = true; // Restaurer le blocage des rayons
        }
    }
}

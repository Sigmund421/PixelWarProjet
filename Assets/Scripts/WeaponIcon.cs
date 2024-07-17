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
            canvasGroup.alpha = 0.6f; // Rendre l'ic�ne semi-transparente
            canvasGroup.blocksRaycasts = false; // Permettre � l'ic�ne d'�tre d�plac�e
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta; // D�placer l'ic�ne avec le curseur
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // Restaurer l'opacit� compl�te
            canvasGroup.blocksRaycasts = true; // Restaurer le blocage des rayons
        }
    }
}

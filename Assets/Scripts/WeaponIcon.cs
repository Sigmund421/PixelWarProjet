using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponIcon : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public int weaponIndex;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private ShopManager shopManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        shopManager = FindObjectOfType<ShopManager>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is missing on " + gameObject.name);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (shopManager != null && shopManager.IsShopOpen() && shopManager.HasPurchasedWeapon(weaponIndex))
        {
            originalPosition = rectTransform.anchoredPosition;
            canvasGroup.alpha = 0.6f; // Rendre l'ic�ne semi-transparente
            canvasGroup.blocksRaycasts = false; // Permettre � l'ic�ne d'�tre d�plac�e
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (shopManager != null && shopManager.IsShopOpen() && shopManager.HasPurchasedWeapon(weaponIndex))
        {
            rectTransform.anchoredPosition += eventData.delta; // D�placer l'ic�ne avec le curseur
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // Restaurer l'opacit� compl�te
            canvasGroup.blocksRaycasts = true; // Restaurer le blocage des rayons
        }

        if (shopManager != null)
        {
            if (!shopManager.OnDropWeapon(this))
            {
                // Revert to original position if the drop was not successful
                rectTransform.anchoredPosition = originalPosition;
            }
        }
    }
}

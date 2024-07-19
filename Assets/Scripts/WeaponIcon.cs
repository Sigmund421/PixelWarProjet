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
            canvasGroup.alpha = 0.6f; // Rendre l'icône semi-transparente
            canvasGroup.blocksRaycasts = false; // Permettre à l'icône d'être déplacée
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (shopManager != null && shopManager.IsShopOpen() && shopManager.HasPurchasedWeapon(weaponIndex))
        {
            rectTransform.anchoredPosition += eventData.delta; // Déplacer l'icône avec le curseur
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (shopManager == null || !shopManager.HasPurchasedWeapon(weaponIndex))
        {
            return; // Ne rien faire si l'arme n'a pas été achetée
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // Restaurer l'opacité complète
            canvasGroup.blocksRaycasts = true; // Restaurer le blocage des rayons
        }

        if (shopManager != null)
        {
            if (!shopManager.OnDropWeapon(this))
            {
                // Revenir à la position initiale si le drop n'a pas été réussi
                rectTransform.anchoredPosition = originalPosition;
            }
        }
    }
}

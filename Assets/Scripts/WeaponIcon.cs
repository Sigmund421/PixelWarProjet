using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponIcon : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public int weaponIndex;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    public Transform originalParent; 
    private ShopManager shopManager;
    public bool isInSlot = false; 

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

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent; // Enregistrement du parent d'origine
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isInSlot) return; // D�sactiver l'interaction si dans un slot

        if (shopManager != null && shopManager.IsShopOpen() && shopManager.HasPurchasedWeapon(weaponIndex))
        {
            canvasGroup.alpha = 0.6f; // Rendre l'ic�ne semi-transparente
            canvasGroup.blocksRaycasts = false; // Permettre � l'ic�ne d'�tre d�plac�e
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isInSlot) return; // D�sactiver l'interaction si dans un slot

        if (shopManager != null && shopManager.IsShopOpen() && shopManager.HasPurchasedWeapon(weaponIndex))
        {
            rectTransform.anchoredPosition += eventData.delta; // D�placer l'ic�ne avec le curseur
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isInSlot) return; // D�sactiver l'interaction si dans un slot

        if (shopManager == null || !shopManager.HasPurchasedWeapon(weaponIndex))
        {
            return; // Ne rien faire si l'arme n'a pas �t� achet�e
        }

        canvasGroup.alpha = 1f; // Restaurer l'opacit� compl�te
        canvasGroup.blocksRaycasts = true; // Restaurer le blocage des rayons

        if (!shopManager.OnDropWeapon(this))
        {
            // Revenir � la position initiale si le drop n'a pas �t� r�ussi
            ReturnToOriginalPosition();
        }
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent); // Restaurer le parent d'origine
        rectTransform.anchoredPosition = originalPosition; // Restaurer la position d'origine
        isInSlot = false; // Mettre � jour le bool�en
    }

    public void SetOriginalPosition(Vector2 newPosition)
    {
        originalPosition = newPosition;
    }
}

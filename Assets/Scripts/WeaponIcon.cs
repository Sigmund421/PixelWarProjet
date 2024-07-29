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
        if (isInSlot) return; // Désactiver l'interaction si dans un slot

        if (shopManager != null && shopManager.IsShopOpen() && shopManager.HasPurchasedWeapon(weaponIndex))
        {
            canvasGroup.alpha = 0.6f; // Rendre l'icône semi-transparente
            canvasGroup.blocksRaycasts = false; // Permettre à l'icône d'être déplacée
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isInSlot) return; // Désactiver l'interaction si dans un slot

        if (shopManager != null && shopManager.IsShopOpen() && shopManager.HasPurchasedWeapon(weaponIndex))
        {
            rectTransform.anchoredPosition += eventData.delta; // Déplacer l'icône avec le curseur
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isInSlot) return; // Désactiver l'interaction si dans un slot

        if (shopManager == null || !shopManager.HasPurchasedWeapon(weaponIndex))
        {
            return; // Ne rien faire si l'arme n'a pas été achetée
        }

        canvasGroup.alpha = 1f; // Restaurer l'opacité complète
        canvasGroup.blocksRaycasts = true; // Restaurer le blocage des rayons

        if (!shopManager.OnDropWeapon(this))
        {
            // Revenir à la position initiale si le drop n'a pas été réussi
            ReturnToOriginalPosition();
        }
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent); // Restaurer le parent d'origine
        rectTransform.anchoredPosition = originalPosition; // Restaurer la position d'origine
        isInSlot = false; // Mettre à jour le booléen
    }

    public void SetOriginalPosition(Vector2 newPosition)
    {
        originalPosition = newPosition;
    }
}

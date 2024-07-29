using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    public float speed = 18f;
    public float grappleMovementSpeedFactor = 0.8f; // Facteur de r�duction de la vitesse pendant le grappinage
    public float airControlFactor = 0.8f; // Facteur de contr�le en l'air
    public float maxSpeed = 18f; // Vitesse maximale
    public float decelerationFactor = 0.95f; // Facteur de d�c�l�ration pour �viter de glisser mais laisser �a smooth

    private float moveInput;

    [Header("Components")]
    private Rigidbody2D _rb;

    [Header("Jump Variables")]
    public float jumpForce = 6f;
    public LayerMask ground;
    public float fallMultiplier = 2f; // Multiplicateur de gravit� pendant la chute
    Collider2D _col;
    private bool isGround;

    [Header("Grappling")]
    public Tutorial_GrapplingGun grapplingGun; // R�f�rence au script du grappin

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        isGround = _col.IsTouchingLayers(ground);

        // Applique le multiplicateur de gravit� pendant la chute
        if (_rb.velocity.y < 0 && !isGround)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Le saut
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Mouvement horizontal
        if (grapplingGun.grappleRope.isGrappling)
        {
            _rb.AddForce(new Vector2(moveInput * speed * grappleMovementSpeedFactor, 0), ForceMode2D.Force);
        }
        else
        {
            float controlFactor = isGround ? 1f : airControlFactor;

            if (moveInput != 0)
            {
                // Applique une force instantan�e pour atteindre la vitesse maximale 
                _rb.velocity = new Vector2(moveInput * speed * controlFactor, _rb.velocity.y);
            }
            else
            {
                // Applique une l�g�re d�c�l�ration pour un arr�t smoooooth
                _rb.velocity = new Vector2(_rb.velocity.x * decelerationFactor, _rb.velocity.y);
            }

            // Limite la vitesse maximale
            if (Mathf.Abs(_rb.velocity.x) > maxSpeed)
            {
                _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * maxSpeed, _rb.velocity.y);
            }
        }

        // Arr�te instantan�ment le mouvement horizontal lorsque les touches de mouvement sont rel�ch�es
        if (isGround && moveInput == 0)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    public void EnableControls()
    {
        
        enabled = true;
    }

    public void DisableControls()
    {
        
        enabled = false;
    }

}

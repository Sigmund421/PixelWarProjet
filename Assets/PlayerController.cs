using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    public float speed;
    public float grappleMovementSpeedFactor = 0.1f; // Facteur de r�duction de la vitesse pendant le grappinage

    private float Move;

    [Header("Components")]
    private Rigidbody2D _rb;

    [Header("Jump Variables")]
    public float Jump;
    public LayerMask ground;
    public float fallMultiplier = 2.5f;  // Multiplicateur de gravit� pendant la chute
    Collider2D _col;
    private bool isGround;

    [Header("Grappling")]
    public Tutorial_GrapplingGun grapplingGun;  // R�f�rence au script du grappin

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {
        Move = Input.GetAxis("Horizontal");

        // Si le joueur est en train de grappiner, appliquez une force horizontale r�duite
        if (grapplingGun.grappleRope.isGrappling)
        {
            _rb.AddForce(new Vector2(Move * speed * grappleMovementSpeedFactor, 0), ForceMode2D.Force);
        }
        else
        {
            _rb.velocity = new Vector2(speed * Move, _rb.velocity.y);

            isGround = _col.IsTouchingLayers(ground);

            if (isGround && Input.GetKeyDown(KeyCode.Space))
            {
                _rb.AddForce(Vector2.up * Jump, ForceMode2D.Impulse);
            }
        }

        // Appliquez le multiplicateur de gravit� pendant la chute
        if (_rb.velocity.y < 0 && !isGround)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // Gardez les calculs physiques ici si n�cessaire
    }
}
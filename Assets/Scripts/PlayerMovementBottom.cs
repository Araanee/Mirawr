using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Déplacement")]
    public float speed = 5f;
    
    [Header("Saut")]
    public float jumpForce = 10f;
    public LayerMask groundLayer; // Pour détecter le sol
    public Transform groundCheck; // Point de vérification du sol (placer sous le chat)
    public float groundCheckRadius = 0.2f;
    
    [Header("Références")]
    public Transform bottomPlayer; // le chat du bas (ShadowPlayer)
    public Transform topMap;    // la map du haut
    public Transform bottomMap; // la map du bas
    public int varTest = 0;
    
    private Rigidbody2D rb;
    private Rigidbody2D bottomRb;
    private bool isGrounded;
    private bool jumpPressed;

    void Start()
    {
        // Récupère le Rigidbody2D du chat du haut (ce GameObject)
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D manquant sur " + gameObject.name);
        }
        
        // Récupère le Rigidbody2D du chat du bas
        if (bottomPlayer != null)
        {
            bottomRb = bottomPlayer.GetComponent<Rigidbody2D>();
            if (bottomRb == null)
            {
                Debug.LogError("Rigidbody2D manquant sur le chat du bas");
            }
        }
    }

    void Update()
    {
        // Déplacement horizontal
        float move = Keyboard.current.aKey.isPressed ? -speed * Time.deltaTime :
              Keyboard.current.dKey.isPressed ? speed * Time.deltaTime : 0f;
        
        // Le chat du haut se déplace
        transform.Translate(move, 0, 0);
        
        // Le chat du bas bouge en sens inverse (synchronisé)
        if (bottomPlayer != null)
            bottomPlayer.Translate(-move, 0, 0);
        
        // Détecte si le joueur appuie sur la touche de saut
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }
    }

    void FixedUpdate()
    {
        // Vérifie si le chat est au sol
        CheckGround();
        
        // Applique le saut si demandé et si au sol
        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false;
        }
    }

    void CheckGround()
    {
        // Vérifie si le chat touche le sol avec un cercle de détection
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Alternative simple si groundCheck n'est pas configuré
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        }
    }

    void Jump()
    {
        Debug.Log("SAUT !");
        
        // Applique une force vers le haut au chat du haut
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
        // Le chat du bas saute en sens inverse (vers le bas)
        if (bottomRb != null)
        {
            bottomRb.linearVelocity = new Vector2(bottomRb.linearVelocity.x, -jumpForce);
        }
    }

    // Pour visualiser la zone de détection du sol dans l'éditeur
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask ground; // Permet de sélectionner un ou plusieurs layers pour notre sol

    private Rigidbody2D myRB; // Référence à un rigidbody (le nôtre)
    private Vector2 stickDirection;
    private Animator myAnimator; // Référence à un animator (le nôtre)

    private bool isOnGround = false; // Permet de vérifier si on a le droit de sauter
    // Start is called before the first frame update

    private void OnEnable()
    {
        //On setup les contrôles avec leurs callbacks respectives
        var controls = new Controls();
        controls.Enable();
        controls.Main.Move.performed += MoveOnperformed;
        controls.Main.Move.canceled += MoveOncanceled;
        controls.Main.Jump.performed += JumpOnperformed;
    }

    /// <summary>
    /// Exécutée lorsque le bouton de saut est appuyé
    /// </summary>
    /// <param name="obj"></param>
    private void JumpOnperformed(InputAction.CallbackContext obj)
    {
        // Si isOnGround est vrai
        if (isOnGround)
        {
            // On saute
            myRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // Et on désactive la possibilité de sauter à nouveau
            isOnGround = false;
        }
    }

    /// <summary>
    /// Exécutée lorsque le stick ou les flèches directionnelles sont utilisés
    /// </summary>
    /// <param name="obj"></param>
    private void MoveOnperformed(InputAction.CallbackContext obj)
    {
        stickDirection = obj.ReadValue<Vector2>();
    }

    /// <summary>
    /// Exécutée lorsque l'on relache l'input de déplacement
    /// </summary>
    /// <param name="obj"></param>
    private void MoveOncanceled(InputAction.CallbackContext obj)
    {
        stickDirection = Vector2.zero;
    }

    void Start()
    {
        // On récupère les références à nos composants
        myRB = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        var direction = new Vector2
        {
            x = stickDirection.x,
            y = 0
        };
        // Déplacement
        if (myRB.velocity.sqrMagnitude < maxSpeed)
        {
            myRB.AddForce(direction * speed);
        }

        /*
        if (Mathf.Abs(myRB.velocity.x) > 0.1f)
        {
            myAnimator.SetBool("IsRunning", true);
        }
        else
        {
           myAnimator.SetBool("IsRunning", false);
        }
        */
        // Contrôle des variables de l'animator
        var isRunning = isOnGround && Mathf.Abs(myRB.velocity.x) > 0.1f;
        myAnimator.SetBool("IsRunning", isRunning);
        var isAscending = !isOnGround && myRB.velocity.y > 0;
        myAnimator.SetBool("IsAscending", isAscending);
        var isDescending = !isOnGround && myRB.velocity.y < 0;
        myAnimator.SetBool("IsDescending", isDescending);
        myAnimator.SetBool("IsGrounded", isOnGround);
        Flip();
    }
    
    /// <summary>
    /// Permet de faire flip le sprite afin que le joueur regarde dans la bonne direction
    /// </summary>
    private void Flip()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Booléen vérifiant si le layer sur lequel on a atteri appartient bien au layerMask "ground"
        var touchGround = ground == (ground | (1 << other.gameObject.layer));
        // Booléen vérifiant que l'on collisionne avec une surface horizontale
        var touchFromAbove = other.contacts[0].normal == Vector2.up;
        if (touchGround && touchFromAbove)
            //if (other.gameObject.CompareTag("Ground") == true)
        {
            isOnGround = true;
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask ground;

    private Rigidbody2D myRB;
    private Vector2 stickDirection;
    private Animator myAnimator;

    private bool isOnGround = false;
    // Start is called before the first frame update

    private void OnEnable()
    {
        var controls = new Controls();
        controls.Enable();
        controls.Main.Move.performed += MoveOnperformed;
        controls.Main.Move.canceled += MoveOncanceled;
        controls.Main.Jump.performed += JumpOnperformed;
    }

    private void JumpOnperformed(InputAction.CallbackContext obj)
    {
        if (isOnGround)
        {
            myRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isOnGround = false;
        }
    }

    private void MoveOnperformed(InputAction.CallbackContext obj)
    {
        stickDirection = obj.ReadValue<Vector2>();
    }

    private void MoveOncanceled(InputAction.CallbackContext obj)
    {
        stickDirection = Vector2.zero;
    }

    void Start()
    {
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
        var isRunning = isOnGround && Mathf.Abs(myRB.velocity.x) > 0.1f;
        myAnimator.SetBool("IsRunning", isRunning);
        var isAscending = !isOnGround && myRB.velocity.y > 0;
        myAnimator.SetBool("IsAscending", isAscending);
        var isDescending = !isOnGround && myRB.velocity.y < 0;
        myAnimator.SetBool("IsDescending", isDescending);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var touchGround = ground == (ground | (1 << other.gameObject.layer));
        var touchFromAbove = other.contacts[0].normal == Vector2.up;
        if (touchGround && touchFromAbove)
        //if (other.gameObject.CompareTag("Ground") == true)
        {
            isOnGround = true;
        }
    }
}
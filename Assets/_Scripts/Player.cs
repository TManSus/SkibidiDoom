using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    // si lees esto lameme los huevos
    Rigidbody rb;
    Camera cam;
    public TextMeshProUGUI healthText;
    public RectTransform healthBarFill;
    public int health;
    public int weapon;
    public int pistolDamage;
    public int shotgunDamage;
    public int rifleDamage;
    public bool grounded;
    public bool jumpable;
    public bool moving;
    public bool crouching;
    public float moveSpeed;
    public float walkSpeed;
    public float airSpeed;
    public float runSpeed;
    public float crouchSpeed;
    public float mouseSensitivity;
    public float maxSpeedGrounded;
    public float deacceleratingCoef;
    float rotationCamX = 0;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = transform.GetChild(0).GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        CheckGround();
        MovementVoid();
        MouseRotation();
        HealthBar();
        SlideCrouch();
        ShootController();
    }
    void CheckGround()
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.95f, transform.position.z), Vector3.down, Color.red, 0.2f);
        if(!crouching)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.95f, transform.position.z), Vector3.down, out hit, 0.2f))
            {
                if (!grounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                }
                grounded = true;
                if (rb.linearVelocity.x >= maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(Mathf.Lerp(rb.linearVelocity.x, maxSpeedGrounded, deacceleratingCoef), rb.linearVelocity.y, rb.linearVelocity.z);
                }
                else if (rb.linearVelocity.z >= maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, Mathf.Lerp(rb.linearVelocity.z, maxSpeedGrounded, deacceleratingCoef));
                }
                if (rb.linearVelocity.x <= -maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(Mathf.Lerp(rb.linearVelocity.x, maxSpeedGrounded, deacceleratingCoef), rb.linearVelocity.y, rb.linearVelocity.z);
                }
                else if (rb.linearVelocity.z <= -maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, Mathf.Lerp(rb.linearVelocity.z, maxSpeedGrounded, deacceleratingCoef));
                }
            }
            else
            {
                grounded = false;
            }
        }else
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z), Vector3.down, out hit, 0.2f))
            {
                if (!grounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                }
                grounded = true;
                if (rb.linearVelocity.x >= maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(Mathf.Lerp(rb.linearVelocity.x, maxSpeedGrounded, deacceleratingCoef), rb.linearVelocity.y, rb.linearVelocity.z);
                }
                else if (rb.linearVelocity.z >= maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, Mathf.Lerp(rb.linearVelocity.z, maxSpeedGrounded, deacceleratingCoef));
                }
                if (rb.linearVelocity.x <= -maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(Mathf.Lerp(rb.linearVelocity.x, maxSpeedGrounded, deacceleratingCoef), rb.linearVelocity.y, rb.linearVelocity.z);
                }
                else if (rb.linearVelocity.z <= -maxSpeedGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, Mathf.Lerp(rb.linearVelocity.z, maxSpeedGrounded, deacceleratingCoef));
                }
            }
            else
            {
                grounded = false;
            }
        }

    }
    void MovementVoid()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * moveSpeed, ForceMode.Force);
            moving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(transform.right * -1 * moveSpeed, ForceMode.Force);
            moving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(transform.forward * -1 * moveSpeed, ForceMode.Force);
            moving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * moveSpeed, ForceMode.Force);
            moving = true;
        }
        if (Input.GetKey(KeyCode.Space) && grounded && jumpable)
        {
            jumpable = false;
            rb.AddForce(0, 10, 0, ForceMode.Impulse);
            grounded = false;
            StartCoroutine(JumpWait());
        }
        if (Input.GetKey(KeyCode.LeftShift) && !crouching)
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
        if (grounded)
        {
            rb.linearVelocity = rb.linearVelocity / 1.1f;
        }
        else
        {
            rb.linearVelocity = rb.linearVelocity / 1.01f;
            moveSpeed = airSpeed;
        }
    }
    IEnumerator JumpWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        jumpable = true;
    }
    void MouseRotation()
    {
        float y = Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationCamX += Input.GetAxis("Mouse Y") * -1 * mouseSensitivity * 0.01f;
        rotationCamX = Mathf.Clamp(rotationCamX, -0.7071068f, 0.7071068f);
        transform.Rotate(0, y, 0);
        cam.transform.localRotation = new Quaternion(rotationCamX, 0, 0, cam.transform.localRotation.w);
    }
    void HealthBar()
    {
        healthBarFill.sizeDelta = new Vector2(health * 3, 50);
        healthBarFill.localPosition = new Vector2(health * 3 / 2 - 150, 0);
        healthText.text = health.ToString();
    }
    public void DamagePlayer(int damage)
    {
        health -= damage;
    }
    void SlideCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveSpeed = crouchSpeed;
            if ((rb.linearVelocity.x > 0.5f || rb.linearVelocity.z > 0.5f || rb.linearVelocity.x < -0.5f || rb.linearVelocity.z < -0.5f) && !crouching && grounded)
            {
                rb.AddForce(transform.forward * 10, ForceMode.Impulse);
                if (transform.localScale.y == 1)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                }
                transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            }
            else
            {
                if (transform.localScale.y == 1)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                }
                transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            }
            crouching = true;
        }
        else
        {
            crouching = false;
            if (transform.localScale.y < 1)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            }
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        }
    }
    void ShootController()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(weapon == 0)
            {
                Debug.DrawRay(cam.transform.position + transform.forward * 0.25f, transform.forward, Color.red, 100000f);
                Physics.Raycast(cam.transform.position + transform.forward * 0.25f, transform.forward, out RaycastHit hit, 100000f);
                if(hit.collider != null)
                {
                    if (hit.collider.GetComponent<Enemy>() != null)
                    {
                        hit.collider.GetComponent<Enemy>().Damage(pistolDamage);
                    }
                }
            }
        }
    }
}

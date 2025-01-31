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
    public AudioSource shootSoundSource;
    public LayerMask enemyMask;
    public int health;
    public int weapon;
    public int pistolDamage;
    public int shotgunDamage;
    public int rifleDamage;
    public int sniperDamage;
    public int pistolAmmo;
    public int shotgunAmmo;
    public int rifleAmmo;
    public int sniperAmmo;
    public bool grounded;
    public bool jumpable;
    public bool moving;
    public bool crouching;
    public bool shootable;
    public bool shootableControl;
    public bool shotgunUnlocked;
    public bool rifleUnlocked;
    public bool sniperUnlocked;
    public float moveSpeed;
    public float walkSpeed;
    public float airSpeed;
    public float runSpeed;
    public float crouchSpeed;
    public float mouseSensitivity;
    public float maxSpeedGrounded;
    public float deacceleratingCoef;
    public float shootCooldown;
    public float pistolCooldown;
    public float shotgunCooldown;
    public float rifleCooldown;
    public float sniperCooldown;
    public float bulletSpread;
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
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("pickup"))
        {
            if (other.GetComponent<PickupManager>().pickupID == 100)
            {
                shotgunUnlocked = true;
            }
            if (other.GetComponent<PickupManager>().pickupID == 101)
            {
                rifleUnlocked = true;
            }
            if (other.GetComponent<PickupManager>().pickupID == 102)
            {
                sniperUnlocked = true;
            }
            Destroy(other.transform.parent.gameObject);
        }
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
        if(Input.GetMouseButton(0) && shootable)
        {
            shootable = false;
            if(weapon == 0)
            {
                shootSoundSource.Play();
                Debug.DrawLine(cam.transform.position + cam.transform.forward * 0.25f, cam.transform.position + cam.transform.forward * 100000.25f, Color.red, 1);
                Physics.Raycast(cam.transform.position + cam.transform.forward * 0.25f, cam.transform.forward, out RaycastHit hit, Mathf.Infinity, enemyMask);
                if(hit.collider != null)
                {
                    if (hit.collider.GetComponent<Enemy>() != null)
                    {
                        hit.collider.GetComponent<Enemy>().Damage(pistolDamage);
                    }
                }
            }
        }
        if (!shootable && !shootableControl)
        {
            StartCoroutine(ShootCooldown());
            shootableControl = true;
        }
    }
    IEnumerator ShootCooldown()
    {
        yield return new WaitForSecondsRealtime(shootCooldown);
        shootable = true;
        shootableControl = false;
    }
    void WeaponSwitch()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {

        }
    }
}

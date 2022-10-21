using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public static RubyController instance { get; private set; }

    public float speed = 3.0f;

    public int maxHealth = 5;
    public int health { get { return currentHealth; } } //khi currentHealth thay đổi thì health thay đổi
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;

    private AudioSource audioSource;
    public AudioClip throwCog;
    public AudioClip hit;
    //Attack
    [SerializeField] private int attackSpeed = 300;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int maxBullet = 20;
    float cooldownTimer = Mathf.Infinity;
    [SerializeField] private int remainingBullet;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //Debug.Log(isInvincible);
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        remainingBullet = maxBullet;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f)) // nếu x khác 0 hoặc y khác 0
        {
            lookDirection.Set(move.x, move.y); // gán x và y vào vector lookDirection
            lookDirection.Normalize(); //độ lớn bằng 1
            //Debug.Log(lookDirection);
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        // shoot
        cooldownTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.C) && cooldownTimer > attackCooldown && remainingBullet >= 0)
        {
            Launch();
            PlaySound(throwCog);
        }
        //talk NPC
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + 3.0f * horizontal * Time.deltaTime;
        position.y = position.y + 3.0f * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0) // truyền vào giá trị -hp
        {
            animator.SetTrigger("Hit");

            PlaySound(hit);

            if (isInvincible) //nếu true dừng
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth); //trả về giá trị nằm trong khoảng
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    void Launch()
    {
        cooldownTimer = 0;
        remainingBullet = Mathf.Clamp(remainingBullet - 1, 0, maxBullet);
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, attackSpeed);

        animator.SetTrigger("Launch");
    }
    public void addBullet()
    {
        remainingBullet = Mathf.Clamp(remainingBullet + 1, 0, maxBullet);
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

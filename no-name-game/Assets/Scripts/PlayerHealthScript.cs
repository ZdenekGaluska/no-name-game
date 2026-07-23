using UnityEngine;

public class PlayerHealthScript : MonoBehaviour
{
    public int health = 3;
    private int _currentHealth;
    public float regenerationRate = 3f;
    private float _currentTimeToReg = 0f;
    
    private bool _isAlive = true;
    public bool IsAlive => _isAlive;

    private bool _isInvincible = false;
    private float _invincibleTimer = 0f;
    public float invincibleDuration = 0.5f;

    public HealthUiScript healthUIScript;
    void Start()
    {
        healthUIScript.UpdateText(health, health);
        _currentHealth = health;
    }

    void HealthRegen()
    {
        if (_currentHealth < health &&  _isAlive)
        {
            _currentTimeToReg += Time.deltaTime;
            if (_currentTimeToReg >= regenerationRate)
            {

                _currentTimeToReg = 0f;
                _currentHealth++;
                healthUIScript.UpdateText(_currentHealth, health);
            }
        }
    }

    void TakeDamage()
    {
        if (_isInvincible || !_isAlive) return;
        
        Debug.Log("Current health: " + _currentHealth);
        _currentHealth -= 1;
        _currentTimeToReg = 0f;
        _isInvincible = true;
        _invincibleTimer = 0f;
        healthUIScript.UpdateText(_currentHealth, health);

        if (_currentHealth <= 0)
        {
            _isAlive = false;
        }
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        HealthRegen();

        if (_isInvincible)
        {
            _invincibleTimer += Time.fixedDeltaTime;
            if (_invincibleTimer >= invincibleDuration)
            {
                _isInvincible = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
}

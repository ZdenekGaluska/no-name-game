using System.Collections;
using UnityEngine;

public class 
    WizzardScript : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform shrinkingSpawnArea;
    [SerializeField] private GameObject spawnArea;
    [SerializeField] private float spawnAreaShrinkDuration = 2f;
    
    [SerializeField] private GameObject wizardBody;
    private Rigidbody2D rb;
    
    [SerializeField] private float minCastDelay = 2f;
    [SerializeField] private float maxCastDelay = 6f;
    private float _castDelay;
    
    [SerializeField] private float minLifetime = 15f;
    [SerializeField] private float maxLifetime = 20f;
    private float _lifetime;
    private float _currentLifetime = 0f;

    [SerializeField] private float spreadRange = 80f;
    [SerializeField] private float minWalkDuration = 1f;
    [SerializeField] private float maxWalkDuration = 2f;
    private float _walkDuration;
    
    private float _currentWalkDuration = 0f;
    [SerializeField] private float speed = 5f;
    private Vector2 direction;
    
    private bool _canWalk = false;
    
    [SerializeField] private float armagedonCastInterval = 1f;
    [SerializeField] private int numberOfFBInArmagedon = 8;
    private Spells spell;
    
    private enum Spells
    {
        Armagedon = 0,
        FireballLine = 1
    }

    void Start()
    {
        _lifetime = Random.Range(minLifetime, maxLifetime);
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(WizardRoutine());
        float angle =  Random.Range(0f, Mathf.PI * 2f);
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        _walkDuration = Random.Range(minWalkDuration, maxWalkDuration);
        
        Spells spell = (Spells)Random.Range(0, 0);
        _castDelay = Random.Range(minCastDelay, maxCastDelay);
    }

    void FixedUpdate()
    {
        _currentLifetime += Time.fixedDeltaTime;
        if (_canWalk)
        {
            Walk();
        }

    }

    void Walk()
    {
        if (IsClose())
        {
            _currentWalkDuration = _walkDuration;
        }
        _currentWalkDuration += Time.fixedDeltaTime;
        if (_currentWalkDuration >= _walkDuration)
        {
            direction = CalculateDirection();
            _walkDuration = Random.Range(minWalkDuration, maxWalkDuration);
            _currentWalkDuration = 0f;
        }
        rb.linearVelocity = direction * speed;
        
    }

    Vector2 CalculateDirection()
    {
        float angleDeg = Random.Range(-spreadRange, spreadRange);
        float angleRad = angleDeg * Mathf.Deg2Rad;
        Vector2 toCenter = (Vector3.zero - transform.position).normalized;
        return MathHelper.RotateVector(toCenter, angleRad);
    }

    bool IsClose()
    {
        return !DespawnHelper.IsInside(transform.position, 0.5f);
    }
    

    private IEnumerator WizardRoutine()
    {
        float currentShrinkTime = 0f;
        Vector3 startScale = shrinkingSpawnArea.localScale;
        while (currentShrinkTime < spawnAreaShrinkDuration)
        {
            currentShrinkTime += Time.deltaTime;
            shrinkingSpawnArea.localScale = Vector3.Lerp(startScale, Vector3.zero, currentShrinkTime / spawnAreaShrinkDuration);
            yield return null;
        }
        currentShrinkTime = 0f;
        
        shrinkingSpawnArea.localScale = Vector3.zero;
        spawnArea.GetComponent<Collider2D>().enabled = true;
        yield return null;
        
        wizardBody.SetActive(true);
        spawnArea.GetComponent<Collider2D>().enabled = false;
        spawnArea.SetActive(false);
        _canWalk = true;

        float currentCastTime = 0f;
        while (true)
        {
            currentCastTime+= Time.deltaTime;
            
            if (currentCastTime >= _castDelay)
            {
                switch (spell)
                {
                    case Spells.Armagedon:
                        yield return StartCoroutine(ArmagedonRoutine());
                        break;
                    case Spells.FireballLine:
                        yield return StartCoroutine(FireballLineRoutine());
                        break;   
                }
                currentCastTime = 0f;
            }
            yield return null;

            if (_currentLifetime >= _lifetime)
            {
                break;
            }
        }
        
        _canWalk = false;
        rb.linearVelocity = Vector3.zero;
        spawnArea.SetActive(true);
        spawnArea.transform.localScale *= 2;
        spawnAreaShrinkDuration *= 1.5f;
        while (currentShrinkTime < spawnAreaShrinkDuration)
        {
            currentShrinkTime += Time.deltaTime;
            shrinkingSpawnArea.localScale = Vector3.Lerp(Vector3.zero,startScale, currentShrinkTime / spawnAreaShrinkDuration);
            yield return null;
        }
        spawnArea.GetComponent<Collider2D>().enabled = true;

        yield return null;
        
        Destroy(gameObject);
    }

    private IEnumerator ArmagedonRoutine()
    {

        int currentNumber = 0;
        _canWalk = false;
        rb.linearVelocity = Vector3.zero;
        Vector2 castDirection = -(transform.position - player.transform.position).normalized * 1.5f;
        Vector2 castPosition = (Vector2)transform.position + castDirection;

        while (currentNumber < numberOfFBInArmagedon)
        {

            if (!DespawnHelper.IsInside(castPosition, -1f))
            {
                break;
            }
            currentNumber++;
            if (currentNumber > 6)
            {
                _canWalk = true;
            }

            yield return new WaitForSeconds(armagedonCastInterval);
            GameObject fireballObj =  Instantiate(fireballPrefab, new Vector2(0,0), Quaternion.identity);
            BurningAreaScript fireballScript = fireballObj.GetComponent<BurningAreaScript>();
            fireballScript.ShootFireball(transform.position, castPosition);
            castPosition += castDirection;
        }
        _canWalk = true;
    }

    private IEnumerator FireballLineRoutine()
    {
        return null;
    }
    
}

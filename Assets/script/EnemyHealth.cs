using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityDuration = 0.5f;

    [Header("Feedback")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;

    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged; // Passes health percentage

    private int currentHealth;
    private bool isInvulnerable;
    private AudioSource audioSource;

    public int CurrentHealth { get; internal set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        onHealthChanged.Invoke(GetHealthPercentage());

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    public void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        audioSource.PlayOneShot(deathSound);
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void SetMaxHealth(int value, bool v) => maxHealth = value;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;

    internal int GetCurrentHealth()
    {
        throw new NotImplementedException();
    }

    internal void SetMaxHealth(int pv)
    {
        throw new NotImplementedException();
    }
}
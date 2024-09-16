﻿using BaseProject.CompositPattern;
using BaseProject.GameManagement;
using Microsoft.Xna.Framework;
using System;

namespace BaseProject.ComponentPattern.Other;
public class Health : Component
{
    private float _damageTimerTotal = 0.05f;
    private double _damageTimer;
    public Color DamageTakenColor { get; private set; } = Color.Red;
    private SpriteRenderer _spriteRenderer;

    public int MaxHealth { get; private set; } = -1;

    /// <summary>
    /// Gets set in Start of Health component
    /// </summary>
    public int CurrentHealth { get; set; } = -1;
    public float NormalizedHealth
    {
        get
        {
            return (float)CurrentHealth / (float)MaxHealth;
        }
    }
    public bool CanTakeDamage = true;
    public Action OnDamageTaken { get; set; }
    public Action OnZeroHealth { get; set; }
    public Action OnResetColor { get; set; }
    public Action On75Hp { get; set; }
    public Action On50Hp { get; set; }
    public Action On25Hp { get; set; }
    /// <summary>
    /// Gets called every frame when health is under 50ptc
    /// </summary>
    public Action On50orUnder { get; set; }

    public bool IsDead { get; private set; }
    public Action<int> AmountDamageTaken { get; set; }
    public Action<Vector2> AttackerPositionDamageTaken { get; set; }

    public Health(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
    }

    public override void Start()
    {
        if (CurrentHealth != -1) return; // Current health has already been set e.g in the DB, where we load the Player
        CurrentHealth = MaxHealth;
    }

    public override void Update()
    {
        HandleOnDamage();
        UpdateActionChecks();
    }

    public void SetHealth(int maxhealth)
    {
        if (MaxHealth != -1) return; // Already have set maxHealth like in cheats

        MaxHealth = maxhealth;

        if (CurrentHealth == -1)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public bool AddHealth(int addHealth)
    {
        if (CurrentHealth == MaxHealth) return false;

        CurrentHealth += addHealth;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackersPosition">This is what will later be used if the character gets hit with a projectile.
    /// <para>So we dont make it spray blood out from where the magic caster was (Would be weird)</para></param>
    public void TakeDamage(int damage, Vector2 attackersPosition)
    {
        if (IsDead || !CanTakeDamage) return; // Already dead

        AttackerPositionDamageTaken?.Invoke(attackersPosition);
        AmountDamageTaken?.Invoke(damage);

        int newHealth = CurrentHealth - damage;

        if (newHealth < 0) CurrentHealth = 0;
        else CurrentHealth = newHealth;

        if (CurrentHealth > 0)
        {
            DamageTaken();
            CheckDmgLeft();
            return;
        }

        IsDead = true;
        ResetColor();

        OnZeroHealth?.Invoke();
        OnZeroHealth = null;
    }

    private void DamageTaken()
    {
        OnDamageTaken?.Invoke(); // For specific behavor when Damage taken

        _damageTimer = _damageTimerTotal;
        _spriteRenderer.Color = DamageTakenColor;
    }

    private void CheckDmgLeft()
    {
        float normalizedHealth = NormalizedHealth;

        // Need to change it to not just turn into null but should unsubscribe on each of the places.
        if (normalizedHealth <= 0.75f)
        {
            On75Hp?.Invoke();
            On75Hp = null;
        }

        if (normalizedHealth <= 0.5f)
        {
            On50Hp?.Invoke();
            On50Hp = null;
        }

        if (normalizedHealth <= 0.25f)
        {
            On25Hp?.Invoke();
            On25Hp = null;
        }
    }

    private void UpdateActionChecks()
    {
        if (NormalizedHealth <= 0.5f)
        {
            On50orUnder?.Invoke();
        }
    }

    private void ResetColor()
    {
        if (IsDead) return;

        _spriteRenderer.Color = Color.White;
        OnResetColor?.Invoke();
    }

    private void HandleOnDamage()
    {
        if (_damageTimer <= 0) return;

        _damageTimer -= GameWorld.DeltaTime;

        // Count down
        if (_damageTimer <= 0)
            ResetColor();
    }
}
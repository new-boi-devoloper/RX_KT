using System;
using R3;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public readonly BehaviorSubject<int> Hp = new(100);
    public readonly BehaviorSubject<int> Medkits = new(3);
    public readonly BehaviorSubject<int> Mana = new(100);
    public readonly BehaviorSubject<int> Coins = new(50);
    public readonly BehaviorSubject<bool> IsSpellOnCooldown = new(false);
    
    private IDisposable cooldownDisposable;

    public void CastSpell()
    {
        if (Mana.Value >= 20 && Hp.Value > 0 && !IsSpellOnCooldown.Value)
        {
            Mana.OnNext(Mana.Value - 20);
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        cooldownDisposable?.Dispose();
        IsSpellOnCooldown.OnNext(true);
        cooldownDisposable = Observable.Timer(TimeSpan.FromSeconds(3))
            .Subscribe(_ => IsSpellOnCooldown.OnNext(false))
            .AddTo(this);
    }
    
    public void Heal()
    {
        if (Medkits.Value > 0 && Hp.Value < 100)
        {
            int newHp = Mathf.Min(Hp.Value + 30, 100);
            Hp.OnNext(newHp);          
            Medkits.OnNext(Medkits.Value - 1);
        }
    }

    public void TakeDamage(int amount)
    {
        int newHp = Mathf.Max(Hp.Value - amount, 0);
        Hp.OnNext(newHp);
    }
}
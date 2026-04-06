using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI medkitsText;
    [SerializeField] private Button healButton;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button spellButton;
    
    private readonly int lowHpThreshold = 30;
    private readonly int maxHp = 100;

    private void Start()
    {
        player.Hp.Subscribe(hp =>
        {
            hpText.text = $"HP: {hp}";
            warningPanel.SetActive(hp < lowHpThreshold && hp > 0);
        }).AddTo(this);  

        player.Medkits.Subscribe(med => medkitsText.text = $"Medkits: {med}")
            .AddTo(this);

        Observable.CombineLatest(player.Hp, player.Medkits,
                (hp, med) => med > 0 && hp < maxHp && hp > 0)
            .Subscribe(canHeal => healButton.interactable = canHeal)
            .AddTo(this);

        player.Hp.Where(hp => hp == 0)
            .Subscribe(_ =>
            {
                gameOverPanel.SetActive(true);
                healButton.interactable = false;
            }).AddTo(this);
        
        player.Mana.Subscribe(mana =>
        {
            manaText.text = $"Mana: {mana}";
            manaText.color = mana < 20 ? Color.red : Color.white;
        }).AddTo(this);

        player.Coins.Subscribe(coins => coinsText.text = $"Coins: {coins}")
            .AddTo(this);

        Observable.CombineLatest(
                player.Hp,
                player.Mana,
                player.IsSpellOnCooldown,
                (hp, mana, cd) => hp > 0 && mana >= 20 && !cd
            ).Subscribe(canCast => spellButton.interactable = canCast)
            .AddTo(this);

        Observable.CombineLatest(player.Hp, player.Mana, (hp, mana) => hp + mana)
            .Subscribe(sum => Debug.Log($"Total HP+Mana: {sum}"))
            .AddTo(this);
    }

}
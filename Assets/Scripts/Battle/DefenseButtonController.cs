using UnityEngine;

public class DefenseButtonController : MonoBehaviour
{
    [Header("Defense Button Settings")]
    [SerializeField] int defenseCost = 7;
    [SerializeField] int defense = 30; // 防御力の値（何％カットさせるか）
    [SerializeField] int defenseTimer = 5; // 防御の持続時間
    [SerializeField] int defenseCooldown = 2; // クールダウン時間
    [SerializeField] private GameObject hero; // 主人公のGameObject


    HeroController heroController; // HeroControllerの参照を保存

    private float cooldownTimer = 0f; //クールダウンタイマー

    // クールダウン状態を確認するプロパティ
    public bool IsOnCooldown => cooldownTimer < defenseCooldown;
    public float CooldownProgress => Mathf.Clamp01(cooldownTimer / defenseCooldown);
    public float RemainingCooldown => Mathf.Max(0f, defenseCooldown - cooldownTimer);

    void Start()
    {
        heroController = hero.GetComponent<HeroController>();
        cooldownTimer = defenseCooldown; // 初期状態ではクールダウン中でない
    }
    void Update()
    {
        // クールダウンタイマーを更新
        if (cooldownTimer < defenseCooldown)
        {
            cooldownTimer += Time.deltaTime;
            // クールダウン中は灰色に
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false; // ボタンを無効化
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(128, 128, 128, 255); // 灰色に変更
        }
        // クールダウンが終了したらタイマーをリセット
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true; // ボタンを有効化
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(255, 255, 255, 255); // 元の色に戻す
        }
    }

    public void OnClick()
    {
        print("Defense button clicked!");

        // クールダウン中の場合は処理を停止
        if (IsOnCooldown)
        {
            print($"Defense is on cooldown! Remaining: {RemainingCooldown:F1}s");
            return;
        }

        // コストが足りない場合は処理を停止
        if (heroController.hStatus.cost < defenseCost)
        {
            print($"Not enough cost! Required: {defenseCost}, Current: {heroController.hStatus.cost}");
            return;
        }

        // 攻撃実行
        cooldownTimer = 0f; // クールダウンをリセット
        OnClickDefense();
        heroController.hStatus.cost -= defenseCost;
        print($"Defense executed! Cooldown started ({defenseCooldown}s)");
    }
    void OnClickDefense()
    {
        print("Defense activated!");
        HeroController heroController = hero.GetComponent<HeroController>();
        heroController.ActivateDefense(defense, defenseTimer);
    }
}

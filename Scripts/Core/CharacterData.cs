using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Monopoly/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string description;
    public Sprite icon;
    
    // 被动加成
    public float propertyBuyDiscount;      // 地产购买折扣（负数为折扣）
    public float rentIncomeBonus;         // 过路费收入加成（正数为加成）
    public int startBonusGold;           // 起点奖励加成
    public int initialGoldAdjustment;    // 初始金币调整（负数为减少）
    public int extraCards;               // 初始额外道具卡数量
    public int diceMinValue;            // 骰子最小值
    public bool freeHospitalPolice;     // 医院/警局免罚款
    public float upgradeCostDiscount;   // 升级费用折扣（负数为折扣）
    
    // 代价
    public int extraHospitalPoliceFine;  // 被送医/警局额外罚款
}

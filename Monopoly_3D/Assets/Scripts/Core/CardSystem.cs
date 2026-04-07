using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 卡牌类型枚举
/// </summary>
public enum CardType
{
    FixedDice,      // 定点骰子卡
    DoubleRent,     // 双倍租金卡
    Immunity,       // 免罚卡
    Frame,          // 陷害卡
    Demolish,       // 拆迁卡
    Extort,         // 勒索卡
    LeaveHospital,  // 出院卡
    LeavePolice     // 保释卡
}

/// <summary>
/// 卡牌类
/// </summary>
public class Card
{
    public CardType type;
    public string name;
    public string description;
    
    public Card(CardType type, string name, string description)
    {
        this.type = type;
        this.name = name;
        this.description = description;
    }
}

/// <summary>
/// 卡牌系统 - 负责道具卡和机会卡的管理
/// </summary>
public class CardSystem : MonoBehaviour
{
    private List<Card> allCards = new List<Card>();
    private List<Card> chanceCards = new List<Card>();
    
    private void Awake()
    {
        // 单例模式在 GameManager 中统一管理
    }

    /// <summary>
    /// 初始化卡牌系统
    /// </summary>
    public void Initialize()
    {
        CreateCards();
        CreateChanceCards();
        Debug.Log($"卡牌系统初始化完成，道具卡 {allCards.Count} 张，机会卡 {chanceCards.Count} 张");
    }
    
    /// <summary>
    /// 创建道具卡
    /// </summary>
    private void CreateCards()
    {
        allCards.Add(new Card(CardType.FixedDice, "定点骰子卡", "本回合指定掷出 1~6 任意点数"));
        allCards.Add(new Card(CardType.DoubleRent, "双倍租金卡", "下一次收取过路费翻倍"));
        allCards.Add(new Card(CardType.Immunity, "免罚卡", "免疫一次医院/警局/税务惩罚"));
        allCards.Add(new Card(CardType.Frame, "陷害卡", "将指定玩家送入医院"));
        allCards.Add(new Card(CardType.Demolish, "拆迁卡", "拆除对手1级房屋，支付少量补偿"));
        allCards.Add(new Card(CardType.Extort, "勒索卡", "抽取对手 100~300 金币"));
        allCards.Add(new Card(CardType.LeaveHospital, "出院卡", "自身在医院时立即离开"));
        allCards.Add(new Card(CardType.LeavePolice, "保释卡", "自身在警局时立即离开"));
    }
    
    /// <summary>
    /// 创建机会卡
    /// </summary>
    private void CreateChanceCards()
    {
        chanceCards.Add(new Card(CardType.FixedDice, "幸运骰子", "获得一次定点骰子机会"));
        chanceCards.Add(new Card(CardType.DoubleRent, "租金翻倍", "获得一张双倍租金卡"));
        chanceCards.Add(new Card(CardType.Immunity, "免罚机会", "获得一张免罚卡"));
        chanceCards.Add(new Card(CardType.Frame, "不幸遭遇", "被陷害送入医院"));
        chanceCards.Add(new Card(CardType.Demolish, "意外事故", "一栋房屋被拆除"));
        chanceCards.Add(new Card(CardType.Extort, "破财消灾", "支付 200 金币"));
        chanceCards.Add(new Card(CardType.LeaveHospital, "健康祝福", "获得一张出院卡"));
        chanceCards.Add(new Card(CardType.LeavePolice, "法律援助", "获得一张保释卡"));
    }
    
    /// <summary>
    /// 随机获取一张道具卡
    /// </summary>
    public Card GetRandomCard()
    {
        if (allCards.Count == 0) return null;
        return allCards[Random.Range(0, allCards.Count)];
    }
    
    /// <summary>
    /// 随机获取一张机会卡
    /// </summary>
    public Card GetRandomChanceCard()
    {
        if (chanceCards.Count == 0) return null;
        return chanceCards[Random.Range(0, chanceCards.Count)];
    }
    
    /// <summary>
    /// 给玩家随机发放一张道具卡
    /// </summary>
    public bool GiveRandomCard(Player player)
    {
        if (player == null)
        {
            Debug.LogWarning("GiveRandomCard called with null player");
            return false;
        }
        
        if (!player.HasFreeSlots())
        {
            UIManager.Instance.ShowError("道具卡已满！");
            return false;
        }
        
        Card card = GetRandomCard();
        if (player.AddCard(card))
        {
            UIManager.Instance.ShowMessage($"{player.nickname} 获得了 {card.name}！");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 执行卡牌效果
    /// </summary>
    public void ExecuteCard(Card card, Player player)
    {
        if (card == null || player == null)
        {
            Debug.LogWarning("ExecuteCard called with null card or player");
            return;
        }
        
        switch (card.type)
        {
            case CardType.FixedDice:
                ExecuteFixedDiceCard(player);
                break;
            case CardType.DoubleRent:
                ExecuteDoubleRentCard(player);
                break;
            case CardType.Immunity:
                ExecuteImmunityCard(player);
                break;
            case CardType.Frame:
                ExecuteFrameCard(player);
                break;
            case CardType.Demolish:
                ExecuteDemolishCard(player);
                break;
            case CardType.Extort:
                ExecuteExtortCard(player);
                break;
            case CardType.LeaveHospital:
                ExecuteLeaveHospitalCard(player);
                break;
            case CardType.LeavePolice:
                ExecuteLeavePoliceCard(player);
                break;
        }
        
        player.RemoveCard(card);
    }
    
    /// <summary>
    /// 执行定点骰子卡
    /// </summary>
    private void ExecuteFixedDiceCard(Player player)
    {
        UIManager.Instance.ShowFixedDicePanel();
    }
    
    /// <summary>
    /// 执行双倍租金卡
    /// </summary>
    private void ExecuteDoubleRentCard(Player player)
    {
        player.hasDoubleRent = true;
        UIManager.Instance.ShowMessage($"{player.nickname} 启用了双倍租金卡！");
    }
    
    /// <summary>
    /// 执行免罚卡
    /// </summary>
    private void ExecuteImmunityCard(Player player)
    {
        player.hasImmunity = true;
        UIManager.Instance.ShowMessage($"{player.nickname} 获得了免罚保护！");
    }
    
    /// <summary>
    /// 执行陷害卡
    /// </summary>
    private void ExecuteFrameCard(Player player)
    {
        UIManager.Instance.ShowTargetPlayerPanel((targetPlayer) =>
        {
            if (targetPlayer == null) return;
            
            targetPlayer.isInHospital = true;
            targetPlayer.skipTurns = 1;
            Economy.Instance.SubtractGold(targetPlayer, 200);
            UIManager.Instance.ShowMessage($"{targetPlayer.nickname} 被 {player.nickname} 陷害送入医院！");
        });
    }
    
    /// <summary>
    /// 执行拆迁卡
    /// </summary>
    private void ExecuteDemolishCard(Player player)
    {
        UIManager.Instance.ShowTargetPropertyPanel((property) =>
        {
            if (property == null) return;
            
            if (property.owner != player && property.level > 0)
            {
                int compensation = property.upgradeCost / 2;
                Economy.Instance.TransferGold(player, property.owner, compensation);
                property.level--;
                UIManager.Instance.ShowMessage($"{player.nickname} 拆除了 {property.name} 的一栋房屋！");
            }
            else
            {
                UIManager.Instance.ShowError("无法拆除该地产！");
            }
        });
    }
    
    /// <summary>
    /// 执行勒索卡
    /// </summary>
    private void ExecuteExtortCard(Player player)
    {
        UIManager.Instance.ShowTargetPlayerPanel((targetPlayer) =>
        {
            if (targetPlayer == null) return;
            
            int amount = Random.Range(100, 301);
            Economy.Instance.TransferGold(targetPlayer, player, amount);
            UIManager.Instance.ShowMessage($"{player.nickname} 勒索了 {targetPlayer.nickname} {amount} 金币！");
        });
    }
    
    /// <summary>
    /// 执行出院卡
    /// </summary>
    private void ExecuteLeaveHospitalCard(Player player)
    {
        if (player.isInHospital)
        {
            player.isInHospital = false;
            player.skipTurns = 0;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用出院卡离开了医院！");
        }
        else
        {
            UIManager.Instance.ShowError("你不在医院！");
        }
    }
    
    /// <summary>
    /// 执行保释卡
    /// </summary>
    private void ExecuteLeavePoliceCard(Player player)
    {
        if (player.isInPoliceStation)
        {
            player.isInPoliceStation = false;
            player.skipTurns = 0;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用保释卡离开了警局！");
        }
        else
        {
            UIManager.Instance.ShowError("你不在警局！");
        }
    }
}

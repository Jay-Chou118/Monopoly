using UnityEngine;
using System.Collections.Generic;

public enum CardType
{
    FixedDice,
    DoubleRent,
    Immunity,
    Frame,
    Demolish,
    Extort,
    LeaveHospital,
    LeavePolice
}

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

public class CardSystem : MonoBehaviour
{
    private List<Card> allCards = new List<Card>();
    private List<Card> chanceCards = new List<Card>();
    
    public void Initialize()
    {
        CreateCards();
        CreateChanceCards();
    }
    
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
    
    public Card GetRandomCard()
    {
        if (allCards.Count == 0) return null;
        return allCards[Random.Range(0, allCards.Count)];
    }
    
    public Card GetRandomChanceCard()
    {
        if (chanceCards.Count == 0) return null;
        return chanceCards[Random.Range(0, chanceCards.Count)];
    }
    
    public bool GiveRandomCard(Player player)
    {
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
    
    public void ExecuteCard(Card card, Player player)
    {
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
    
    private void ExecuteFixedDiceCard(Player player)
    {
        UIManager.Instance.ShowFixedDicePanel();
    }
    
    private void ExecuteDoubleRentCard(Player player)
    {
        player.hasDoubleRent = true;
        UIManager.Instance.ShowMessage($"{player.nickname} 启用了双倍租金卡！");
    }
    
    private void ExecuteImmunityCard(Player player)
    {
        player.hasImmunity = true;
        UIManager.Instance.ShowMessage($"{player.nickname} 获得了免罚保护！");
    }
    
    private void ExecuteFrameCard(Player player)
    {
        UIManager.Instance.ShowTargetPlayerPanel((targetPlayer) =>
        {
            targetPlayer.isInHospital = true;
            targetPlayer.skipTurns = 1;
            Economy.Instance.SubtractGold(targetPlayer, 200);
            UIManager.Instance.ShowMessage($"{targetPlayer.nickname} 被 {player.nickname} 陷害送入医院！");
        });
    }
    
    private void ExecuteDemolishCard(Player player)
    {
        UIManager.Instance.ShowTargetPropertyPanel((property) =>
        {
            if (property.owner != player && property.level >0)
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
    
    private void ExecuteExtortCard(Player player)
    {
        UIManager.Instance.ShowTargetPlayerPanel((targetPlayer) =>
        {
            int amount = Random.Range(100, 301);
            Economy.Instance.TransferGold(targetPlayer, player, amount);
            UIManager.Instance.ShowMessage($"{player.nickname} 勒索了 {targetPlayer.nickname} {amount} 金币！");
        });
    }
    
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

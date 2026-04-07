using UnityEngine;

/// <summary>
/// 玩家类 - 管理玩家数据和行为
/// </summary>
public class Player
{
    /// <summary>玩家昵称</summary>
    public string nickname;
    /// <summary>金币数量</summary>
    public int gold;
    /// <summary>当前位置</summary>
    public int position;
    /// <summary>是否破产</summary>
    public bool isBankrupt;
    /// <summary>是否在医院</summary>
    public bool isInHospital;
    /// <summary>是否在警局</summary>
    public bool isInPoliceStation;
    /// <summary>跳过回合数</summary>
    public int skipTurns;
    /// <summary>是否拥有双倍租金效果</summary>
    public bool hasDoubleRent;
    /// <summary>是否拥有免罚效果</summary>
    public bool hasImmunity;
    
    /// <summary>拥有的地产列表</summary>
    public List<PropertyCell> properties = new List<PropertyCell>();
    /// <summary>拥有的道具卡列表</summary>
    public List<Card> cards = new List<Card>();
    
    /// <summary>道具卡持有上限</summary>
    public const int MAX_CARDS = 6;
    /// <summary>初始金币</summary>
    public const int INITIAL_GOLD = 3000;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="nickname">玩家昵称</param>
    public Player(string nickname)
    {
        this.nickname = nickname;
        ResetPlayer();
    }
    
    /// <summary>
    /// 重置玩家状态
    /// </summary>
    public void ResetPlayer()
    {
        gold = INITIAL_GOLD;
        position = 0;
        isBankrupt = false;
        isInHospital = false;
        isInPoliceStation = false;
        skipTurns = 0;
        hasDoubleRent = false;
        hasImmunity = false;
        
        properties.Clear();
        cards.Clear();
    }
    
    /// <summary>
    /// 检查是否拥有指定类型的卡牌
    /// </summary>
    public bool HasCard(CardType type)
    {
        return cards.Exists(card => card.type == type);
    }
    
    /// <summary>
    /// 获取指定类型的卡牌
    /// </summary>
    public Card GetCard(CardType type)
    {
        return cards.Find(card => card.type == type);
    }
    
    /// <summary>
    /// 添加卡牌
    /// </summary>
    /// <returns>是否添加成功</returns>
    public bool AddCard(Card card)
    {
        if (cards.Count >= MAX_CARDS)
        {
            return false;
        }
        
        cards.Add(card);
        return true;
    }
    
    /// <summary>
    /// 移除卡牌
    /// </summary>
    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }
    
    /// <summary>
    /// 移除指定类型的卡牌
    /// </summary>
    public void RemoveCard(CardType type)
    {
        Card cardToRemove = cards.Find(card => card.type == type);
        if (cardToRemove != null)
        {
            cards.Remove(cardToRemove);
        }
    }
    
    /// <summary>
    /// 检查是否能够支付指定金额
    /// </summary>
    public bool CanAfford(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"CanAfford called with negative amount: {amount}");
            return false;
        }
        return gold >= amount;
    }
    
    /// <summary>
    /// 增加金币
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"AddGold called with negative amount: {amount}");
            return;
        }
        gold += amount;
    }
    
    /// <summary>
    /// 减少金币
    /// </summary>
    public void SubtractGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"SubtractGold called with negative amount: {amount}");
            return;
        }
        gold -= amount;
    }
    
    /// <summary>
    /// 获取地产数量
    /// </summary>
    public int GetPropertyCount()
    {
        return properties.Count;
    }
    
    /// <summary>
    /// 获取卡牌数量
    /// </summary>
    public int GetCardCount()
    {
        return cards.Count;
    }
    
    /// <summary>
    /// 检查是否还有卡牌槽位
    /// </summary>
    public bool HasFreeSlots()
    {
        return cards.Count < MAX_CARDS;
    }
}

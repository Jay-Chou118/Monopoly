using UnityEngine;

/// <summary>
/// 经济系统 - 负责金币管理、地产价值计算、抵押赎回等
/// </summary>
public class Economy : MonoBehaviour
{
    public static Economy Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化经济系统
    /// </summary>
    public void Initialize()
    {
        Debug.Log("经济系统初始化完成");
    }

    /// <summary>
    /// 检查玩家是否有足够的金币
    /// </summary>
    public bool HasEnoughGold(Player player, int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"HasEnoughGold called with negative amount: {amount}");
            return false;
        }
        return player.gold >= amount;
    }

    /// <summary>
    /// 增加玩家金币
    /// </summary>
    public void AddGold(Player player, int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"AddGold called with negative amount: {amount}");
            return;
        }
        player.gold += amount;
        UIManager.Instance.UpdatePlayerGold(player);
    }

    /// <summary>
    /// 减少玩家金币
    /// </summary>
    public void SubtractGold(Player player, int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"SubtractGold called with negative amount: {amount}");
            return;
        }
        player.gold -= amount;
        UIManager.Instance.UpdatePlayerGold(player);
    }

    /// <summary>
    /// 转移金币（从一个玩家到另一个玩家）
    /// </summary>
    public void TransferGold(Player fromPlayer, Player toPlayer, int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"TransferGold called with negative amount: {amount}");
            return;
        }
        
        if (HasEnoughGold(fromPlayer, amount))
        {
            SubtractGold(fromPlayer, amount);
            AddGold(toPlayer, amount);
            UIManager.Instance.ShowMessage($"{fromPlayer.nickname} 向 {toPlayer.nickname} 支付了 {amount} 金币！");
        }
        else
        {
            // 金币不足，需要处理抵押或破产
            int deficit = amount - fromPlayer.gold;
            UIManager.Instance.ShowMessage($"{fromPlayer.nickname} 金币不足 {amount}，差 {deficit} 金币！");
            
            // TODO: 弹出抵押面板让玩家选择抵押地产
            // UIManager.Instance.ShowMortgagePanel(fromPlayer, deficit, () => {
            //     TransferGold(fromPlayer, toPlayer, amount);
            // });
            
            // 临时处理：直接扣减，让金币变负
            SubtractGold(fromPlayer, amount);
            AddGold(toPlayer, amount);
            UIManager.Instance.ShowError($"{fromPlayer.nickname} 金币不足，已负债！");
        }
    }

    /// <summary>
    /// 计算地产租金
    /// </summary>
    public int CalculateRent(PropertyCell property)
    {
        if (property == null || property.owner == null)
        {
            return 0;
        }
        
        int baseRent = property.rent;
        int levelMultiplier = 1 + property.level;
        int rent = baseRent * levelMultiplier;
        
        // 应用角色过路费收入加成
        if (property.owner.character != null)
        {
            rent = Mathf.RoundToInt(rent * (1 + property.owner.character.rentIncomeBonus));
        }
        
        // 双倍租金效果
        if (property.owner.hasDoubleRent)
        {
            rent *= 2;
            property.owner.hasDoubleRent = false;
        }
        
        return rent;
    }

    /// <summary>
    /// 计算升级费用
    /// </summary>
    public int CalculateUpgradeCost(PropertyCell property)
    {
        if (property == null)
        {
            return 0;
        }
        
        int baseCost = property.upgradeCost * (property.level + 1);
        
        // 应用角色升级费用折扣
        if (property.owner != null && property.owner.character != null)
        {
            baseCost = Mathf.RoundToInt(baseCost * (1 + property.owner.character.upgradeCostDiscount));
        }
        
        return baseCost;
    }

    /// <summary>
    /// 计算地产购买价格（考虑角色折扣）
    /// </summary>
    public int CalculateBuyPrice(PropertyCell property, Player buyer)
    {
        if (property == null)
        {
            return 0;
        }
        
        int basePrice = property.price;
        
        // 应用角色购买折扣
        if (buyer != null && buyer.character != null)
        {
            basePrice = Mathf.RoundToInt(basePrice * (1 + buyer.character.propertyBuyDiscount));
        }
        
        return basePrice;
    }

    /// <summary>
    /// 计算地产总价值（购买价格 + 升级价值）
    /// </summary>
    public int CalculatePropertyValue(PropertyCell property)
    {
        if (property == null)
        {
            return 0;
        }
        int baseValue = property.price;
        int upgradeValue = property.upgradeCost * property.level;
        return baseValue + upgradeValue;
    }

    /// <summary>
    /// 计算抵押价值
    /// </summary>
    public int CalculateMortgageValue(PropertyCell property)
    {
        return CalculatePropertyValue(property) / 2;
    }

    /// <summary>
    /// 计算赎回费用
    /// </summary>
    public int CalculateRedeemCost(PropertyCell property)
    {
        return CalculateMortgageValue(property) + 10;
    }

    /// <summary>
    /// 抵押地产
    /// </summary>
    public void MortgageProperty(PropertyCell property)
    {
        if (property == null)
        {
            Debug.LogWarning("MortgageProperty called with null property");
            return;
        }
        
        if (property.owner != null && !property.isMortgaged)
        {
            int mortgageValue = CalculateMortgageValue(property);
            AddGold(property.owner, mortgageValue);
            property.isMortgaged = true;
            UIManager.Instance.ShowMessage($"{property.owner.nickname} 抵押了 {property.name}，获得 {mortgageValue} 金币！");
        }
        else
        {
            UIManager.Instance.ShowError("无法抵押该地产！");
        }
    }

    /// <summary>
    /// 赎回地产
    /// </summary>
    public void RedeemProperty(PropertyCell property)
    {
        if (property == null)
        {
            Debug.LogWarning("RedeemProperty called with null property");
            return;
        }
        
        if (property.owner != null && property.isMortgaged)
        {
            int redeemCost = CalculateRedeemCost(property);
            if (HasEnoughGold(property.owner, redeemCost))
            {
                SubtractGold(property.owner, redeemCost);
                property.isMortgaged = false;
                UIManager.Instance.ShowMessage($"{property.owner.nickname} 赎回了 {property.name}！");
            }
            else
            {
                UIManager.Instance.ShowError("金币不足！");
            }
        }
        else
        {
            UIManager.Instance.ShowError("无法赎回该地产！");
        }
    }

    /// <summary>
    /// 计算玩家总财富（金币 + 地产价值）
    /// </summary>
    public int CalculateTotalWealth(Player player)
    {
        if (player == null)
        {
            return 0;
        }
        
        int total = player.gold;
        
        foreach (PropertyCell property in player.properties)
        {
            if (!property.isMortgaged)
            {
                total += CalculatePropertyValue(property);
            }
            else
            {
                total -= CalculateRedeemCost(property);
            }
        }
        
        return total;
    }
}

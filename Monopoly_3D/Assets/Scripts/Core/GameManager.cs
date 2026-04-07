using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏全局管理器 - 负责游戏流程控制、回合管理、胜负判定
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    /// <summary>玩家列表</summary>
    public List<Player> players = new List<Player>();
    /// <summary>当前玩家索引</summary>
    public int currentPlayerIndex = 0;
    /// <summary>游戏是否结束</summary>
    public bool isGameOver = false;

    /// <summary>地图引用</summary>
    public Map map;
    /// <summary>卡牌系统引用</summary>
    public CardSystem cardSystem;
    /// <summary>经济系统引用</summary>
    public Economy economy;
    /// <summary>可选角色列表</summary>
    public CharacterData[] availableCharacters;

    // 常量定义
    private const int DICE_MIN = 1;
    private const int DICE_MAX = 7;  // Random.Range 是左闭右开，所以用 7 得到 1-6
    private const int PASS_START_REWARD = 500;  // 经过起点奖励

    /// <summary>玩家信息变化事件</summary>
    public event System.Action<Player> OnPlayerInfoChanged;

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

    private void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        map.Initialize();
        cardSystem.Initialize();
        economy.Initialize();
        
        StartGame();
    }

    /// <summary>
    /// 创建新玩家
    /// </summary>
    /// <param name="nickname">玩家昵称</param>
    /// <param name="character">选择的角色</param>
    /// <returns>创建的玩家</returns>
    public Player CreatePlayer(string nickname, CharacterData character)
    {
        Player player = new Player(nickname, character);
        players.Add(player);
        
        // 应用角色初始额外道具卡
        if (character != null && character.extraCards > 0)
        {
            for (int i = 0; i< character.extraCards; i++)
            {
                cardSystem.GiveRandomCard(player);
            }
        }
        
        return player;
    }
    
    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartGame()
    {
        currentPlayerIndex = 0;
        isGameOver = false;
        
        foreach (Player player in players)
        {
            player.ResetPlayer();
            
            // 重新应用角色初始额外道具卡
            if (player.character != null && player.character.extraCards >0)
            {
                for (int i = 0; i< player.character.extraCards; i++)
                {
                    cardSystem.GiveRandomCard(player);
                }
            }
        }
        
        UpdateCurrentPlayer();
    }

    /// <summary>
    /// 掷骰子
    /// </summary>
    public void RollDice()
    {
        if (!IsCurrentPlayerTurn() || isGameOver) return;

        Player currentPlayer = players[currentPlayerIndex];
        int minValue = DICE_MIN;
        
        // 应用角色骰子最小值限制
        if (currentPlayer.character != null && currentPlayer.character.diceMinValue > minValue)
        {
            minValue = currentPlayer.character.diceMinValue;
        }

        int diceResult = Random.Range(minValue, DICE_MAX);
        MovePlayer(diceResult);
    }

    /// <summary>
    /// 移动玩家
    /// </summary>
    /// <param name="steps">骰子点数</param>
    public void MovePlayer(int steps)
    {
        Player currentPlayer = players[currentPlayerIndex];
        int oldPosition = currentPlayer.position;
        int newPosition = (currentPlayer.position + steps) % map.totalCells;
        
        // 检查是否经过起点（绕圈）
        if (oldPosition + steps >= map.totalCells)
        {
            int reward = PASS_START_REWARD;
            
            // 应用角色起点奖励加成
            if (currentPlayer.character != null)
            {
                reward += currentPlayer.character.startBonusGold;
            }
            
            economy.AddGold(currentPlayer, reward);
            UIManager.Instance.ShowMessage($"{currentPlayer.nickname} 经过起点，获得 {reward} 金币！");
        }
        
        currentPlayer.position = newPosition;
        TriggerCellEvent(newPosition);
        NotifyPlayerInfoChanged(currentPlayer);
    }

    /// <summary>
    /// 触发格子事件
    /// </summary>
    /// <param name="cellIndex">格子索引</param>
    private void TriggerCellEvent(int cellIndex)
    {
        Cell cell = map.GetCell(cellIndex);
        switch (cell.type)
        {
            case CellType.Start:
                break;
            case CellType.Property:
                HandlePropertyCell(cell as PropertyCell);
                break;
            case CellType.Special:
                HandleSpecialCell(cell as SpecialCell);
                break;
            case CellType.Card:
                HandleCardCell();
                break;
        }
    }

    /// <summary>
    /// 处理地产格子
    /// </summary>
    private void HandlePropertyCell(PropertyCell property)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        if (property.owner == null)
        {
            UIManager.Instance.ShowBuyPropertyPanel(property);
        }
        else if (property.owner != currentPlayer)
        {
            int rent = economy.CalculateRent(property);
            economy.TransferGold(currentPlayer, property.owner, rent);
        }
    }

    /// <summary>
    /// 处理特殊场所格子
    /// </summary>
    private void HandleSpecialCell(SpecialCell specialCell)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        switch (specialCell.specialType)
        {
            case SpecialType.Hospital:
                SendToHospital(currentPlayer);
                break;
            case SpecialType.PoliceStation:
                SendToPoliceStation(currentPlayer);
                break;
            case SpecialType.Bank:
                UIManager.Instance.ShowBankPanel();
                break;
            case SpecialType.TaxOffice:
                PayTax(currentPlayer);
                break;
            case SpecialType.Park:
                EndTurn();
                break;
            case SpecialType.Chance:
                DrawChanceCard();
                break;
        }
    }

    /// <summary>
    /// 处理道具卡格子
    /// </summary>
    private void HandleCardCell()
    {
        cardSystem.GiveRandomCard(players[currentPlayerIndex]);
    }

    /// <summary>
    /// 送入医院
    /// </summary>
    private void SendToHospital(Player player)
    {
        if (player.hasImmunity)
        {
            player.hasImmunity = false;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用免罚卡避免了被送入医院！");
        }
        else if (player.character != null && player.character.freeHospitalPolice)
        {
            player.isInHospital = true;
            player.skipTurns = 1;
            UIManager.Instance.ShowMessage($"{player.nickname} 被送入医院，但因律师身份免交罚款！");
        }
        else
        {
            player.isInHospital = true;
            player.skipTurns = 1;
            
            int fine = 200;
            // 应用角色额外罚款
            if (player.character != null)
            {
                fine += player.character.extraHospitalPoliceFine;
            }
            
            economy.SubtractGold(player, fine);
            UIManager.Instance.ShowMessage($"{player.nickname} 被送入医院，支付 {fine} 金币！");
        }
    }

    /// <summary>
    /// 送入警局
    /// </summary>
    private void SendToPoliceStation(Player player)
    {
        if (player.hasImmunity)
        {
            player.hasImmunity = false;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用免罚卡避免了被送入警局！");
        }
        else if (player.character != null && player.character.freeHospitalPolice)
        {
            player.isInPoliceStation = true;
            player.skipTurns = 1;
            UIManager.Instance.ShowPoliceStationPanel(player);
            UIManager.Instance.ShowMessage($"{player.nickname} 被送入警局，但因律师身份免交保释金！");
        }
        else
        {
            player.isInPoliceStation = true;
            player.skipTurns = 1;
            UIManager.Instance.ShowPoliceStationPanel(player);
        }
    }

    /// <summary>
    /// 支付税款
    /// </summary>
    private void PayTax(Player player)
    {
        if (player.hasImmunity)
        {
            player.hasImmunity = false;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用免罚卡豁免了税款！");
        }
        else
        {
            int tax = (int)(player.gold * 0.1f);
            economy.SubtractGold(player, tax);
            UIManager.Instance.ShowMessage($"{player.nickname} 支付了 {tax} 金币的税款！");
        }
    }

    /// <summary>
    /// 抽取机会卡
    /// </summary>
    private void DrawChanceCard()
    {
        Card card = cardSystem.GetRandomChanceCard();
        cardSystem.ExecuteCard(card, players[currentPlayerIndex]);
    }

    /// <summary>
    /// 购买地产
    /// </summary>
    public void BuyProperty(PropertyCell property)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        // 计算考虑角色折扣后的购买价格
        int buyPrice = economy.CalculateBuyPrice(property, currentPlayer);
        
        if (economy.HasEnoughGold(currentPlayer, buyPrice))
        {
            economy.SubtractGold(currentPlayer, buyPrice);
            property.owner = currentPlayer;
            currentPlayer.properties.Add(property);
            
            UIManager.Instance.ShowMessage($"{currentPlayer.nickname} 购买了 {property.name}，花费 {buyPrice} 金币！");
            NotifyPlayerInfoChanged(currentPlayer);
        }
        else
        {
            UIManager.Instance.ShowError($"金币不足！需要 {buyPrice} 金币");
        }
    }

    /// <summary>
    /// 升级地产
    /// </summary>
    public void UpgradeProperty(PropertyCell property)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        if (property.owner == currentPlayer && property.level < 3)
        {
            int upgradeCost = economy.CalculateUpgradeCost(property);
            
            if (economy.HasEnoughGold(currentPlayer, upgradeCost))
            {
                economy.SubtractGold(currentPlayer, upgradeCost);
                property.level++;
                
                UIManager.Instance.ShowMessage($"{currentPlayer.nickname} 升级了 {property.name}！");
                NotifyPlayerInfoChanged(currentPlayer);
            }
            else
            {
                UIManager.Instance.ShowError("金币不足！");
            }
        }
    }

    /// <summary>
    /// 结束当前回合
    /// </summary>
    public void EndTurn()
    {
        if (!IsCurrentPlayerTurn() || isGameOver) return;

        Player currentPlayer = players[currentPlayerIndex];
        
        // 处理跳过回合
        if (currentPlayer.skipTurns > 0)
        {
            currentPlayer.skipTurns--;
            if (currentPlayer.skipTurns == 0)
            {
                currentPlayer.isInHospital = false;
                currentPlayer.isInPoliceStation = false;
                UIManager.Instance.ShowMessage($"{currentPlayer.nickname} 解除了囚禁状态！");
            }
            NextPlayer();
            return;
        }
        
        CheckBankruptcy(currentPlayer);
        NextPlayer();
    }

    /// <summary>
    /// 切换到下一位玩家
    /// </summary>
    private void NextPlayer()
    {
        int startIndex = currentPlayerIndex;
        
        do
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            
            // 防止无限循环
            if (currentPlayerIndex == startIndex)
            {
                CheckGameOver();
                return;
            }
        } while (players[currentPlayerIndex].isBankrupt);
        
        UpdateCurrentPlayer();
    }

    /// <summary>
    /// 更新当前玩家 UI
    /// </summary>
    private void UpdateCurrentPlayer()
    {
        UIManager.Instance.UpdatePlayerPanel(players[currentPlayerIndex]);
        NotifyPlayerInfoChanged(players[currentPlayerIndex]);
    }

    /// <summary>
    /// 检查是否是当前玩家的回合
    /// </summary>
    private bool IsCurrentPlayerTurn()
    {
        Player currentPlayer = players[currentPlayerIndex];
        return !currentPlayer.isBankrupt && !currentPlayer.isInHospital && !currentPlayer.isInPoliceStation;
    }

    /// <summary>
    /// 检查玩家破产
    /// </summary>
    private void CheckBankruptcy(Player player)
    {
        if (player.gold < 0 && player.properties.Count == 0)
        {
            player.isBankrupt = true;
            UIManager.Instance.ShowMessage($"{player.nickname} 破产了！");
            
            CheckGameOver();
        }
    }

    /// <summary>
    /// 检查游戏是否结束
    /// </summary>
    private bool CheckGameOver()
    {
        int activePlayers = 0;
        Player winner = null;
        
        foreach (Player player in players)
        {
            if (!player.isBankrupt)
            {
                activePlayers++;
                winner = player;
            }
        }
        
        if (activePlayers <= 1)
        {
            isGameOver = true;
            UIManager.Instance.ShowGameOverPanel(winner);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    /// <summary>
    /// 移除玩家
    /// </summary>
    public void RemovePlayer(Player player)
    {
        players.Remove(player);
    }

    /// <summary>
    /// 通知玩家信息变化
    /// </summary>
    private void NotifyPlayerInfoChanged(Player player)
    {
        OnPlayerInfoChanged?.Invoke(player);
    }
}

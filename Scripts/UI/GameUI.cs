using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏 UI - 负责游戏主界面的交互
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("按钮")]
    public Button rollDiceButton;
    public Button endTurnButton;
    
    [Header("文本显示")]
    public Text currentPlayerText;
    public Text currentPositionText;
    public Text goldText;
    public Text propertyCountText;
    public Text cardCountText;

    private bool hasRolledDice = false;

    private void Start()
    {
        SetupButtonListeners();
        
        // 监听玩家信息变化事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerInfoChanged += OnPlayerInfoChanged;
        }
    }

    private void OnDestroy()
    {
        // 移除事件监听
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerInfoChanged -= OnPlayerInfoChanged;
        }
    }

    /// <summary>
    /// 设置按钮监听器
    /// </summary>
    private void SetupButtonListeners()
    {
        if (rollDiceButton != null)
        {
            rollDiceButton.onClick.AddListener(OnRollDiceClicked);
        }

        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
    }

    /// <summary>
    /// 掷骰子按钮点击
    /// </summary>
    private void OnRollDiceClicked()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGameOver) return;
        
        GameManager.Instance.RollDice();
        hasRolledDice = true;
        
        if (rollDiceButton != null)
        {
            rollDiceButton.interactable = false;
        }
    }

    /// <summary>
    /// 结束回合按钮点击
    /// </summary>
    private void OnEndTurnClicked()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGameOver) return;
        
        GameManager.Instance.EndTurn();
        hasRolledDice = false;
        
        if (rollDiceButton != null)
        {
            rollDiceButton.interactable = true;
        }
    }

    /// <summary>
    /// 玩家信息变化回调
    /// </summary>
    private void OnPlayerInfoChanged(Player player)
    {
        UpdatePlayerInfo(player);
    }

    /// <summary>
    /// 更新玩家信息显示
    /// </summary>
    private void UpdatePlayerInfo(Player player)
    {
        if (player == null || GameManager.Instance == null) return;
        
        Cell currentCell = GameManager.Instance.map.GetCell(player.position);
        
        if (currentPlayerText != null)
        {
            currentPlayerText.text = $"当前玩家：{player.nickname}";
        }
        
        if (currentPositionText != null && currentCell != null)
        {
            currentPositionText.text = $"位置：{currentCell.name}";
        }
        
        if (goldText != null)
        {
            goldText.text = $"金币：{player.gold}";
        }
        
        if (propertyCountText != null)
        {
            propertyCountText.text = $"地产：{player.GetPropertyCount()}";
        }
        
        if (cardCountText != null)
        {
            cardCountText.text = $"道具卡：{player.GetCardCount()}/{Player.MAX_CARDS}";
        }

        // 更新按钮状态
        UpdateButtonState(player);
    }

    /// <summary>
    /// 更新按钮状态
    /// </summary>
    private void UpdateButtonState(Player player)
    {
        if (GameManager.Instance == null) return;
        
        bool isCurrentPlayerTurn = !player.isBankrupt && !player.isInHospital && !player.isInPoliceStation;
        bool canInteract = isCurrentPlayerTurn && !GameManager.Instance.isGameOver;
        
        if (rollDiceButton != null)
        {
            rollDiceButton.interactable = canInteract && !hasRolledDice;
        }
        
        if (endTurnButton != null)
        {
            endTurnButton.interactable = canInteract;
        }
    }

    /// <summary>
    /// 重置 UI 状态（新回合开始时调用）
    /// </summary>
    public void ResetUIState()
    {
        hasRolledDice = false;
        
        if (rollDiceButton != null)
        {
            rollDiceButton.interactable = true;
        }
    }
}

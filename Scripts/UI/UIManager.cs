using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// UI管理器 - 负责所有UI面板的显示和隐藏
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("消息面板")]
    public Text messageText;
    public GameObject messagePanel;
    
    [Header("错误面板")]
    public Text errorText;
    public GameObject errorPanel;
    
    [Header("购买地产面板")]
    public GameObject buyPropertyPanel;
    public Text buyPropertyNameText;
    public Text buyPropertyPriceText;
    public Button buyButton;
    public Button cancelButton;
    
    [Header("银行面板")]
    public GameObject bankPanel;
    public Button mortgageButton;
    public Button redeemButton;
    
    [Header("警局面板")]
    public GameObject policeStationPanel;
    public Button payBailButton;
    public Button stayButton;
    
    [Header("定点骰子面板")]
    public GameObject fixedDicePanel;
    public Button[] diceButtons;
    
    [Header("游戏结束面板")]
    public GameObject gameOverPanel;
    public Text gameOverWinnerText;
    
    [Header("目标玩家选择面板")]
    public GameObject targetPlayerPanel;
    
    [Header("目标地产选择面板")]
    public GameObject targetPropertyPanel;

    // 玩家信息显示组件
    [Header("玩家信息")]
    public Text currentPlayerText;
    public Text goldText;
    public Text positionText;
    public Text propertyCountText;
    public Text cardCountText;

    private PropertyCell currentProperty;
    private Player currentPlayer;
    private Action<Player> targetPlayerCallback;
    private Action<PropertyCell> targetPropertyCallback;

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
        HideAllPanels();
        SetupButtonListeners();
    }

    /// <summary>
    /// 设置按钮监听器
    /// </summary>
    private void SetupButtonListeners()
    {
        // 购买地产按钮
        buyButton.onClick.AddListener(() =>
        {
            GameManager.Instance.BuyProperty(currentProperty);
            HideBuyPropertyPanel();
        });
        
        cancelButton.onClick.AddListener(HideBuyPropertyPanel);
        
        // 警局按钮
        payBailButton.onClick.AddListener(() =>
        {
            Economy.Instance.SubtractGold(currentPlayer, 300);
            currentPlayer.isInPoliceStation = false;
            currentPlayer.skipTurns = 0;
            HidePoliceStationPanel();
            GameManager.Instance.EndTurn();
        });
        
        stayButton.onClick.AddListener(() =>
        {
            HidePoliceStationPanel();
            GameManager.Instance.EndTurn();
        });
        
        // 定点骰子按钮
        for (int i = 0; i < diceButtons.Length; i++)
        {
            int diceValue = i + 1;
            diceButtons[i].onClick.AddListener(() =>
            {
                GameManager.Instance.MovePlayer(diceValue);
                HideFixedDicePanel();
            });
        }
    }

    /// <summary>
    /// 显示消息
    /// </summary>
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);
        StartCoroutine(HidePanelAfterDelay(messagePanel, 3f));
    }

    /// <summary>
    /// 显示错误
    /// </summary>
    public void ShowError(string error)
    {
        errorText.text = error;
        errorPanel.SetActive(true);
        StartCoroutine(HidePanelAfterDelay(errorPanel, 2f));
    }

    /// <summary>
    /// 延迟隐藏面板（协程）
    /// </summary>
    private IEnumerator HidePanelAfterDelay(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// 显示购买地产面板
    /// </summary>
    public void ShowBuyPropertyPanel(PropertyCell property)
    {
        currentProperty = property;
        buyPropertyNameText.text = property.name;
        buyPropertyPriceText.text = $"价格：{property.price} 金币";
        buyPropertyPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏购买地产面板
    /// </summary>
    public void HideBuyPropertyPanel()
    {
        buyPropertyPanel.SetActive(false);
    }

    /// <summary>
    /// 显示银行面板
    /// </summary>
    public void ShowBankPanel()
    {
        bankPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏银行面板
    /// </summary>
    public void HideBankPanel()
    {
        bankPanel.SetActive(false);
    }

    /// <summary>
    /// 显示警局面板
    /// </summary>
    public void ShowPoliceStationPanel(Player player)
    {
        currentPlayer = player;
        policeStationPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏警局面板
    /// </summary>
    public void HidePoliceStationPanel()
    {
        policeStationPanel.SetActive(false);
    }

    /// <summary>
    /// 显示定点骰子面板
    /// </summary>
    public void ShowFixedDicePanel()
    {
        fixedDicePanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏定点骰子面板
    /// </summary>
    public void HideFixedDicePanel()
    {
        fixedDicePanel.SetActive(false);
    }

    /// <summary>
    /// 显示游戏结束面板
    /// </summary>
    public void ShowGameOverPanel(Player winner)
    {
        if (winner != null)
        {
            gameOverWinnerText.text = $"{winner.nickname} 获胜！";
        }
        else
        {
            gameOverWinnerText.text = "游戏结束！";
        }
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏游戏结束面板
    /// </summary>
    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// 显示目标玩家选择面板
    /// </summary>
    public void ShowTargetPlayerPanel(Action<Player> callback)
    {
        targetPlayerCallback = callback;
        targetPlayerPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏目标玩家选择面板
    /// </summary>
    public void HideTargetPlayerPanel()
    {
        targetPlayerPanel.SetActive(false);
    }

    /// <summary>
    /// 选择目标玩家
    /// </summary>
    public void SelectTargetPlayer(Player player)
    {
        targetPlayerCallback?.Invoke(player);
        HideTargetPlayerPanel();
    }

    /// <summary>
    /// 显示目标地产选择面板
    /// </summary>
    public void ShowTargetPropertyPanel(Action<PropertyCell> callback)
    {
        targetPropertyCallback = callback;
        targetPropertyPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏目标地产选择面板
    /// </summary>
    public void HideTargetPropertyPanel()
    {
        targetPropertyPanel.SetActive(false);
    }

    /// <summary>
    /// 选择目标地产
    /// </summary>
    public void SelectTargetProperty(PropertyCell property)
    {
        targetPropertyCallback?.Invoke(property);
        HideTargetPropertyPanel();
    }

    /// <summary>
    /// 更新玩家面板信息
    /// </summary>
    public void UpdatePlayerPanel(Player player)
    {
        if (player == null) return;
        
        if (currentPlayerText != null)
        {
            currentPlayerText.text = $"当前玩家：{player.nickname}";
        }
        
        UpdatePlayerGold(player);
        
        if (positionText != null && GameManager.Instance != null)
        {
            Cell cell = GameManager.Instance.map.GetCell(player.position);
            if (cell != null)
            {
                positionText.text = $"位置：{cell.name}";
            }
        }
        
        if (propertyCountText != null)
        {
            propertyCountText.text = $"地产：{player.GetPropertyCount()}";
        }
        
        if (cardCountText != null)
        {
            cardCountText.text = $"道具卡：{player.GetCardCount()}/{Player.MAX_CARDS}";
        }
    }

    /// <summary>
    /// 更新玩家金币显示
    /// </summary>
    public void UpdatePlayerGold(Player player)
    {
        if (player == null || goldText == null) return;
        goldText.text = $"金币：{player.gold}";
    }

    /// <summary>
    /// 隐藏所有面板
    /// </summary>
    public void HideAllPanels()
    {
        if (messagePanel != null) messagePanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (buyPropertyPanel != null) buyPropertyPanel.SetActive(false);
        if (bankPanel != null) bankPanel.SetActive(false);
        if (policeStationPanel != null) policeStationPanel.SetActive(false);
        if (fixedDicePanel != null) fixedDicePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (targetPlayerPanel != null) targetPlayerPanel.SetActive(false);
        if (targetPropertyPanel != null) targetPropertyPanel.SetActive(false);
    }
}

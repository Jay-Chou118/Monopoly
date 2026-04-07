using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 弹窗管理器 - 负责创建和管理通用弹窗
/// </summary>
public class PopupManager : MonoBehaviour
{
    public GameObject popupPrefab;
    public Transform popupParent;

    /// <summary>
    /// 显示弹窗
    /// </summary>
    public void ShowPopup(string title, string message, Action onConfirm = null, Action onCancel = null)
    {
        GameObject popup = Instantiate(popupPrefab, popupParent);
        Popup popupComponent = popup.GetComponent<Popup>();
        
        if (popupComponent != null)
        {
            popupComponent.Setup(title, message, onConfirm, onCancel);
        }
    }

    /// <summary>
    /// 显示确认弹窗
    /// </summary>
    public void ShowConfirmPopup(string title, string message, Action onConfirm)
    {
        ShowPopup(title, message, onConfirm);
    }

    /// <summary>
    /// 显示信息弹窗
    /// </summary>
    public void ShowInfoPopup(string title, string message)
    {
        ShowPopup(title, message, () => { });
    }

    /// <summary>
    /// 显示错误弹窗
    /// </summary>
    public void ShowErrorPopup(string message)
    {
        ShowPopup("错误", message, () => { });
    }
}

/// <summary>
/// 弹窗组件
/// </summary>
public class Popup : MonoBehaviour
{
    public Text titleText;
    public Text messageText;
    public Button confirmButton;
    public Button cancelButton;

    private Action onConfirm;
    private Action onCancel;

    /// <summary>
    /// 设置弹窗内容
    /// </summary>
    public void Setup(string title, string message, Action onConfirm = null, Action onCancel = null)
    {
        titleText.text = title;
        messageText.text = message;
        
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;

        confirmButton.onClick.AddListener(OnConfirm);
        
        if (onCancel != null)
        {
            cancelButton.gameObject.SetActive(true);
            cancelButton.onClick.AddListener(OnCancel);
        }
        else
        {
            cancelButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 确认按钮点击
    /// </summary>
    private void OnConfirm()
    {
        onConfirm?.Invoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// 取消按钮点击
    /// </summary>
    private void OnCancel()
    {
        onCancel?.Invoke();
        Destroy(gameObject);
    }
}

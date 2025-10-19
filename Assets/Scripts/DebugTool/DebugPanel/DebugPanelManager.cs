using UnityEngine;
using System;
using System.Collections.Generic;

namespace DebugTool
{
    public partial class DebugPanelManager : SingletonBehaviour<DebugPanelManager>
    {
        // 根据屏幕分辨率更新自适应值（不涉及GUI）
        private void UpdateAdaptiveValues()
        {
            float scaleFactor = GetScaleFactor();

            panelWidth = basePanelWidth * scaleFactor;
            margin = baseMargin * scaleFactor;
            titleHeight = baseTitleHeight * scaleFactor;
            headerHeight = baseHeaderHeight * scaleFactor;
        }

        // 确保自适应字体样式已初始化（只在OnGUI中调用）
        private void EnsureAdaptiveStyles()
        {
            float scaleFactor = GetScaleFactor();
            int adaptiveFontSize = Mathf.RoundToInt(baseFontSize * scaleFactor);
            int adaptiveHeaderFontSize = Mathf.RoundToInt(baseHeaderFontSize * scaleFactor);

            // 初始化或更新Label样式
            if (adaptiveLabelStyle == null)
            {
                adaptiveLabelStyle = new GUIStyle(GUI.skin.label);
            }
            adaptiveLabelStyle.fontSize = adaptiveFontSize;
            adaptiveLabelStyle.normal.textColor = Color.white; // 设置文字为白色

            // Header样式（面板标题）
            if (adaptiveHeaderStyle == null)
            {
                adaptiveHeaderStyle = new GUIStyle(GUI.skin.button);
            }
            adaptiveHeaderStyle.fontSize = adaptiveHeaderFontSize;
            adaptiveHeaderStyle.normal.textColor = Color.white; // 设置Header文字为白色
        }

        private List<DebugGUIPanelItem> debugItems = new List<DebugGUIPanelItem>();
        private bool isExpanded = true; // 面板是否展开

        // 基准尺寸（基于1080p）
        private float basePanelWidth = 250f;
        private float baseMargin = 10f;
        private float baseTitleHeight = 25f;  // 调大一点适配更大字体
        private float baseHeaderHeight = 35f; // 调大一点适配更大字体

        // 当前自适应尺寸
        private float panelWidth;
        private float margin;
        private float titleHeight;
        private float headerHeight;

        // 基准分辨率（1080p）
        private const float baseScreenHeight = 1080f;

        // 基准字体大小
        private const int baseFontSize = 14;  // 调大一点
        private const int baseHeaderFontSize = 16;

        // 自适应GUIStyle
        private GUIStyle adaptiveLabelStyle;
        private GUIStyle adaptiveHeaderStyle;

        // 注册调试项
        public void RegisterDebugItem(DebugGUIPanelItem panelItem)
        {
            if (panelItem != null)
            {
                debugItems.Add(panelItem);
            }
        }

        // 移除调试项
        public void RemoveDebugItem(DebugGUIPanelItem panelItem)
        {
            if (panelItem != null)
            {
                debugItems.Remove(panelItem);
            }
        }

        // 清除所有调试项
        public void ClearAll()
        {
            debugItems.Clear();
        }

        // 便捷方法：添加标签
        public DebugPanelLabelItem AddLabel(Func<string> getValue)
        {
            var item = new DebugPanelLabelItem(getValue);
            RegisterDebugItem(item);
            return item;
        }

        // 便捷方法：添加滑动条
        public DebugPanelSliderItem AddSlider(string title, float min, float max, Action<float> setValue, Func<float> getValue)
        {
            var item = new DebugPanelSliderItem(title, min, max, setValue, getValue);
            RegisterDebugItem(item);
            return item;
        }

        // 便捷方法：添加按钮
        public DebugGUIPanelButtonItem AddButton(string title, Action onClick)
        {
            var item = new DebugGUIPanelButtonItem(title, onClick);
            RegisterDebugItem(item);
            return item;
        }

        // 便捷方法：添加间距
        public DebugPanelSpaceItem AddSpace(float height = 20f)
        {
            var item = new DebugPanelSpaceItem(height);
            RegisterDebugItem(item);
            return item;
        }

        // 控制面板展开/收缩状态
        public void SetPanelExpanded(bool expanded)
        {
            isExpanded = expanded;
        }

        // 获取面板展开状态
        public bool IsPanelExpanded()
        {
            return isExpanded;
        }

        // 切换面板展开状态
        public void TogglePanelExpansion()
        {
            isExpanded = !isExpanded;
        }

        private void OnGUI()
        {
            if (debugItems.Count == 0) return;

            // 更新自适应值和样式
            UpdateAdaptiveValues();
            EnsureAdaptiveStyles();

            float currentY = margin;
            float panelHeight = CalculatePanelHeight();
            float panelX = Screen.width - panelWidth - margin;

            // 绘制背景面板
            GUI.Box(new Rect(panelX, margin, panelWidth, panelHeight), "");

            // 绘制标题按钮（可点击折叠/展开）
            Rect headerRect = new Rect(panelX + 5f, margin + 5f, panelWidth - 10f, headerHeight);

            // 设置按钮样式（比正常按钮暗一点）
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.gray;

            string headerText = isExpanded ? "Debug Panel [−]" : "Debug Panel [+]";
            GUIStyle headerStyle = adaptiveHeaderStyle ?? GUI.skin.button;
            if (GUI.Button(headerRect, headerText, headerStyle))
            {
                isExpanded = !isExpanded;
            }

            GUI.backgroundColor = originalColor;

            // 只有在展开状态下才绘制内容
            if (isExpanded)
            {
                currentY += margin + headerHeight + 10f; // 标题按钮空间

                foreach (var item in debugItems)
                {
                    currentY += DrawDebugItem(item, currentY);
                }
            }
        }

        private float CalculatePanelHeight()
        {
            float height = margin + headerHeight + 10f; // 开始边距 + 标题按钮高度 + 间距

            // 只有在展开状态下才计算内容高度
            if (isExpanded)
            {
                foreach (var item in debugItems)
                {
                    height += GetItemTotalHeight(item);
                }
            }

            height += margin; // 底部边距
            return height;
        }

        private float GetItemTotalHeight(DebugGUIPanelItem panelItem)
        {
            float height = 0f;

            // 如果是Label，在前面添加额外的上边距
            if (panelItem is DebugPanelLabelItem)
            {
                height += margin * 0.5f; // Label前面添加额外间距，离上面远一点
            }

            // 标题高度（如果有标题）
            // if (!string.IsNullOrEmpty(item.Title))
            // {
            //     height += titleHeight;
            // }

            // 项目高度
            height += panelItem.GetRequiredHeight();

            // 间距 - 为Label类型使用更小的间距
            if (panelItem is DebugPanelLabelItem)
            {
                height += margin * 0.2f; // Label使用很小的间距，与下个元素挨近
            }
            else
            {
                height += margin;
            }

            return height;
        }

        private float DrawDebugItem(DebugGUIPanelItem panelItem, float startY)
        {
            float currentY = startY;
            float panelX = Screen.width - panelWidth - margin;
            float itemX = panelX + margin; // 面板左边缘 + 左边距
            float itemWidth = panelWidth - margin * 2; // 面板宽度 - 左右边距

            // 如果是Label，在前面添加额外的上边距
            if (panelItem is DebugPanelLabelItem)
            {
                currentY += margin * 0.5f; // Label前面添加额外间距，离上面远一点
            }

            // 绘制标题（如果存在）
            // if (!string.IsNullOrEmpty(item.Title))
            // {
            //     GUIStyle titleStyle = adaptiveLabelStyle ?? GUI.skin.label;
            //     GUI.Label(new Rect(itemX, currentY, itemWidth, titleHeight), item.Title, titleStyle);
            //     currentY += titleHeight;
            // }

            // 绘制项目内容
            Rect contentRect = new Rect(itemX, currentY, itemWidth, panelItem.GetRequiredHeight());
            // 确保样式已经准备好
            GUIStyle labelStyle = adaptiveLabelStyle ?? GUI.skin.label;
            panelItem.Draw(contentRect, labelStyle, null); // Button使用默认样式，传null
            currentY += panelItem.GetRequiredHeight();

            // 添加间距 - 为Label类型使用更小的间距
            if (panelItem is DebugPanelLabelItem)
            {
                currentY += margin * 0.2f; // Label使用很小的间距，与下个元素挨近
            }
            else
            {
                currentY += margin;
            }

            return currentY - startY;
        }
    }
}

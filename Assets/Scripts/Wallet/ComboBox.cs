using Poltergeist;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboBox
{
    private bool isClickedComboButton = false;
    private int selectedItemIndex = 0;

    private string buttonStyle;
    private string boxStyle;

    private static GUIStyle listStyle;
    private static string listStyleUiTheme;

    public Vector2 ListScroll = Vector2.zero;

    public ComboBox() : this("button", "box")
    {
    }

    public ComboBox(string buttonStyle, string boxStyle)
    {
        this.buttonStyle = buttonStyle;
        this.boxStyle = boxStyle;
    }

    public int Show<T>(Rect rect, IList<T> listContent, int maxAvailableHeight, out int height, string caption = null, int offset = 0)
    {
        return Show(rect, listContent.Select(x => new GUIContent(x.ToString())).ToArray(), maxAvailableHeight, out height, caption, offset);
    }

    public int Show(Rect rect, IList<GUIContent> listContent, int maxAvailableHeight, out int height, string caption = null, int offset = 0)
    {
        if (listStyle == null || listStyleUiTheme != AccountManager.Instance.Settings.uiThemeName)
        {
            listStyle = GUI.skin.customStyles[0];
            listStyleUiTheme = AccountManager.Instance.Settings.uiThemeName;

            Color backgroundColor;
            if (AccountManager.Instance.Settings.uiThemeName == UiThemes.Phantasia.ToString())
            {
                if (!ColorUtility.TryParseHtmlString("#00508c", out backgroundColor))
                {
                    backgroundColor = new Color(0, 0, 0, 1);
                }
            }
            else
            {
                backgroundColor = new Color(0, 0, 0, 1);
            }
            var normalTex = ResourceManager.TextureFromColor(backgroundColor);
            var hoverTex = ResourceManager.TextureFromColor(Color.white);

            listStyle.normal.textColor = Color.white;
            listStyle.normal.background = normalTex;

            listStyle.onHover.background =
            listStyle.hover.background = hoverTex;

            listStyle.padding.left =
            listStyle.padding.right =
            listStyle.padding.top =
            listStyle.padding.bottom = WalletGUI.Units(1);
        }

        bool done = false;

        var buttonContent = caption == null ? listContent[selectedItemIndex] : new GUIContent(caption);
        if (GUI.Button(rect, buttonContent, buttonStyle))
        {
            if (isClickedComboButton)
                done = true;

            isClickedComboButton = true;
        }

        height = WalletGUI.Units(2);
        if (isClickedComboButton)
        {
            height += WalletGUI.Units(2) * (listContent.Count - offset);
            if (maxAvailableHeight > 0)
            {
                height = Math.Min(maxAvailableHeight, height);
            }

            var insideRect = new Rect(0, 0, rect.width, WalletGUI.Units(2) * (listContent.Count - offset));
            var outsideRect = new Rect(rect.x, rect.y + WalletGUI.Units(2), rect.width, height);

            bool needsScroll = insideRect.height > outsideRect.height;
            if (needsScroll)
            {
                insideRect.width -= WalletGUI.Units(1);
            }

            Rect listRect = new Rect(rect.x, rect.y + WalletGUI.Units(2), rect.width, height);

            if (needsScroll)
            {
                ListScroll = GUI.BeginScrollView(outsideRect, ListScroll, insideRect);
                listRect = insideRect;
            }

            int newSelectedItemIndex = GUI.SelectionGrid(listRect, selectedItemIndex - offset, listContent.Skip(offset).ToArray(), 1, listStyle) + offset;
            if (newSelectedItemIndex != selectedItemIndex)
            {
                selectedItemIndex = newSelectedItemIndex;
                done = true;
            }

            if (needsScroll)
            {
                GUI.EndScrollView();
            }

            height += WalletGUI.Units(2);
        }

        if (done)
            isClickedComboButton = false;

        return selectedItemIndex;
    }

    public int SelectedItemIndex
    {
        get
        {
            return selectedItemIndex;
        }
        set
        {
            selectedItemIndex = value;
        }
    }

    public bool DropDownIsOpened()
    {
        return isClickedComboButton;
    }

    public void ResetState()
    {
        isClickedComboButton = false;
    }
}
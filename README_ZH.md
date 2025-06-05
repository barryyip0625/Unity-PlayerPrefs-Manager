# Unity PlayerPrefs Manager

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 概述

PlayerPrefs Manager 是一款 Unity 編輯器擴充工具，能夠讓您直接在 Unity 編輯器中輕鬆查看、編輯、建立和刪除 PlayerPrefs 資料。這個工具對於需要管理遊戲設定、使用者偏好設定和遊戲存檔的開發者特別有用。

## 功能特色

- 查看所有現有的 PlayerPrefs 項目，包括其類型和值
- 添加新的 PlayerPrefs 項目（字串、整數、浮點數）
- 編輯現有的 PlayerPrefs 值和類型
- 刪除單個 PlayerPrefs 項目
- 一次性刪除所有 PlayerPrefs
- 搜尋功能以過濾 PlayerPrefs 項目
- 選項顯示原始鍵值（帶有 Unity 的雜湊後綴）

## 安裝方式

### 方式一：使用 Unity 套件

1. 下載最新的 Unity 套件發行檔
2. 在 Unity 編輯器中，前往 Assets > Import Package > Custom Package
3. 選擇剛剛下載的套件檔案
4. 確認所有檔案都已勾選，然後點擊 Import

### 方式二：手動安裝

1. 複製或下載本專案
2. 將 `PlayerPrefsManager.cs` 檔案複製到你的 Unity 專案中的 `Assets/Editor` 資料夾
   （如果沒有 Editor 資料夾請自行建立）

## 使用方法

1. 在 Unity 編輯器中，前往 Tools > PlayerPrefsManager
2. 視窗將顯示您專案中所有現有的 PlayerPrefs
3. 使用搜尋欄位來過濾項目
4. 切換「Show Raw Keys」選項以查看 Unity 內部使用的雜湊版本鍵值
5. 使用視窗頂部的欄位添加新項目
6. 使用每行上的按鈕編輯或刪除現有項目

## 授權

本專案採用 MIT 授權條款，詳情請參閱 [LICENSE](LICENSE) 檔案

## 貢獻

歡迎提出 Issue 或 Pull Request，您的貢獻將讓這個工具更加完善！

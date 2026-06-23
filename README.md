# 人体冒険オープンワールドRPG（はたらく細胞 × 原神 風）

Unity 製のオープンワールドRPGの**第1ステップ「プレイヤーの移動と変身（憑依）システムの基礎」**の実装です。
プレイヤーは「脳の意識体」として人体内部を冒険し、様々な細胞に**変身（憑依）**しながら行動します。

## ✅ このステップで動くもの

- 三人称視点でのキャラクター移動（WASD / 矢印キー）
- ジャンプ・スプリント
- **変身システム**：`Q`キーで「赤血球 ⇄ マクロファージ」を切り替え
- 近接攻撃（左クリック）・固有スキル（`E`キー）
- 簡易な敵（ウイルス）にダメージ → 撃破
- HUD（現在の細胞名・HPバー）
- マウス追従カメラ（右ドラッグで回転 / ホイールでズーム）

## 🎮 操作方法

| 操作 | キー |
|------|------|
| 移動 | `W` `A` `S` `D` / 矢印キー |
| スプリント | `左Shift` を押しながら移動 |
| ジャンプ | `Space` |
| 攻撃 | `左クリック` |
| スキル | `E` |
| **変身** | `Q` |
| カメラ回転 | `右クリック`ドラッグ |
| ズーム | マウスホイール |

## 📦 スクリプト構成

```
Assets/Scripts/
├── Core/
│   ├── CellStats.cs      … 細胞のステータス（HP/速度/攻撃力など）
│   ├── CellBase.cs       … 全細胞の基底（抽象）クラス。移動/攻撃/スキルの共通処理
│   └── IDamageable.cs    … ダメージを受けられるオブジェクト共通I/F
├── Cells/
│   ├── RedBloodCell.cs   … 赤血球：高速移動・スタミナ無限・ダッシュスキル
│   └── Macrophage.cs     … マクロファージ：重攻撃・広範囲スラム・高耐久
├── Player/
│   ├── PlayerManager.cs  … 入力取得 → アクティブ細胞へ操作を委譲
│   ├── CellSwitcher.cs   … 変身（憑依）システム本体
│   └── FollowCamera.cs   … 簡易三人称カメラ
├── UI/
│   └── PlayerHUD.cs      … 細胞名 / HPバー表示
└── Enemy/
    └── SimpleVirus.cs    … 動作確認用の最小限の敵
```

## 🚀 Unity でのセットアップ手順

> **対象バージョン**: Unity 2021 LTS 以降（URPでもBuilt-inでも可）。
> 入力は旧 **Input Manager** API（`Input.GetAxis` 等）を使用しているため、
> 追加パッケージ無しで動きます。（後で New Input System へ差し替え可能）

### 1. プロジェクトに取り込む
1. Unity で新規 3D プロジェクトを作成。
2. この `Assets/Scripts` フォルダをプロジェクトの `Assets` 配下にコピー。

### 2. プレイヤーを組む
1. 空の GameObject を作り、名前を `Player` にする（これがルート）。
2. `Player` に **PlayerManager** と **CellSwitcher** をアタッチ。
3. `Player` の子として **2つの細胞** を作る：
   - `Cell_RedBloodCell`：Capsule などのメッシュ + `CharacterController` + **RedBloodCell** スクリプト
   - `Cell_Macrophage`：Capsule などのメッシュ + `CharacterController` + **Macrophage** スクリプト
   - ※ `CellBase` は `CharacterController` を要求するので自動で付きます。
4. `CellSwitcher` の `cells` 配列に、上記2つの細胞をドラッグして登録。
5. `CellSwitcher` の `playerManager` に `Player` の PlayerManager を割り当て（同一GameObjectなら自動取得）。

### 3. カメラを設定
1. `Main Camera` に **FollowCamera** をアタッチ。
2. `FollowCamera.target` に `Player`（CellSwitcher）を割り当て。

### 4. HUD（任意）
1. `Canvas` を作成し、子に `Text`（細胞名）、`Image`（HPバー: Image Type = `Filled` / Horizontal）、`Text`（HP数値）を配置。
2. 空 GameObject に **PlayerHUD** をアタッチし、各UI要素と `cellSwitcher` を割り当て。

### 5. 敵（動作確認用）
1. Sphere などを作り、Collider を付けて **SimpleVirus** をアタッチ。
2. マクロファージに変身して近づき、左クリックや `E`（範囲スラム）でダメージが入るか確認。
   - ※ `Macrophage.enemyLayer` で対象レイヤーを絞れます（初期値は全レイヤー）。

### 6. 地面
- Plane を1枚置けば移動とジャンプの確認ができます。

## 🔢 ステータス初期値（Inspectorで調整可）

| 細胞 | 役割 | 特徴 |
|------|------|------|
| 赤血球 | 探索 | 高速移動・スタミナ無限・ダッシュ(`E`) |
| マクロファージ | 戦闘 | 高耐久・重攻撃・範囲スラム(`E`) |

各数値（`maxHP`, `moveSpeed`, `attackPower` など）は各細胞コンポーネントの `stats` から調整してください。

## 🗺️ 次のステップ（仕様書より）

- 血管ハイウェイ（Spline による高速移動）
- 敵AI（NavMesh / Behavior Tree）
- New Input System への移行（PC + スマホ仮想パッド対応）
- Visual Effect Graph による演出
```

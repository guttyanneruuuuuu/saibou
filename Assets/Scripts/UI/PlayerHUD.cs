using UnityEngine;
using UnityEngine.UI;
using HumanBodyRPG.Core;
using HumanBodyRPG.Player;

namespace HumanBodyRPG.UI
{
    /// <summary>
    /// 画面上に「現在の細胞名」と「HPバー」を表示する簡易HUD。
    /// CellSwitcher の OnCellChanged と CellBase の OnHPChanged を購読する。
    ///
    /// セットアップ:
    ///  - Canvas 配下に Text(cellNameText) と Image(hpFill: type=Filled, Horizontal) を用意。
    ///  - この値を Inspector で割り当てる。
    /// </summary>
    public class PlayerHUD : MonoBehaviour
    {
        [Header("参照")]
        public CellSwitcher cellSwitcher;

        [Header("UI要素")]
        public Text cellNameText;          // 例: "赤血球 (Red Blood Cell)"
        public Image hpFill;               // Image.type = Filled
        public Text hpText;                // 例: "100 / 100"

        private CellBase tracked;

        private void Start()
        {
            if (cellSwitcher != null)
            {
                cellSwitcher.OnCellChanged += HandleCellChanged;
                if (cellSwitcher.Current != null)
                    HandleCellChanged(cellSwitcher.Current);
            }
        }

        private void OnDestroy()
        {
            if (cellSwitcher != null)
                cellSwitcher.OnCellChanged -= HandleCellChanged;
            Unsubscribe();
        }

        private void HandleCellChanged(CellBase cell)
        {
            Unsubscribe();
            tracked = cell;
            if (tracked == null) return;

            tracked.OnHPChanged += UpdateHP;
            if (cellNameText != null) cellNameText.text = tracked.stats.cellName;
            UpdateHP(tracked.stats.currentHP, tracked.stats.maxHP);
        }

        private void Unsubscribe()
        {
            if (tracked != null)
                tracked.OnHPChanged -= UpdateHP;
        }

        private void UpdateHP(float current, float max)
        {
            if (hpFill != null) hpFill.fillAmount = max > 0 ? current / max : 0f;
            if (hpText != null) hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }
}

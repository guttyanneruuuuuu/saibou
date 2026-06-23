using UnityEngine;
using HumanBodyRPG.Core;

namespace HumanBodyRPG.Player
{
    /// <summary>
    /// 変身（憑依）システム。
    /// 仕様書「Qキーまたは画面ボタンで、現在のプレイヤーのモデルとステータスが入れ替わる CellSwitcher」に対応。
    ///
    /// 仕組み:
    ///  - 子オブジェクトとして複数の細胞モデル（CellBase を持つ GameObject）を保持する。
    ///  - Qキー（または NextCell()）で次の細胞に切り替える。
    ///  - 切り替え時、非アクティブ細胞は SetActive(false)、新細胞を有効化し、
    ///    位置・向きを引き継いで PlayerManager.ActiveCell を差し替える。
    /// </summary>
    public class CellSwitcher : MonoBehaviour
    {
        [Header("参照")]
        public PlayerManager playerManager;

        [Tooltip("切り替え対象の細胞モデル一覧（子オブジェクト推奨）")]
        public CellBase[] cells;

        [Header("操作")]
        public KeyCode switchKey = KeyCode.Q;

        [Header("現在の状態")]
        [SerializeField] private int currentIndex = 0;

        // UIなどへ「変身したよ」を通知するためのイベント（現在の細胞を渡す）
        public System.Action<CellBase> OnCellChanged;

        private void Start()
        {
            if (playerManager == null)
                playerManager = GetComponent<PlayerManager>();

            // 初期化: 最初の細胞だけ有効化
            for (int i = 0; i < cells.Length; i++)
                if (cells[i] != null)
                    cells[i].gameObject.SetActive(i == currentIndex);

            ApplyActiveCell(currentIndex);
        }

        private void Update()
        {
            if (Input.GetKeyDown(switchKey))
                NextCell();
        }

        /// <summary>次の細胞へ循環的に切り替える。</summary>
        public void NextCell()
        {
            if (cells == null || cells.Length == 0) return;
            int next = (currentIndex + 1) % cells.Length;
            SwitchTo(next);
        }

        /// <summary>指定インデックスの細胞へ切り替える（UIボタンからも呼べる）。</summary>
        public void SwitchTo(int index)
        {
            if (cells == null || index < 0 || index >= cells.Length) return;
            if (index == currentIndex && cells[index].gameObject.activeSelf) return;

            CellBase oldCell = cells[currentIndex];
            CellBase newCell = cells[index];
            if (newCell == null) return;

            // 位置・向きを引き継ぐ
            if (oldCell != null)
            {
                newCell.transform.SetPositionAndRotation(
                    oldCell.transform.position, oldCell.transform.rotation);
                oldCell.gameObject.SetActive(false);
            }

            newCell.gameObject.SetActive(true);
            currentIndex = index;
            ApplyActiveCell(index);
        }

        private void ApplyActiveCell(int index)
        {
            CellBase cell = cells[index];
            if (playerManager != null)
                playerManager.ActiveCell = cell;

            OnCellChanged?.Invoke(cell);
            Debug.Log($"[CellSwitcher] 変身 -> {cell.stats.cellName}");
        }

        /// <summary>現在アクティブな細胞を返す。</summary>
        public CellBase Current =>
            (cells != null && currentIndex < cells.Length) ? cells[currentIndex] : null;
    }
}

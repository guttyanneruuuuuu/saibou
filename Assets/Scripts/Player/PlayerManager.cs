using UnityEngine;
using HumanBodyRPG.Core;

namespace HumanBodyRPG.Player
{
    /// <summary>
    /// プレイヤー（脳の意識体）の中枢。
    /// 仕様書「PlayerManager が現在憑依している細胞のHP/スピード/スキルをリアルタイム管理」に対応。
    ///
    /// 役割:
    ///  - 入力の取得（移動 / ジャンプ / 攻撃 / スキル）
    ///  - カメラ基準でのワールド移動方向の計算
    ///  - 現在アクティブな CellBase へ操作を委譲
    ///
    /// 実際の「どの細胞か」の切り替えは CellSwitcher が行い、
    /// PlayerManager は ActiveCell プロパティ経由でそれを操作する。
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [Header("参照")]
        [Tooltip("カメラ（移動方向の基準）。未設定なら Camera.main を使用")]
        public Transform cameraTransform;

        [Header("現在憑依している細胞")]
        [SerializeField] private CellBase activeCell;

        /// <summary>現在操作中の細胞。CellSwitcher から切り替えられる。</summary>
        public CellBase ActiveCell
        {
            get => activeCell;
            set => activeCell = value;
        }

        private void Awake()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (activeCell == null) return;

            HandleMovement();
            HandleActions();
        }

        private void HandleMovement()
        {
            // --- 入力取得（旧 Input Manager / Input System どちらでも差し替え可能な箇所） ---
            float h = Input.GetAxisRaw("Horizontal"); // A/D, ←/→
            float v = Input.GetAxisRaw("Vertical");   // W/S, ↑/↓
            bool sprint = Input.GetKey(KeyCode.LeftShift);

            // カメラ基準のワールド方向に変換
            Vector3 worldDir = Vector3.zero;
            if (cameraTransform != null)
            {
                Vector3 fwd = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                fwd.y = 0f; right.y = 0f;
                fwd.Normalize(); right.Normalize();
                worldDir = (fwd * v + right * h);
            }
            else
            {
                worldDir = new Vector3(h, 0f, v);
            }

            if (worldDir.sqrMagnitude > 1f) worldDir.Normalize();

            activeCell.Move(worldDir, sprint);

            if (Input.GetButtonDown("Jump")) // Space
                activeCell.Jump();
        }

        private void HandleActions()
        {
            // 攻撃: 左クリック
            if (Input.GetMouseButtonDown(0))
                activeCell.Attack();

            // スキル: E キー
            if (Input.GetKeyDown(KeyCode.E))
                activeCell.UseSkill();
        }
    }
}

using UnityEngine;
using HumanBodyRPG.Core;

namespace HumanBodyRPG.Cells
{
    /// <summary>
    /// 赤血球：高速移動・スタミナ無限。フィールド探索や迅速なエリア移動に最適。
    /// 仕様書「RedBloodCell：高速移動、スタミナ無限」に対応。
    /// </summary>
    public class RedBloodCell : CellBase
    {
        [Header("赤血球 固有設定")]
        [Tooltip("ダッシュスキル使用時の瞬間加速量")]
        public float dashImpulse = 18f;
        [Tooltip("ダッシュスキルのクールタイム")]
        public float dashCooldown = 3f;
        private float lastDashTime = -999f;

        protected override void Awake()
        {
            // 赤血球らしいデフォルト値（Inspectorで上書き可能）
            if (string.IsNullOrEmpty(stats.cellName) || stats.cellName == "Cell")
                stats.cellName = "赤血球 (Red Blood Cell)";
            base.Awake();
        }

        // 赤血球はスタミナ無限なのでスプリントしても消費しない（常に高速）
        protected override float GetCurrentMoveSpeed(bool sprint)
        {
            stats.infiniteStamina = true;
            return sprint ? stats.moveSpeed * 1.8f : stats.moveSpeed;
        }

        protected override void PerformAttack()
        {
            // 赤血球は戦闘向きではない（体当たり程度の弱い攻撃）
            Debug.Log($"[{stats.cellName}] 体当たり! dmg={stats.attackPower}");
        }

        public override void UseSkill()
        {
            // スキル: 進行方向へ瞬間ダッシュ（探索特化）
            if (Time.time - lastDashTime < dashCooldown) return;
            lastDashTime = Time.time;

            controller.Move(transform.forward * dashImpulse * Time.deltaTime * 30f);
            Debug.Log($"[{stats.cellName}] ダッシュ!");
        }
    }
}

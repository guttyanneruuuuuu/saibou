using UnityEngine;
using HumanBodyRPG.Core; // IDamageable, CellBase

namespace HumanBodyRPG.Cells
{
    /// <summary>
    /// マクロファージ：重攻撃・広範囲破壊・高耐久。多数の敵や強敵との戦闘の主力。
    /// 仕様書「Macrophage：重攻撃、広範囲破壊、高耐久」に対応。
    /// </summary>
    public class Macrophage : CellBase
    {
        [Header("マクロファージ 固有設定")]
        [Tooltip("広範囲攻撃の半径")]
        public float aoeRadius = 3.5f;
        [Tooltip("当たり判定の対象レイヤー（敵）")]
        public LayerMask enemyLayer = ~0;
        [Tooltip("スキル(範囲スラム)の追加ダメージ倍率")]
        public float skillDamageMultiplier = 2.5f;
        [Tooltip("スキルのクールタイム")]
        public float skillCooldown = 5f;
        private float lastSkillTime = -999f;

        protected override void Awake()
        {
            if (string.IsNullOrEmpty(stats.cellName) || stats.cellName == "Cell")
                stats.cellName = "マクロファージ (Macrophage)";
            base.Awake();
        }

        protected override void PerformAttack()
        {
            // 前方範囲の敵にダメージを与える（重攻撃）
            DealAreaDamage(transform.position + transform.forward * 1.5f,
                           stats.attackRange, stats.attackPower);
        }

        public override void UseSkill()
        {
            // スキル: 自身を中心とした広範囲スラム攻撃
            if (Time.time - lastSkillTime < skillCooldown) return;
            lastSkillTime = Time.time;

            DealAreaDamage(transform.position, aoeRadius,
                           stats.attackPower * skillDamageMultiplier);
            Debug.Log($"[{stats.cellName}] 範囲スラム! radius={aoeRadius}");
        }

        private void DealAreaDamage(Vector3 center, float radius, float damage)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius, enemyLayer);
            int count = 0;
            foreach (var h in hits)
            {
                // IDamageable を実装した敵にダメージを与える
                var dmg = h.GetComponentInParent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damage);
                    count++;
                }
            }
            Debug.Log($"[{stats.cellName}] {count}体にヒット (dmg={damage})");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
    }
}

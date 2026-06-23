using UnityEngine;

namespace HumanBodyRPG.Core
{
    /// <summary>
    /// 細胞ユニットのステータスを表すデータ。
    /// Inspector で簡単に調整できるよう [System.Serializable] にしている。
    /// 仕様書（人体冒険オープンワールドRPG）の「PlayerManager がHP/スピード/スキルを管理」に対応。
    /// </summary>
    [System.Serializable]
    public class CellStats
    {
        [Header("基本情報")]
        public string cellName = "Cell";

        [Header("体力")]
        public float maxHP = 100f;
        [System.NonSerialized] public float currentHP;

        [Header("移動")]
        [Tooltip("移動速度 (m/s)")]
        public float moveSpeed = 6f;
        [Tooltip("回転の滑らかさ。大きいほど素早く向きを変える")]
        public float rotationSpeed = 12f;
        [Tooltip("ジャンプ/浮上の力")]
        public float jumpPower = 5f;

        [Header("スタミナ")]
        [Tooltip("true の場合スタミナ無限（赤血球など）")]
        public bool infiniteStamina = false;
        public float maxStamina = 100f;
        [System.NonSerialized] public float currentStamina;

        [Header("戦闘")]
        [Tooltip("近接攻撃のダメージ")]
        public float attackPower = 10f;
        [Tooltip("攻撃の射程 (m)")]
        public float attackRange = 2f;
        [Tooltip("攻撃のクールタイム (秒)")]
        public float attackCooldown = 0.6f;

        /// <summary>戦闘開始時などにHP/スタミナを最大値で初期化する。</summary>
        public void ResetRuntimeValues()
        {
            currentHP = maxHP;
            currentStamina = maxStamina;
        }
    }
}

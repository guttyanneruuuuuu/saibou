using UnityEngine;
using HumanBodyRPG.Core;

namespace HumanBodyRPG.Enemy
{
    /// <summary>
    /// 動作確認用の最小限の敵（ウイルス）。
    /// 第1ステップでは「攻撃が当たってHPが減り、倒せる」ことの確認に使う。
    /// 本格的なAI（NavMesh / Behavior Tree）は後続ステップで実装する。
    /// </summary>
    public class SimpleVirus : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHP = 30f;
        private float hp;

        private void Awake() => hp = maxHP;

        public void TakeDamage(float amount)
        {
            hp -= amount;
            Debug.Log($"[Virus] {amount} ダメージ -> 残りHP {Mathf.Max(0, hp)}");

            // ヒット表現（赤く点滅）
            var rend = GetComponentInChildren<Renderer>();
            if (rend != null) StartCoroutine(Flash(rend));

            if (hp <= 0f)
            {
                Debug.Log("[Virus] 撃破!");
                Destroy(gameObject);
            }
        }

        private System.Collections.IEnumerator Flash(Renderer rend)
        {
            Color original = rend.material.color;
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if (rend != null) rend.material.color = original;
        }
    }
}

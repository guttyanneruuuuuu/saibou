using UnityEngine;

namespace HumanBodyRPG.Core
{
    /// <summary>
    /// 全ての細胞ユニットの基底となる抽象クラス。
    /// 仕様書の「CellBase 抽象クラスを定義し、移動・攻撃・スキル発動などの共通インターフェースを設ける」に対応。
    ///
    /// このコンポーネントは「変身先のモデル」プレハブごとにアタッチして使う。
    /// PlayerManager が現在アクティブな CellBase を切り替えて操作する。
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public abstract class CellBase : MonoBehaviour
    {
        [Header("ステータス")]
        public CellStats stats = new CellStats();

        protected CharacterController controller;
        protected float verticalVelocity;
        protected float lastAttackTime = -999f;

        // 攻撃が発生したことを外部（UI/エフェクト）へ通知するイベント
        public System.Action<float> OnAttack;       // 引数: ダメージ量
        public System.Action OnDeath;
        public System.Action<float, float> OnHPChanged; // 現在HP, 最大HP

        protected virtual void Awake()
        {
            controller = GetComponent<CharacterController>();
            stats.ResetRuntimeValues();
        }

        protected virtual void OnEnable()
        {
            // 変身で再アクティブ化されたときにHPバーを更新
            OnHPChanged?.Invoke(stats.currentHP, stats.maxHP);
        }

        /// <summary>
        /// 入力方向（カメラ基準のワールド方向）を受け取って移動する。
        /// 共通の重力・回転処理を実装し、細胞ごとの味付けは派生先で行う。
        /// </summary>
        /// <param name="worldMoveDir">正規化されたワールド空間の移動方向</param>
        /// <param name="sprint">スプリント入力</param>
        public virtual void Move(Vector3 worldMoveDir, bool sprint)
        {
            float speed = GetCurrentMoveSpeed(sprint);

            if (worldMoveDir.sqrMagnitude > 0.001f)
            {
                // 進行方向へ滑らかに回転
                Quaternion target = Quaternion.LookRotation(worldMoveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, target, stats.rotationSpeed * Time.deltaTime);
            }

            // 重力
            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

            Vector3 motion = worldMoveDir * speed + Vector3.up * verticalVelocity;
            controller.Move(motion * Time.deltaTime);
        }

        /// <summary>派生クラスで速度の計算ルールを変えられるようにする。</summary>
        protected virtual float GetCurrentMoveSpeed(bool sprint)
        {
            return sprint ? stats.moveSpeed * 1.6f : stats.moveSpeed;
        }

        public virtual void Jump()
        {
            if (controller.isGrounded)
                verticalVelocity = stats.jumpPower;
        }

        /// <summary>
        /// 近接攻撃。クールタイムを共通管理し、実際のヒット判定は派生先で実装。
        /// </summary>
        public virtual void Attack()
        {
            if (Time.time - lastAttackTime < stats.attackCooldown)
                return;

            lastAttackTime = Time.time;
            OnAttack?.Invoke(stats.attackPower);
            PerformAttack();
        }

        /// <summary>細胞ごとの攻撃の実体（射程・範囲など）を実装する。</summary>
        protected abstract void PerformAttack();

        /// <summary>細胞固有スキル（変身先により挙動が変わる）。</summary>
        public abstract void UseSkill();

        public virtual void TakeDamage(float amount)
        {
            stats.currentHP = Mathf.Max(0f, stats.currentHP - amount);
            OnHPChanged?.Invoke(stats.currentHP, stats.maxHP);
            if (stats.currentHP <= 0f)
                OnDeath?.Invoke();
        }
    }
}

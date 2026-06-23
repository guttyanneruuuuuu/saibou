namespace HumanBodyRPG.Core
{
    /// <summary>
    /// ダメージを受けられるオブジェクトの共通インターフェース。
    /// 敵（ウイルス）やプレイヤー細胞が実装する。
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}

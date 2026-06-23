using UnityEngine;
using HumanBodyRPG.Player;

namespace HumanBodyRPG.Player
{
    /// <summary>
    /// プレイヤー細胞を追従する簡易な三人称カメラ。
    /// マウスで周囲を見回し（右ドラッグ不要・常時）、ホイールでズーム。
    /// 本格的なカメラは後でCinemachineへ置き換え推奨。
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [Header("追従対象")]
        [Tooltip("CellSwitcher を持つプレイヤールート。実体の細胞は自動追従する")]
        public CellSwitcher target;
        [Tooltip("target を使わず固定のTransformを追う場合はこちら")]
        public Transform explicitTarget;

        [Header("カメラ設定")]
        public float distance = 8f;
        public float minDistance = 3f;
        public float maxDistance = 20f;
        public float height = 3f;
        public float rotationSpeed = 180f;
        public float followLerp = 10f;

        private float yaw;
        private float pitch = 15f;

        private void Start()
        {
            yaw = transform.eulerAngles.y;
        }

        private void LateUpdate()
        {
            Transform t = GetTargetTransform();
            if (t == null) return;

            // マウスで回転
            if (Input.GetMouseButton(1)) // 右ドラッグ中だけ回転
            {
                yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, -20f, 70f);
            }

            // ズーム
            distance -= Input.GetAxis("Mouse ScrollWheel") * 5f;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 targetPos = t.position + Vector3.up * height;
            Vector3 desiredPos = targetPos - rot * Vector3.forward * distance;

            transform.position = Vector3.Lerp(
                transform.position, desiredPos, followLerp * Time.deltaTime);
            transform.LookAt(targetPos);
        }

        private Transform GetTargetTransform()
        {
            if (explicitTarget != null) return explicitTarget;
            if (target != null && target.Current != null)
                return target.Current.transform;
            return null;
        }
    }
}

using UnityEngine;

namespace AnimalMerge
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class AnimalController : MonoBehaviour
    {
        public AnimalData data { get; private set; }
        public bool IsDropped { get; private set; }

        // 투하 직후 1.5초 유예 — 낙하 중 데드라인 통과 오탐 방지
        public bool IsEligibleForGameOver => IsDropped && Time.time - droppedTime > 0.5f;

        private bool isMerging;
        private float droppedTime;
        private Rigidbody2D rb;
        private static PhysicsMaterial2D sharedMat;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Initialize(AnimalData animalData, Sprite circleSprite)
        {
            data = animalData;

            var sr = GetComponent<SpriteRenderer>();
            sr.sprite = animalData.sprite != null ? animalData.sprite : circleSprite;
            sr.color = animalData.sprite != null ? Color.white : animalData.color;
            sr.sortingOrder = 1;

            var col = GetComponent<CircleCollider2D>();
            col.radius = data.sprite != null ? data.sprite.bounds.extents.x : 0.5f;
            if (sharedMat == null)
                sharedMat = new PhysicsMaterial2D("Animal") { friction = 0.2f, bounciness = 0.1f };
            col.sharedMaterial = sharedMat;

            rb.gravityScale = 2f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.3f;
            rb.constraints = RigidbodyConstraints2D.None; // 회전 허용 → 굴러가는 물리
            rb.simulated = false;

            transform.localScale = Vector3.one * animalData.radius * 1.4f;
        }

        public void Drop()
        {
            IsDropped = true;
            droppedTime = Time.time;
            rb.simulated = true;
            // 랜덤 횡력 — 수직 낙하 시 옆으로 굴러가도록
            rb.AddForce(new Vector2(Random.Range(-0.4f, 0.4f), 0f), ForceMode2D.Impulse);
            GameManager.Instance.RegisterAnimal(this);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsDropped || isMerging)
                return;
            if (data.stage >= 10)
                return;

            var other = collision.gameObject.GetComponent<AnimalController>();
            if (other == null || !other.IsDropped || other.isMerging)
                return;
            if (other.data.stage != data.stage)
                return;

            isMerging = true;
            other.isMerging = true;

            Vector2 mid = ((Vector2)transform.position + (Vector2)other.transform.position) * 0.5f;

            GameManager.Instance.UnregisterAnimal(this);
            GameManager.Instance.UnregisterAnimal(other);
            GameManager.Instance.OnMerge(data.stage, mid, data.mergeSound);

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}

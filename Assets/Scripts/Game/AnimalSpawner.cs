using System.Collections;
using UnityEngine;

namespace AnimalMerge
{
    public class AnimalSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public float spawnY = 5f;
        public float wallInnerX = 3f;
        public float dropCooldown = 0.5f;
        public int guidelineDropLimit = 3;

        private AnimalController currentAnimal;
        private int currentIdx;
        private int nextIdx;
        private int afterNextIdx;
        private int totalDropCount;
        private bool canDrop = true;
        private Sprite circleSprite;
        private LineRenderer guideline;
        private Camera mainCam;

        private AnimalData[] AnimalDatas => GameManager.Instance.allAnimalData;
        private UIManager UI => GameManager.Instance.uiManager;

        private void Start()
        {
            mainCam = Camera.main;
            circleSprite = CreateCircleSprite();
            UI?.SetCircleSprite(circleSprite);
            SetupGuideline();

            currentIdx = GetRandomIdx();
            nextIdx = GetRandomIdx();
            afterNextIdx = GetRandomIdx();

            SpawnCurrentAnimal();
            RefreshUI();
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver || currentAnimal == null)
                return;

            TrackMouse();
            UpdateGuideline();

            if (Input.GetMouseButtonDown(0) && canDrop)
                DropCurrentAnimal();
        }

        private void TrackMouse()
        {
            Vector3 world = mainCam.ScreenToWorldPoint(Input.mousePosition);
            float r = currentAnimal.data.radius;
            float x = Mathf.Clamp(world.x, -wallInnerX + r, wallInnerX - r);
            currentAnimal.transform.position = new Vector3(x, spawnY, 0f);
        }

        private void DropCurrentAnimal()
        {
            canDrop = false;
            currentAnimal.Drop();
            currentAnimal = null;
            totalDropCount++;

            if (totalDropCount >= guidelineDropLimit)
                guideline.enabled = false;

            currentIdx = nextIdx;
            nextIdx = afterNextIdx;
            afterNextIdx = GetRandomIdx();

            RefreshUI();
            StartCoroutine(SpawnAfterDelay());
        }

        private IEnumerator SpawnAfterDelay()
        {
            yield return new WaitForSeconds(dropCooldown);
            if (!GameManager.Instance.IsGameOver)
            {
                SpawnCurrentAnimal();
                canDrop = true;
            }
        }

        private void SpawnCurrentAnimal()
        {
            currentAnimal = CreateAnimal(AnimalDatas[currentIdx], new Vector3(0f, spawnY, 0f));
        }

        public void SpawnMergedAnimal(int stage, Vector2 position)
        {
            if (stage < 1 || stage > 10)
                return;
            var animal = CreateAnimal(AnimalDatas[stage - 1], position);
            animal.Drop();
        }

        private AnimalController CreateAnimal(AnimalData data, Vector3 pos)
        {
            var go = new GameObject($"Animal_{data.animalName}");
            go.transform.position = pos;
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<CircleCollider2D>();
            var ctrl = go.AddComponent<AnimalController>();
            ctrl.Initialize(data, circleSprite);
            return ctrl;
        }

        private int GetRandomIdx()
        {
            int max = totalDropCount < 5 ? 2 : 5;
            return Random.Range(0, max);
        }

        private void RefreshUI()
        {
            UI?.UpdateNextPreviews(AnimalDatas[nextIdx], AnimalDatas[afterNextIdx]);
        }

        private void UpdateGuideline()
        {
            if (!guideline.enabled)
                return;
            Vector3 top = currentAnimal.transform.position;
            guideline.SetPosition(0, top);
            guideline.SetPosition(1, new Vector3(top.x, -5f, 0f));
        }

        private void SetupGuideline()
        {
            guideline = gameObject.AddComponent<LineRenderer>();
            guideline.useWorldSpace = true;
            guideline.positionCount = 2;
            guideline.startWidth = 0.05f;
            guideline.endWidth = 0.05f;
            guideline.sortingOrder = 10;
            var mat = new Material(Shader.Find("Sprites/Default"));
            guideline.material = mat;
            guideline.startColor = new Color(1f, 1f, 1f, 0.7f);
            guideline.endColor = new Color(1f, 1f, 1f, 0.1f);
            guideline.enabled = true;
        }

        private Sprite CreateCircleSprite()
        {
            const int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var center = new Vector2(size * 0.5f, size * 0.5f);
            float r = size * 0.5f;

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float a = dist < r - 1.5f ? 1f : Mathf.Clamp01(r - dist);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, size);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnimalMerge
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Animal Data")]
        public AnimalData[] allAnimalData; // 인덱스 0~9 = 단계 1~10

        [Header("Settings")]
        public float deadlineY = 4f;
        public float gameOverDelay = 5f;
        public int baseScore = 10;

        [Header("References")]
        public AnimalSpawner spawner;
        public UIManager uiManager;
        public AudioClip mergeClip;
        public AudioClip bgmClip;
        public GameObject mergeFxPrefab;

        private int score;
        private int bestScore;
        private bool isGameOver;
        private float gameOverTimer;
        private readonly List<AnimalController> droppedAnimals = new();

        public bool IsGameOver => isGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            bestScore = PlayerPrefs.GetInt("BestScore", 0);
        }

        private void Start()
        {
            uiManager?.UpdateScore(score, bestScore);
            if (bgmClip != null)
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.clip = bgmClip;
                src.loop = true;
                src.volume = 0.5f;
                src.Play();
            }
        }

        private void Update()
        {
            if (isGameOver)
                return;
            CheckDeadline();
        }

        private void CheckDeadline()
        {
            bool anyOver = false;
            for (int i = droppedAnimals.Count - 1; i >= 0; i--)
            {
                if (droppedAnimals[i] == null)
                {
                    droppedAnimals.RemoveAt(i);
                    continue;
                }
                if (
                    droppedAnimals[i].IsEligibleForGameOver
                    && droppedAnimals[i].transform.position.y > deadlineY
                )
                {
                    anyOver = true;
                    break;
                }
            }

            if (anyOver)
            {
                gameOverTimer += Time.deltaTime;
                uiManager?.ShowCountdown(Mathf.CeilToInt(gameOverDelay - gameOverTimer));
                if (gameOverTimer >= gameOverDelay)
                    TriggerGameOver();
            }
            else if (gameOverTimer > 0f)
            {
                gameOverTimer = 0f;
                uiManager?.HideCountdown();
            }
        }

        public void RegisterAnimal(AnimalController animal) => droppedAnimals.Add(animal);

        public void UnregisterAnimal(AnimalController animal) => droppedAnimals.Remove(animal);

        public void OnMerge(int currentStage, Vector2 position, AudioClip sound)
        {
            int nextStage = currentStage + 1;
            score += nextStage * baseScore;
            if (score > bestScore)
            {
                bestScore = score;
                PlayerPrefs.SetInt("BestScore", bestScore);
            }
            uiManager?.UpdateScore(score, bestScore);

            AudioClip clip = mergeClip != null ? mergeClip : sound;
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);

            if (mergeFxPrefab != null && currentStage >= 4)
            {
                var fx = Instantiate(mergeFxPrefab, position, Quaternion.identity);
                var ps = fx.GetComponent<ParticleSystem>();
                if (ps != null)
                    Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
                else
                    Destroy(fx, 2f);
            }

            spawner.SpawnMergedAnimal(nextStage, position);
        }

        private void TriggerGameOver()
        {
            isGameOver = true;
            uiManager?.ShowGameOver(score, bestScore);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

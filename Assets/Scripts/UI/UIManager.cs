using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimalMerge
{
    public class UIManager : MonoBehaviour
    {
        [Header("Score")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI bestScoreText;

        [Header("Countdown")]
        public TextMeshProUGUI countdownText;

        [Header("Next Previews")]
        public Image nextImage;
        public Image afterNextImage;
        public TextMeshProUGUI nextNameText;
        public TextMeshProUGUI afterNextNameText;

        [Header("Game Over")]
        public GameObject gameOverPanel;
        public TextMeshProUGUI finalScoreText;
        public TextMeshProUGUI finalBestText;

        private Sprite circleSprite;

        public void SetCircleSprite(Sprite sprite) => circleSprite = sprite;

        public void UpdateScore(int score, int best)
        {
            if (scoreText)
                scoreText.text = score.ToString("N0");
            if (bestScoreText)
                bestScoreText.text = $"{best:N0}";
        }

        public void UpdateNextPreviews(AnimalData next, AnimalData afterNext)
        {
            SetPreview(nextImage, nextNameText, next);
            SetPreview(afterNextImage, afterNextNameText, afterNext);
        }

        private void SetPreview(Image panelImg, TextMeshProUGUI label, AnimalData data)
        {
            if (panelImg)
            {
                // 패널(부모)의 자식 Image가 동물 미리보기, 없으면 자신에 표시
                Image animalImg =
                    panelImg.transform.childCount > 0
                        ? panelImg.transform.GetChild(0).GetComponent<Image>()
                        : null;
                Image target = animalImg != null ? animalImg : panelImg;
                target.sprite = data.sprite != null ? data.sprite : circleSprite;
                target.color = data.sprite != null ? Color.white : data.color;
            }
            if (label)
                label.text = data.animalName;
        }

        public void ShowCountdown(int seconds)
        {
            if (!countdownText)
                return;
            countdownText.gameObject.SetActive(true);
            countdownText.text = seconds.ToString();
        }

        public void HideCountdown()
        {
            if (countdownText)
                countdownText.gameObject.SetActive(false);
        }

        public void ShowGameOver(int score, int best)
        {
            if (!gameOverPanel)
                return;
            gameOverPanel.SetActive(true);
            if (finalScoreText)
                finalScoreText.text = $"점수: {score:N0}";
            if (finalBestText)
                finalBestText.text = $"최고: {best:N0}";
        }
    }
}

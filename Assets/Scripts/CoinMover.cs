using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Нужно установить DOTween
using System.Collections;

public class CoinMover : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject coinPrefab;    // Префаб монетки (UI Image)
    [SerializeField] private Transform targetPosition; // Иконка монетки в углу экрана
    [SerializeField] private Text coinText; // Текст счетчика
    [SerializeField] private Transform canvasParent;   // Родитель для спавна монет (обычно Canvas)

    [Header("Settings")]
    [SerializeField] private float duration = 2f;    // Время полета монеты
    [SerializeField] private float delayBetween = 0.05f; // Задержка между вылетами
    [SerializeField] private AudioClip coinSound;      // Звук монетки
    [Header("Scripts")]
    [SerializeField] private MoneyHandler moneyHandler;
    
    private AudioSource audioSource;
    private int currentCoinsCount = 0;

    void Start()
    {
        currentCoinsCount = moneyHandler.GetMoney();
        audioSource = gameObject.AddComponent<AudioSource>();
        UpdateUI();
    }

    // Главный метод: вызывайте его, когда игрок получает награду
    public void AddCoins(Vector3 spawnPosition, int amount)
    {
        StartCoroutine(AnimateCoins(spawnPosition, amount));
    }

    private IEnumerator AnimateCoins(Vector3 spawnPosition, int amount)
    {
        moneyHandler.AddMoney(amount);
        for (int i = 0; i < amount; i++)
        {
            // 1. Создаем монету
            GameObject coin = Instantiate(coinPrefab, canvasParent);
            coin.transform.position = spawnPosition;
            
            // Немного разбрасываем монеты в начале для "сочности"
            coin.transform.localScale = Vector3.zero;
            coin.transform.DOScale(1f, 0.2f);
            coin.transform.DOMove(spawnPosition + (Vector3)Random.insideUnitCircle * 6f, 0.2f);

            // 2. Летим к цели через небольшую паузу
            coin.transform.DOMove(targetPosition.position, duration)
                .SetDelay(0.2f)
                .SetEase(Ease.InBack) // Эффект рывка в конце
                .OnComplete(() => {
                    // 3. Когда долетела:
                    currentCoinsCount++;
                    UpdateUI();
                    PlayCoinSound();
                    
                    // Анимация пульсации счетчика
                    targetPosition.DOScale(1.2f, 0.1f).OnComplete(() => targetPosition.DOScale(1f, 0.1f));
                    
                    Destroy(coin);
                });

            yield return new WaitForSeconds(delayBetween);
        }
    }

    void UpdateUI()
    {
        coinText.text = "Деньги " + currentCoinsCount.ToString();
    }

    void PlayCoinSound()
    {
        if (coinSound != null)
        {
            // Повышаем Pitch (высоту звука) с каждой монетой для "прикольного" эффекта
            audioSource.pitch = 1f + (currentCoinsCount % 10 * 0.05f); 
            audioSource.PlayOneShot(coinSound);
        }
    }
}

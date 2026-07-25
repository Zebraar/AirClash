using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SessionTimerUIScr : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject warningPanel;

    [Header("Texts")]
    [SerializeField] private Text warningTextTitle;
    [SerializeField] private Text warningTextBody;

    private Vector2 startPosAchievementPanel;
    private Coroutine animationCoroutine; 

    void Awake()
    {
        startPosAchievementPanel = warningPanel.GetComponent<RectTransform>().anchoredPosition;
    }

    void Start()
    {
        if(SessionTimer.Instance != null)
        {
            SessionTimer.Instance.onMinuteChanged.AddListener(ShowWarning);
        }
    }

    void OnDestroy()
    {
        if(SessionTimer.Instance != null)
        {
            SessionTimer.Instance.onMinuteChanged.RemoveListener(ShowWarning);
        }
    }

    private struct WarningMessage
    {
        public string Title;
        public string Body;

        public WarningMessage(string title, string body)
        {
            Title = title;
            Body = body;
        }
    }

    private readonly Dictionary<int, List<WarningMessage>> warnings = new Dictionary<int, List<WarningMessage>>
    {
        { 10, new List<WarningMessage>
            {
                new WarningMessage("10 минут игры!", "Ого, уже целых 10 минут в игре!"),
                new WarningMessage("10 минут игры!", "Время летит незаметно, первая десяточка!"),
                new WarningMessage("10 минут игры!", "10 минут позади, полет нормальный."),
                new WarningMessage("10 минут игры!", "Не забывайте моргать, игра идет уже 10 минут.")
            }
        },
        { 30, new List<WarningMessage>
            {
                new WarningMessage("30 минут игры!", "Не пора ли сделать перерыв?"),
                new WarningMessage("30 минут игры!", "Самое время для разминки."),
                new WarningMessage("30 минут игры!", "Полчаса пролетело! Сделайте глубокий вдох.")
            }
        },
        { 60, new List<WarningMessage>
            {
                new WarningMessage("60 минут игры!", "Может, сделаем разминку?"),
                new WarningMessage("60 минут игры!", "Пора ненадолго отвлечься от игры."),
                new WarningMessage("60 минут игры!", "Уже целый час! Встаньте и потянитесь.")
            }
        },
        { 90, new List<WarningMessage>
            {
                new WarningMessage("90 минут игры!", "Самое время пойти отдохнуть и попить чай."),
                new WarningMessage("90 минут игры!", "Может, немного чаю?"),
                new WarningMessage("90 минут игры!", "Полтора часа - отличный повод сделать паузу.")
            }
        },
        { 120, new List<WarningMessage>
            {
                new WarningMessage("120 минут игры!", "Не пора ли выйти на улицу и подышать воздухом?"),
                new WarningMessage("120 минут игры!", "На улице такая хорошая погода, может выйти?"),
                new WarningMessage("120 минут игры!", "Два часа у экрана! Глазам нужен отдых.")
            }
        },
        { 150, new List<WarningMessage>
            {
                new WarningMessage("150 минут игры!", "Ваши глаза явно не скажут вам спасибо, поэтому может отдохнуть?"),
                new WarningMessage("150 минут игры!", "У вас не болят глаза? Может перерыв?"),
                new WarningMessage("150 минут игры!", "Сделайте перерыв, посмотрите в окно пару минут.")
            }
        },
        { 180, new List<WarningMessage>
            {
                new WarningMessage("180 минут игры!", "Время выйти на улицу и потрогать траву."),
                new WarningMessage("180 минут игры!", "На улице есть трава и можно её потрогать."),
                new WarningMessage("180 минут игры!", "Три часа! Это уже серьезная игровая сессия, пора отдохнуть.")
            }
        },
        { 210, new List<WarningMessage>
            {
                new WarningMessage("210 минут игры!", "Эй, такая сессия может вызвать проблемы со здоровьем, может уже надо наконец-то выключить телефон?"),
                new WarningMessage("210 минут игры!", "Такая сессия вызывает проблемы со здоровьем, не пора ли уже отдохнуть?"),
                new WarningMessage("210 минут игры!", "Пожалуйста, отложите устройство. Ваше здоровье важнее игры.")
            }
        }
    };

    public void ShowWarning(int minutes)
    {
        if (warnings.TryGetValue(minutes, out List<WarningMessage> messageList) && messageList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, messageList.Count);
            WarningMessage randomMessage = messageList[randomIndex];

            warningTextTitle.text = randomMessage.Title;
            warningTextBody.text = randomMessage.Body;

            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateWarningPanel());
        }
    }

    IEnumerator AnimateWarningPanel()
    {
        warningPanel.SetActive(true);
        var rect = warningPanel.GetComponent<RectTransform>();
        rect.DOKill();

        rect.DOAnchorPos(new Vector2(0, -90), 2.0f).SetLink(warningPanel);

        yield return new WaitForSeconds(5f);

        rect.DOAnchorPos(startPosAchievementPanel, 2.0f)
            .SetLink(warningPanel)
            .OnComplete(() => warningPanel.SetActive(false));
    }
}

using UnityEngine;
using TMPro;
using DG.Tweening;

[DisallowMultipleComponent]
public class FPSCounter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private RectTransform textRectTransform;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f;

    private float accum = 0f;
    private int frames = 0;
    private bool isVisible;
    private GameObject textGameObject;

    private readonly char[] displayBuffer = new char[] { 'F', 'P', 'S', ':', ' ', ' ', ' ', ' ' };

    void Awake()
    {
        if(fpsText == null) fpsText = GetComponentInChildren<TextMeshProUGUI>(true);
        if(fpsText != null)
        {
            textGameObject = fpsText.gameObject;
            if (textRectTransform == null) textRectTransform = fpsText.GetComponent<RectTransform>();
        }

        bool shouldBeActive = PlayerPrefs.GetInt("FpsCounter", 0) != 0;
        SetIsActiveImmediate(shouldBeActive);
    }

    private void OnEnable()
    {
        ResetCounters();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus)
        {
            ResetCounters();
        }
    }

    private void ResetCounters()
    {
        accum = 0f;
        frames = 0;
    }

    void Update()
    {
        if(!isVisible) return;

        frames++;
        accum += Time.unscaledDeltaTime;

        if(accum >= updateInterval)
        {
            if(accum > 0f)
            {
                int fps = Mathf.RoundToInt(frames / accum);
                UpdateFPSTextNonAlloc(fps);
            }

            accum = (accum >= updateInterval * 2) ? 0f : accum - updateInterval;
            frames = 0;
        }
    }

    private void UpdateFPSTextNonAlloc(int fps)
    {
        fps = Mathf.Clamp(fps, 0, 999);

        int hundreds = fps / 100;
        int tens = (fps / 10) % 10;
        int ones = fps % 10;

        displayBuffer[5] = hundreds > 0 ? (char)('0' + hundreds) : ' ';
        displayBuffer[6] = (hundreds > 0 || tens > 0) ? (char)('0' + tens) : ' ';
        displayBuffer[7] = (char)('0' + ones);

        fpsText.SetText(displayBuffer, 0, displayBuffer.Length);
    }

    private void SetIsActiveImmediate(bool isActive)
    {
        isVisible = isActive;
        if(textGameObject != null) textGameObject.SetActive(isActive);
        if(textRectTransform != null) textRectTransform.localScale = isActive ? Vector3.one : Vector3.zero;
    }

    public void SetIsActive(bool isActive)
    {
        if(textGameObject == null || textRectTransform == null) return;
        
        textRectTransform.DOKill(); 

        if(isActive)
        {
            isVisible = true;
            textGameObject.SetActive(true);
            textRectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        else
        {
            isVisible = false;
            textRectTransform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .SetUpdate(true)
                .OnComplete(() => textGameObject.SetActive(false));
        }
    }
}
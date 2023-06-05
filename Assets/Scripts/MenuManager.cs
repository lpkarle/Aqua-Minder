using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject panelOnboarding, panelMessage;

    [SerializeField] private Transform locationSpeechbubble;

    // Only for development
    [SerializeField] private TextMeshProUGUI textSystemInfo;

    private TextMeshProUGUI textWelcome;

    private void Awake()
    {
        Instance = this;

        GameManager.OnAquaMinderStateChanged += GameManagerOnAquaMinderStateChanged;
    }

    void Start()
    {
        textWelcome = panelMessage.GetComponentInChildren<TextMeshProUGUI>();

        panelOnboarding.transform.position = locationSpeechbubble.position;
        panelMessage.transform.position = locationSpeechbubble.position;
    }

    void OnDestroy()
    {
        GameManager.OnAquaMinderStateChanged -= GameManagerOnAquaMinderStateChanged;
    }

    public void ShowWelcomeMessage(string userName)
    {
        textWelcome.text = $"Willkommen {userName}\n\nfrohes trinken :)";
    }

    // Only for development
    public void UpdateSystemInfoText(User user, float temperature, float humidity, float weight)
    {
        if (user == null)
        {
            textSystemInfo.text = $@"
            User: null
            Temperature: {temperature} °C
            Luftfeuchtigkeit: {humidity} %
            Bereits getrunken: {weight} ml
            ";
        }
        else
        {
            textSystemInfo.text = $@"
            User: {user.name}, {user.uid}, {user.drankWeight} ml
            Temperature: {temperature} °C
            Luftfeuchtigkeit: {humidity} %
            Bereits getrunken: {weight} ml
            ";
        }
    }

    private void GameManagerOnAquaMinderStateChanged(AquaMinderState state)
    {
        panelOnboarding.SetActive(state == AquaMinderState.ONBOARDING);
        panelMessage.SetActive(state == AquaMinderState.BOTTLE_ON);
    }
}

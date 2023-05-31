using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField]
    private GameObject panelOnboarding, panelMessage;

    private void Awake()
    {
        Instance = this;

        GameManager.OnAquaMinderStateChanged += GameManagerOnAquaMinderStateChanged;
    }

    void OnDestroy()
    {
        GameManager.OnAquaMinderStateChanged -= GameManagerOnAquaMinderStateChanged;
    }

    private void GameManagerOnAquaMinderStateChanged(AquaMinderState state)
    {
        panelOnboarding.SetActive(state == AquaMinderState.ONBOARDING);
        panelMessage.SetActive(state == AquaMinderState.BOTTLE_ON);
    }
}

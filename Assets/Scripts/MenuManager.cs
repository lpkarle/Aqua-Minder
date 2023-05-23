using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject panelOnboarding;

    private void Awake() => GameManager.OnAquaMinderStateChanged += GameManagerOnAquaMinderStateChanged;

    void Start()
    {
        
    }

    private void GameManagerOnAquaMinderStateChanged(AquaMinderState state)
    {
        // TODO panelOnboarding.SetActive(state == AquaMinderState.ONBOARDING);
    }
}

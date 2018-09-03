using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreBoard;

    private PlayerController controller;

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    public void SetController (PlayerController _controller)
    {
        controller = _controller;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(_amount, 1f, 1f);
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }
}

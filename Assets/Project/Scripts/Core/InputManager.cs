using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;
    private LayerMask interactableLayer = -1;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleTouchInput();
        HandleKeyboardInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                ProcessTouch(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            ProcessTouch(Input.mousePosition);
        }
    }

    private void ProcessTouch(Vector2 screenPosition)
    {
        if (GameStateMachine.Instance.CurrentState != GameState.Playing) return;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, interactableLayer);

        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            interactable?.OnInteract();
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    private void HandleBackButton()
    {
        switch (GameStateMachine.Instance.CurrentState)
        {
            case GameState.Playing:
                GameStateMachine.Instance.ChangeState(GameState.Paused);
                break;
            case GameState.Paused:
                GameStateMachine.Instance.ChangeState(GameState.Playing);
                break;
            case GameState.GameOver:
                GameManager.Instance.BackToMenu();
                break;
        }
    }
}
using UnityEngine;
using DG.Tweening;

public class Coin : PoolableObject, IInteractable
{
    [Header("Coin Settings")]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int pointValue = 1;
    [SerializeField] private LayerMask touchLayer = -1;

    [Header("Animation Settings")]
    [SerializeField] private float spawnScale = 0.3f;
    [SerializeField] private float spawnDuration = 0.3f;
    [SerializeField] private float collectScale = 1.2f;
    [SerializeField] private float collectDuration = 0.15f;

    private CircleCollider2D coinCollider;
    private SpriteRenderer spriteRenderer;
    private float timer;
    private Tween currentTween;
    private bool isInteractable = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetCoin();
        PlaySpawnAnimation();
    }

    protected override void Awake()
    {
        base.Awake();
        coinCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (coinCollider == null)
            coinCollider = gameObject.AddComponent<CircleCollider2D>();
    }

    protected override void Update()
    {
        if (GameStateMachine.Instance.CurrentState != GameState.Playing) return;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            DespawnWithAnimation();
        }
    }

    private void ResetCoin()
    {
        timer = 0f;
        isInteractable = true;
        coinCollider.enabled = true;
        spriteRenderer.color = Color.white;
        currentTween?.Kill();
    }

    private void PlaySpawnAnimation()
    {
        transform.localScale = Vector3.one * spawnScale;
        currentTween = transform.DOScale(Vector3.one, spawnDuration)
            .SetEase(Ease.OutBack);
    }

    public void OnInteract()
    {
        if (!isInteractable || GameStateMachine.Instance.CurrentState != GameState.Playing)
            return;

        CollectCoin();
    }

    private void CollectCoin()
    {
        isInteractable = false;
        coinCollider.enabled = false;

        // Score and events
        GameManager.Instance.AddScore(pointValue);
        GameEvents.Instance.TriggerCoinCollected();
        AudioManager.Instance.PlaySFX(SFXType.CoinCollect);

        // Collect animation
        currentTween = transform.DOScale(Vector3.one * collectScale, collectDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                spriteRenderer.DOFade(0f, 0.1f).OnComplete(() => ReturnToPool());
            });
    }

    private void DespawnWithAnimation()
    {
        isInteractable = false;
        coinCollider.enabled = false;

        currentTween = spriteRenderer.DOFade(0f, 0.2f)
            .OnComplete(() => ReturnToPool());
    }

    private void OnMouseDown()
    {
        OnInteract();
    }

    public override void OnReturnToPool()
    {
        currentTween?.Kill();
        spriteRenderer.color = Color.white;
        transform.localScale = Vector3.one;
        base.OnReturnToPool();
    }
}

using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    [SerializeField] private float _fadeTime = 2f;

    private float _timeElapsed = 0f;
    private SpriteRenderer _spriteRenderer;
    private Color _startColor;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeElapsed = 0f;
        _spriteRenderer = animator.GetComponent<SpriteRenderer>();
        _startColor = _spriteRenderer.color;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeElapsed += Time.deltaTime;

        float newAlpha = _startColor.a * (1 - (_timeElapsed / _fadeTime));
        _spriteRenderer.color = new Color(_startColor.r, _startColor.g, _startColor.b, newAlpha);

        if (_timeElapsed > _fadeTime)
        {
            Destroy(this);
        }
    }
}

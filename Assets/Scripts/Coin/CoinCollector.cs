using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Coin>(out Coin coin))
        {
            PlayPickingSound();
            Destroy(collision.gameObject);
        }
    }

    private void PlayPickingSound()
    {
        _audioSource.Play();
    }
}

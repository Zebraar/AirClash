using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip whooshSound;
    [SerializeField] private AudioClip clickSound;

    [Header("Other")]
    [SerializeField] private AudioSource audioSource;

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }
    public void PlayWhooshSound()
    {
        audioSource.PlayOneShot(whooshSound);
    }
}

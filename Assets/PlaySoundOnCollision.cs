using UnityEngine;
public class PlaySoundOnCollision : MonoBehaviour
{
    AudioSource audioCache;
    void Start()
    {
        audioCache = GetComponent<AudioSource>();
    }
    void OnCollisionEnter(Collision info)
    {
        audioCache.Stop();
        audioCache.Play();
    }
}

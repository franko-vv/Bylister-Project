using UnityEngine;

/// <summary>
/// Клас удаления отработавших частиц
/// </summary>
public class DeleteParticle: MonoBehaviour
{
    public ParticleSystem _particlesystem;

    void Start()
    {
        _particlesystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_particlesystem.isPlaying)
            return;

        Destroy(gameObject);
    }
}

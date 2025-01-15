using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectController : MonoBehaviour
{
    // make sure to assign the ParticleSystem component in the inspector
    //set requires particle system
    
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }
    public void PlayParticleEffect()
    {
        particleSystem.Play();
    }   
    public void StopParticleEffect()
    {
        particleSystem.Stop();
    }
    public void PauseParticleEffect()
    {
        particleSystem.Pause();
    }
    public void ResumeParticleEffect()
    {
        particleSystem.Play();
    }

}

using UnityEngine;
public class EffectManager : MonoBehaviour
{
   [SerializeField] private ParticleSystem landingEffect;

   public void PlayLandingEffect()
   {
      if (landingEffect == null) return;
      
      landingEffect.transform.position = transform.position;
      landingEffect.Stop();
      landingEffect.Play();
      Debug.Log("Landing effect played!");
   }
}

using UnityEngine;
public class EffectManager : MonoBehaviour
{
   [SerializeField] private ParticleSystem dustTrailEffect;

   public void PlayDustTrailEffect(bool play)
   {
      if(dustTrailEffect == null) return;

      if (play && !dustTrailEffect.isPlaying)
      {
         dustTrailEffect.Play();
      }
      else if (!play && dustTrailEffect.isPlaying)
      {
         dustTrailEffect.Stop();
      }
   }
}

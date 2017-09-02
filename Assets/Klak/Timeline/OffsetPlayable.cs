using UnityEngine;
using UnityEngine.Playables;
using Klak.Math;

namespace Klak.Timeline
{
    [System.Serializable]
    public class OffsetPlayable : PlayableBehaviour
    {
        #region Serialized variables

        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;

        public float noiseFrequency = 0.1f;
        public int noiseOctaves = 0;

        public float amplitude = 1;
        public AnimationCurve envelope = AnimationCurve.Linear(0, 1, 1, 1);

        public int randomSeed;

        #endregion

        #region PlayableBehaviour overrides

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var target = playerData as Transform;
            if (target == null) return;

            var time = (float)playable.GetTime();
            var normalizedTime = time / (float)playable.GetDuration();

            // Calculate amplitude. Break if it's nearly zero.
            var amp = info.weight * amplitude * envelope.Evaluate(normalizedTime);
            if (Mathf.Approximately(amp, 0)) return;

            // Noise variables.
            var hash = new XXHash(randomSeed);
            var nt = time * noiseFrequency;
            var norm = 1 / 0.75f;

            // Calculate position offset.
            var p = position;

            if (noiseOctaves > 0)
            {
                var n = new Vector3(
                    Perlin.Fbm(hash.Range(-1e3f, 1e3f, 0) + nt, noiseOctaves),
                    Perlin.Fbm(hash.Range(-1e3f, 1e3f, 1) + nt, noiseOctaves),
                    Perlin.Fbm(hash.Range(-1e3f, 1e3f, 2) + nt, noiseOctaves)
                );
                p = Vector3.Scale(p, n) * norm;
            }

            target.localPosition += p * amp;

            // Calculate rotation offset.
            var r = rotation;

            if (noiseOctaves > 0)
            {
                var n = new Vector3(
                    Perlin.Fbm(hash.Range(-1e3f, 1e3f, 3) + nt, noiseOctaves),
                    Perlin.Fbm(hash.Range(-1e3f, 1e3f, 4) + nt, noiseOctaves),
                    Perlin.Fbm(hash.Range(-1e3f, 1e3f, 5) + nt, noiseOctaves)
                );
                r = Vector3.Scale(r, n) * norm;
            }

            target.localRotation *= Quaternion.Euler(r * amp);
        }

        #endregion
    }
}

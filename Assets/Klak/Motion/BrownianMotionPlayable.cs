using UnityEngine;
using UnityEngine.Playables;
using Klak.Math;

namespace Klak
{
    public class BrownianMotionPlayable : PlayableBehaviour
    {
        #region Construction and configuration

        public static ScriptPlayable<BrownianMotionPlayable>
            Create(PlayableGraph graph, Transform target, int seed)
        {
            var playable = ScriptPlayable<BrownianMotionPlayable>.Create(graph);
            var instance = playable.GetBehaviour();

            instance._target = target;
            instance._hash = new XXHash(seed);

            return playable;
        }

        public void SetPositionNoise(
            float frequency, Vector3 amplitude, int fractalLevel)
        {
            _positionFrequency = frequency;
            _positionAmplitude = amplitude;
            _positionFractalLevel = fractalLevel;
        }

        public void SetRotationNoise(
            float frequency, Vector3 amplitude, int fractalLevel)
        {
            _rotationFrequency = frequency;
            _rotationAmplitude = amplitude;
            _rotationFractalLevel = fractalLevel;
        }

        #endregion

        #region Parameters and variables

        float _positionFrequency;
        float _rotationFrequency;

        Vector3 _positionAmplitude;
        Vector3 _rotationAmplitude;

        int _positionFractalLevel;
        int _rotationFractalLevel;

        Transform _target;
        XXHash _hash;

        #endregion

        #region Playable implementation

        public override void OnGraphStop(Playable playable)
        {
            if (_target == null) return;

            _target.localPosition = Vector3.zero;
            _target.localRotation = Quaternion.identity;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (_target == null) return;

            const float _fbmNorm = 1 / 0.75f;

            var time = (float)playable.GetTime();

            if (_positionAmplitude != Vector3.zero)
            {
                var t = time * _positionFrequency;
                var tx = _hash.Range(-1e3f, 1e3f, 0) + t;
                var ty = _hash.Range(-1e3f, 1e3f, 1) + t;
                var tz = _hash.Range(-1e3f, 1e3f, 2) + t;

                var n = new Vector3(
                    Perlin.Fbm(tx, _positionFractalLevel),
                    Perlin.Fbm(ty, _positionFractalLevel),
                    Perlin.Fbm(tz, _positionFractalLevel)
                );

                n = Vector3.Scale(n, _positionAmplitude) * _fbmNorm;

                _target.localPosition = n;
            }

            if (_rotationAmplitude != Vector3.zero)
            {
                var t = time * _rotationFrequency;
                var tx = _hash.Range(-1e3f, 1e3f, 3) + t;
                var ty = _hash.Range(-1e3f, 1e3f, 4) + t;
                var tz = _hash.Range(-1e3f, 1e3f, 5) + t;

                var n = new Vector3(
                    Perlin.Fbm(tx, _rotationFractalLevel),
                    Perlin.Fbm(ty, _rotationFractalLevel),
                    Perlin.Fbm(tz, _rotationFractalLevel)
                );

                n = Vector3.Scale(n, _rotationAmplitude) * _fbmNorm;

                _target.localRotation = Quaternion.Euler(n);
            }
        }

        #endregion
    }
}

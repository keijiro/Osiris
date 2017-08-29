using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Klak
{
    [System.Serializable]
    public class BrownianMotionPlayableAsset : PlayableAsset, IPropertyPreview
    {
        #region Editable variables

        [SerializeField] ExposedReference<Transform> _target;

        [SerializeField] bool _enablePositionNoise = true;
        [SerializeField] bool _enableRotationNoise = true;

        [SerializeField] float _positionFrequency = 0.2f;
        [SerializeField] float _rotationFrequency = 0.2f;

        [SerializeField] Vector3 _positionAmplitude = Vector3.one;
        [SerializeField] Vector3 _rotationAmplitude = new Vector3(1, 1, 0);

        [SerializeField, Range(0, 8)] int _positionFractalLevel = 3;
        [SerializeField, Range(0, 8)] int _rotationFractalLevel = 3;

        [SerializeField] int _randomSeed;

        #endregion

        #region PlayableAsset implementation

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var targetTransform = _target.Resolve(graph.GetResolver());

            var playable = BrownianMotionPlayable.Create(
                graph, targetTransform, _randomSeed
            );

            var motion = playable.GetBehaviour();

            if (_enablePositionNoise)
                motion.SetPositionNoise(
                    _positionFrequency,
                    _positionAmplitude,
                    _positionFractalLevel
                );

            if (_enableRotationNoise)
                motion.SetRotationNoise(
                    _rotationFrequency,
                    _rotationAmplitude,
                    _rotationFractalLevel
                );

            return playable;
        }

        #endregion

        #region IPropertyPreview implementation

        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            var transform = _target.Resolve(director);
            if (transform == null) return;

            var go = transform.gameObject;
            Debug.Log(go);
            driver.AddFromName<Transform>(go, "m_LocalPosition.x");
            driver.AddFromName<Transform>(go, "m_LocalPosition.y");
            driver.AddFromName<Transform>(go, "m_LocalPosition.z");
            driver.AddFromName<Transform>(go, "m_LocalRotation.x");
            driver.AddFromName<Transform>(go, "m_LocalRotation.y");
            driver.AddFromName<Transform>(go, "m_LocalRotation.z");
        }

        #endregion
    }
}

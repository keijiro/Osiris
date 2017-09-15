using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Klak.Timeline
{
    [System.Serializable]
    class OffsetMixer : PlayableBehaviour
    {
        #region Private state

        Transform _captured;
        Vector3 _defaultPosition;
        Quaternion _defaultRotation;

        #endregion

        #region PlayableBehaviour overrides

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var target = playerData as Transform;
            if (target == null) return;

            if (_captured == null)
            {
                //_defaultPosition = target.localPosition;
                //_defaultRotation = target.localRotation;
                _defaultPosition = Vector3.zero;
                _defaultRotation = Quaternion.identity;
                _captured = target;
            }
            else
            {
                target.localPosition = _defaultPosition;
                target.localRotation = _defaultRotation;
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            if (_captured != null)
            {
                _captured.localPosition = _defaultPosition;
                _captured.localRotation = _defaultRotation;
                _captured = null;
            }
        }

        #endregion
    }
}

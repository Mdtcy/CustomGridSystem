﻿using UnityEngine;

namespace Mdtcy.GridBuildingSystem.Scripts._
{
    public class BuildingAnimation : MonoBehaviour {

        [SerializeField] private AnimationCurve animationCurve = null;

        private float time;

        private void Update() {
            time += Time.deltaTime;

            transform.localScale = new Vector3(1, animationCurve.Evaluate(time), 1);
        }

    }
}

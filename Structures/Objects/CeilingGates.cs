using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreLevelTypesInPlus.Structures.Objects
{
    public class CeilingGates : Door
    {
        public Vector3 cageOffset = new(0, 0, -11.5f);
        public Transform model;
        public float animationSpeed = 9999f;
        public float fastSpeed = 10f;
        public float slowSpeed = 0.5f;

        private static Entity target;
        private static readonly List<CeilingGates> gates = new();

        public override void Initialize()
        {
            base.Initialize();
            ec = Singleton<BaseGameManager>.Instance.ec;
            Open(false, false);
            if (!gates.Contains(this))
                gates.Add(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                Switch();

            AnimateGate();
        }

        private void AnimateGate()
        {
            Vector3 targetPos = (open ? Vector3.up * 10f : Vector3.up) + cageOffset;
            model.localPosition = Vector3.MoveTowards(
                model.localPosition,
                targetPos,
                animationSpeed * ec.EnvironmentTimeScale * Time.deltaTime
            );
        }

        private void OnTriggerEnter(Collider other)
        {
            if (target != null) return;

            if (other.TryGetComponent<Entity>(out Entity entity))
            {
                target = entity;
                StartCoroutine(GateSequence());
            }
        }

        private IEnumerator GateSequence()
        {
            foreach (CeilingGates gate in gates)
            {
                gate.Shut();
                gate.animationSpeed = fastSpeed;
            }

            yield return new WaitForSeconds(5f);

            Open(true, false);
            animationSpeed = fastSpeed;

            yield return new WaitUntil(() =>
                Vector3.Distance(model.localPosition, Vector3.up * 10f + cageOffset) < 0.01f);

            foreach (CeilingGates gate in gates)
            {
                if (gate == this) continue;

                gate.Open(true, false);
                gate.animationSpeed = slowSpeed;

                yield return new WaitUntil(() =>
                    Vector3.Distance(gate.model.localPosition, Vector3.up * 10f + gate.cageOffset) < 0.01f);
            }

            target = null;
        }

        public void Switch()
        {
            if (open)
                Shut();
            else
                Open(true, false);
        }

        public override void Shut()
        {
            base.Shut();
        }

        public override void Open(bool cancelTimer, bool makeNoise)
        {
            base.Open(cancelTimer, makeNoise);
        }
    }
}

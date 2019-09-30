using Assets.Scripts;
using HoloToolkit.Unity.InputModule;
using System;
using System.Linq;
using UnityEngine;

namespace HelloHolo
{
    public class ShootHolo : MonoBehaviour, IInputClickHandler, IShootHolo
    {
        private Transform camTransform;
        private int mask;
        private event Action<RaycastHit> hitAction;

        public event System.Action<RaycastHit> HitAction
        {
            add { if (hitAction == null || !hitAction.GetInvocationList().Contains(value)) hitAction += value; }
            remove { hitAction -= value; }
        }

        private event System.Action shotHappened;
        public event System.Action ShotHappened
        {
            add { if (shotHappened == null || !shotHappened.GetInvocationList().Contains(value)) shotHappened += value; }
            remove { shotHappened -= value; }
        }

        private void Start()
        {
            camTransform = Camera.main.transform;
        }

        private void OnShot()
        {
            if (hitAction != null)
            {
                RaycastHit[] hits = Physics.RaycastAll(camTransform.position,
                   camTransform.forward, float.PositiveInfinity, mask);

                if (hits != null && hits.Length != 0)
                {
                    hitAction.Invoke(hits.OrderBy(x => x.distance).FirstOrDefault());
                }
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            OnShot();
        }

        public void ChangeLayer(int mask)
        {
            this.mask = mask;
        }
    }

    internal interface IShootHolo
    {
        void ChangeLayer(int mask);
        event System.Action<RaycastHit> HitAction;
    }
}


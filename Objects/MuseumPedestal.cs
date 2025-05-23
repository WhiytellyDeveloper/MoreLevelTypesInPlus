using MTM101BaldAPI.Registers;
using System;
using UnityEngine;

namespace MoreLevelTypesInPlus.Objects
{
    public class MuseumPedestal : MonoBehaviour
    {
        public RoomController room;
        public Notebook notebook;
        public Pickup pickup;
        public bool initialized;
        public float test = 0;
        private MaterialPropertyBlock _propertyBlock;

        private void Start()
        {
            Singleton<BaseGameManager>.Instance.AddNotebookTotal(-1);
        }

        private void Update()
        {
            SetSpriteVisibility();
            room = Singleton<BaseGameManager>.Instance.Ec.CellFromPosition(transform.position).room;

            if (pickup != null && !notebook.collected)
            {
                if (pickup.item.GetMeta().tags.Contains("Useless"))
                {
                    notebook.Hide(false);
                    pickup.gameObject.SetActive(false);
                }
            }

            if (room.category == RoomCategory.Null)
                return;

            if (initialized)
                return;

            Vector2 pedestalXZ = new(transform.position.x, transform.position.z);
            pickup = Singleton<BaseGameManager>.Instance.Ec.CreateItem(room, Singleton<CoreGameManager>.Instance.NoneItem, pedestalXZ);
            pickup.survivePickup = true;

            var activity = room.activity;
            if (activity?.notebook == null) return;

            activity.notebook.transform.localPosition = new(transform.localPosition.x, 5f, transform.localPosition.z);
            activity.notebook.Hide(true);
            notebook = activity.notebook;
            initialized = true;
        }

        private void SetSpriteVisibility()
        {
            if (this._propertyBlock == null)
            {
                this._propertyBlock = new MaterialPropertyBlock();
            }
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.GetPropertyBlock(this._propertyBlock);
                this._propertyBlock.SetFloat("_PercentInvisible", test);
                renderer.SetPropertyBlock(this._propertyBlock);
            }
        }
    }
}

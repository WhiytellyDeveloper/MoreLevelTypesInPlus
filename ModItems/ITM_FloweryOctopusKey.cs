using MoreLevelTypesInPlus.LevelTypes_SpecialManagers;
using MTM101BaldAPI;
using UnityEngine;

namespace MoreLevelTypesInPlus.ModItems
{
    public class ITM_FloweryOctopusKey : Item
    {
        public override bool Use(PlayerManager pm)
        {
            if (Singleton<LevelTypeManager_Base>.Instance.GetComponent<LevelTypeManager_Castle>().keys == 0)
            {
                Destroy(base.gameObject);
                return false;
            }

            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out this.hit, pm.pc.reach, this.layerMask.mask))
            {
                foreach (IItemAcceptor itemAcceptor in this.hit.transform.GetComponents<IItemAcceptor>())
                {
                    if (itemAcceptor != null && itemAcceptor.ItemFits(EnumExtensions.GetFromExtendedName<Items>("ClassKey")))
                    {
                        itemAcceptor.InsertItem(pm, pm.ec);
                        Destroy(base.gameObject);

                        if (audUse != null)  Singleton<CoreGameManager>.Instance.audMan.PlaySingle(this.audUse);

                        Singleton<LevelTypeManager_Base>.Instance.GetComponent<LevelTypeManager_Castle>().keys--;
                    }
                }
            }
            Object.Destroy(base.gameObject);
            return false;
        }

        private RaycastHit hit;
        public SoundObject audUse;
        public LayerMaskObject layerMask;
    }
}

using MoreLevelTypesInPlus.LevelTypes_SpecialManagers;

namespace MoreLevelTypesInPlus.ModItems
{
    public class ITM_FloweryOctopusKeyPickup : Item
    {
        public override bool Use(PlayerManager pm)
        {
            Singleton<LevelTypeManager_Base>.Instance.GetComponent<LevelTypeManager_Castle>().keys++;
            Destroy(gameObject);
            return true;
        }
    }
}

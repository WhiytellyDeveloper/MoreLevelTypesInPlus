using MoreLevelTypesInPlus.LevelTypes_Constructor;

namespace MoreLevelTypesInPlus.LevelTypes_SpecialManagers
{
    public class LevelTypeManager_Museum : LevelTypeManager_Base
    {
        public override void ActivityCompleted(bool correct, Activity act)
        {
            base.ActivityCompleted(correct, act);
            act.notebook.Hide(true);

            var pickup = act.room.ec.CreateItem(act.room, LevelType_Museum.instance.fakeNotebook, new());
            pickup.transform.localPosition = act.notebook.transform.localPosition;

            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.AddStamina(Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.staminaMax, true);
        }
    }
}

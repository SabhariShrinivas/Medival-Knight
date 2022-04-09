using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction CurrentAction;

        public void StartAction(IAction action)
        {
            if (CurrentAction == action) return;
            if (CurrentAction != null)
                CurrentAction.Cancel();
            CurrentAction = action;
        }
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}

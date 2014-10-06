using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndexerWpf
{
    public class BackgroundGuiAction
    {
        public List<Action> ContinueWith { get; private set; }

        public Action Action { get; set; }

        public BackgroundGuiAction()
        {
            ContinueWith = new List<Action>();
        }

        public void Start()
        {
            var action = Action;
            if (action == null)
                return;

            var task = new Task(() =>
            {
                action();
                Continue();
            });
            task.Start();
        }

        void Continue()
        {
            foreach (var action in ContinueWith)
            {
                App.Current.Dispatcher.Invoke(action);
            }
        }
    }
}

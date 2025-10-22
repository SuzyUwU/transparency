using System;

namespace Models
{
    public class WindowHandler
    {
        private readonly Action<string> updateUiAction;

        public WindowHandler(Action<string> updateUi)
        {
            updateUiAction = updateUi;
        }

        public void OnWindowSelected(string windowName)
        {
            string message = $"{windowName} selected";
            updateUiAction.Invoke(message);
        }
    }
}

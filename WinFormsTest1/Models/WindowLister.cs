using System;

namespace Models
{
    public class WindowListener
    {
        public void OnWindowSelected(string windowName)
        {
            Console.WriteLine($"{windowName} selected");
        }
    }
}

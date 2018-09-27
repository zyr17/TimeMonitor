using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMonitor
{
    class Hook
    {
        static long lastMove = 0;
        public static void Start()
        {
            MouseHook.AddEvent(MouseEvent);
            KeyboardHook.AddEvent(KeyboardEvent);
            MouseHook.Start();
            KeyboardHook.Start();
        }

        public static void Stop()
        {
            MouseHook.Stop();
            KeyboardHook.Stop();
        }

        private static void KeyboardEvent(ref KeyboardHook.StateKeyboard state)
        {
            if (DateTime.Now.Ticks < lastMove + Consts.HookTimeSpan.Ticks) return;
            lastMove = DateTime.Now.Ticks;
            Data.Actions actions = new Data.Actions
            {
                DateTime = DateTime.Now,
                Type = 1,
                Action = "Keyboard",
                Title = state.Key.ToString()
            };
            Data.AddActions(actions);
        }

        private static void MouseEvent(ref MouseHook.StateMouse state)
        {
            if (DateTime.Now.Ticks < lastMove + Consts.HookTimeSpan.Ticks) return;
            lastMove = DateTime.Now.Ticks;
            Data.Actions actions = new Data.Actions
            {
                DateTime = DateTime.Now,
                Type = 1,
                Action = "Mouse",
                Title = state.button + " " + state.posX + " " + state.posY
            };
            Data.AddActions(actions);
        }
    }
}

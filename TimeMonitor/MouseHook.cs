using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMonitor
{
    public static class MouseHook
    {
        private static class NativeMethods
        {
            /// <summary>
            /// マウスに関するウィンドウメッセージ
            /// </summary>
            public enum Message
            {
                WM_MOUSEMOVE = 0x0200,
                WM_LBUTTONDOWN = 0x0201,
                WM_LBUTTONUP = 0x0202,
                WM_LBUTTONDBLCLK = 0x0203,
                WM_RBUTTONDOWN = 0x0204,
                WM_RBUTTONUP = 0x0205,
                WM_MBUTTONDOWN = 0x0207,
                WM_MBUTTONUP = 0x0208,
                WM_MOUSEWHEEL = 0x020A,
                WM_XBUTTONDOWN = 0x20B,
                WM_XBUTTONUP = 0x20C
            }

            /// <summary>
            /// x 座標と y 軸座標の構造体
            /// </summary>
            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public struct POINT
            {
                public int x;
                public int y;
            }

            /// <summary>
            /// 低レベルのマウスの入力イベントの構造体
            /// </summary>
            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public struct MSLLHOOKSTRUCT
            {
                public POINT pt;
                public uint mouseData;
                public uint flags;
                public uint time;
                public System.IntPtr dwExtraInfo;
            }

            /// <summary>
            /// フックプロシージャのデリゲート
            /// </summary>
            /// <param name="nCode">フックプロシージャに渡すフックコード</param>
            /// <param name="msg">フックプロシージャに渡す値</param>
            /// <param name="msllhookstruct">フックプロシージャに渡す値</param>
            /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
            public delegate System.IntPtr MouseHookCallback(int nCode, NativeMethods.Message msg, ref MSLLHOOKSTRUCT msllhookstruct);

            /// <summary>
            /// アプリケーション定義のフックプロシージャをフックチェーン内にインストールします。
            /// フックプロシージャをインストールすると、特定のイベントタイプを監視できます。
            /// 監視の対象になるイベントは、特定のスレッド、または呼び出し側スレッドと同じデスクトップ内のすべてのスレッドに関連付けられているものです。
            /// </summary>
            /// <param name="idHook">フックタイプ</param>
            /// <param name="lpfn">フックプロシージャ</param>
            /// <param name="hMod">アプリケーションインスタンスのハンドル</param>
            /// <param name="dwThreadId">スレッドの識別子</param>
            /// <returns>関数が成功すると、フックプロシージャのハンドルが返ります。関数が失敗すると、NULL が返ります。</returns>
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr SetWindowsHookEx(int idHook, MouseHookCallback lpfn, System.IntPtr hMod, uint dwThreadId);

            /// <summary>
            /// SetWindowsHookEx 関数を使ってフックチェーン内にインストールされたフックプロシージャを削除します。
            /// </summary>
            /// <param name="hhk">削除対象のフックプロシージャのハンドル</param>
            /// <returns>関数が成功すると、0 以外の値が返ります。関数が失敗すると、0 が返ります。</returns>
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(System.IntPtr hhk);

            /// <summary>
            /// 現在のフックチェーン内の次のフックプロシージャに、フック情報を渡します。
            /// フックプロシージャは、フック情報を処理する前でも、フック情報を処理した後でも、この関数を呼び出せます。
            /// </summary>
            /// <param name="hhk">現在のフックのハンドル</param>
            /// <param name="nCode">フックプロシージャに渡すフックコード</param>
            /// <param name="msg">フックプロシージャに渡す値</param>
            /// <param name="msllhookstruct">フックプロシージャに渡す値</param>
            /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr CallNextHookEx(System.IntPtr hhk, int nCode, NativeMethods.Message msg, ref MSLLHOOKSTRUCT msllhookstruct);
        }

        /// <summary>
        /// マウスの状態の構造体
        /// </summary>
        public struct StateMouse
        {
            public ButtonType button;
            public int posX;
            public int posY;
            public uint mouseData;
            public uint flags;
            public uint time;
            public System.IntPtr dwExtraInfo;
        }

        /// <summary>
        /// マウスのボタンの列挙型
        /// </summary>
        public enum ButtonType
        {
            MOVE,
            L_DOWN,
            L_UP,
            R_DOWN,
            R_UP,
            M_DOWN,
            M_UP,
            W_DOWN,
            W_UP,
            X1_DOWN,
            X1_UP,
            X2_DOWN,
            X2_UP,
            UNKNOWN
        }


        /// <summary>
        /// マウスのグローバルフックを実行しているかどうかを取得します。
        /// </summary>
        public static bool IsHooking
        {
            get;
            private set;
        }

        /// <summary>
        /// マウスの状態を取得、設定します。
        /// </summary>
        public static StateMouse State;

        /// <summary>
        /// フックプロシージャ内でのイベント用のデリゲート
        /// </summary>
        /// <param name="msg">マウスに関するウィンドウメッセージ</param>
        /// <param name="msllhookstruct">低レベルのマウスの入力イベントの構造体</param>
        public delegate void HookHandler(ref StateMouse state);

        /// <summary>
        /// フックプロシージャのハンドル
        /// </summary>
        private static System.IntPtr Handle;

        /// <summary>
        /// 入力をキャンセルするかどうかを取得、設定します。
        /// </summary>
        private static bool IsCancel;

        /// <summary>
        /// 登録イベントのリストを取得、設定します。
        /// </summary>
        private static System.Collections.Generic.HashSet<HookHandler> Events;

        /// <summary>
        /// フックプロシージャ内でのイベント
        /// </summary>
        private static event HookHandler HookEvent;

        /// <summary>
        /// フックチェーンにインストールするフックプロシージャのイベント
        /// </summary>
        private static event NativeMethods.MouseHookCallback HookCallback;

        /// <summary>
        /// フックプロシージャをフックチェーン内にインストールし、マウスのグローバルフックを開始します。
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public static void Start()
        {
            if (IsHooking)
            {
                return;
            }

            HookCallback = HookProcedure;
            System.IntPtr h = System.Runtime.InteropServices.Marshal.GetHINSTANCE(typeof(MouseHook).Assembly.GetModules()[0]);
            Handle = NativeMethods.SetWindowsHookEx(14, HookCallback, h, 0);

            if (Handle == System.IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception();
            }

            IsHooking = true;
        }

        /// <summary>
        /// マウスのグローバルフックを停止し、フックプロシージャをフックチェーン内から削除します。
        /// </summary>
        public static void Stop()
        {
            if (!IsHooking)
            {
                return;
            }

            ClearEvent();

            if (Handle != System.IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(Handle);
                Handle = System.IntPtr.Zero;
                HookCallback -= HookProcedure;
            }

            IsHooking = false;
        }

        /// <summary>
        /// 次のフックプロシージャにフック情報を渡すのをキャンセルします。
        /// </summary>
        public static void Cancel()
        {
            IsCancel = true;
        }

        /// <summary>
        /// マウス操作時のイベントを追加します。
        /// </summary>
        /// <param name="hookHandler"></param>
        public static void AddEvent(HookHandler hookHandler)
        {
            if (Events == null)
            {
                Events = new System.Collections.Generic.HashSet<HookHandler>();
            }

            if (Events.Add(hookHandler))
            {
                HookEvent += hookHandler;
            }
        }

        /// <summary>
        /// マウス操作時のイベントを削除します。
        /// </summary>
        /// <param name="hookHandler"></param>
        public static void RemoveEvent(HookHandler hookHandler)
        {
            HookEvent -= hookHandler;
            Events.Remove(hookHandler);
        }

        /// <summary>
        /// マウス操作時のイベントを全て削除します。
        /// </summary>
        public static void ClearEvent()
        {
            foreach (HookHandler e in Events)
            {
                HookEvent -= e;
            }

            Events.Clear();
        }

        /// <summary>
        /// フックチェーンにインストールするフックプロシージャ
        /// </summary>
        /// <param name="nCode">フックプロシージャに渡すフックコード</param>
        /// <param name="msg">フックプロシージャに渡す値</param>
        /// <param name="msllhookstruct">フックプロシージャに渡す値</param>
        /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
        private static System.IntPtr HookProcedure(int nCode, NativeMethods.Message msg, ref NativeMethods.MSLLHOOKSTRUCT s)
        {
            if (nCode >= 0 && HookEvent != null)
            {
                State.button = GetButtonState(msg, ref s);
                State.posX = s.pt.x;
                State.posY = s.pt.y;
                State.mouseData = s.mouseData;
                State.flags = s.flags;
                State.time = s.time;
                State.dwExtraInfo = s.dwExtraInfo;

                HookEvent(ref State);

                if (IsCancel)
                {
                    IsCancel = false;
                    return (System.IntPtr)1;
                }
            }

            return NativeMethods.CallNextHookEx(Handle, nCode, msg, ref s);
        }

        /// <summary>
        /// 操作されたボタンを取得します。
        /// </summary>
        /// <param name="msg">マウスに関するウィンドウメッセージ</param>
        /// <param name="s">低レベルのマウスの入力イベントの構造体</param>
        /// <returns></returns>
        private static ButtonType GetButtonState(NativeMethods.Message msg, ref NativeMethods.MSLLHOOKSTRUCT s)
        {
            switch (msg)
            {
                case NativeMethods.Message.WM_MOUSEMOVE:
                    return ButtonType.MOVE;
                case NativeMethods.Message.WM_LBUTTONDOWN:
                    return ButtonType.L_DOWN;
                case NativeMethods.Message.WM_LBUTTONUP:
                    return ButtonType.L_UP;
                case NativeMethods.Message.WM_LBUTTONDBLCLK:
                    return ButtonType.UNKNOWN;
                case NativeMethods.Message.WM_RBUTTONDOWN:
                    return ButtonType.R_DOWN;
                case NativeMethods.Message.WM_RBUTTONUP:
                    return ButtonType.R_UP;
                case NativeMethods.Message.WM_MBUTTONDOWN:
                    return ButtonType.M_DOWN;
                case NativeMethods.Message.WM_MBUTTONUP:
                    return ButtonType.M_UP;
                case NativeMethods.Message.WM_MOUSEWHEEL:
                    return ((short)((s.mouseData >> 16) & 0xffff) > 0) ? ButtonType.W_UP : ButtonType.W_DOWN;
                case NativeMethods.Message.WM_XBUTTONDOWN:
                    switch (s.mouseData >> 16)
                    {
                        case 1:
                            return ButtonType.X1_DOWN;
                        case 2:
                            return ButtonType.X2_DOWN;
                        default:
                            return ButtonType.UNKNOWN;
                    }
                case NativeMethods.Message.WM_XBUTTONUP:
                    switch (s.mouseData >> 16)
                    {
                        case 1:
                            return ButtonType.X1_UP;
                        case 2:
                            return ButtonType.X2_UP;
                        default:
                            return ButtonType.UNKNOWN;
                    }
                default:
                    return ButtonType.UNKNOWN;
            }
        }
    }
}

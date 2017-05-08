#if UNITY_METRO && !UNITY_EDITOR

using System;

namespace System.Runtime.InteropServices
{
    [ComVisible (true)]
    public struct HandleRef
    {
        object wrapper;
        IntPtr handle;

        public HandleRef (object wrapper, IntPtr handle)
        {
            this.wrapper = wrapper;
            this.handle = handle;
        }

        public IntPtr Handle
        {
            get { return handle; }
        }

        public object Wrapper
        {
            get { return wrapper; }
        }

        public static explicit operator IntPtr (HandleRef value)
        {
            return value.Handle;
        }

        public static IntPtr ToIntPtr(HandleRef value)
        {
            return value.Handle;
        }
    }
}

#endif
namespace DragDropLib
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    [ComVisible(true)]
    [ComImport]
    [Guid("DE5BF786-477A-11D2-839D-00C04FD918D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDragSourceHelper
    {
        void InitializeFromBitmap(
            [In, MarshalAs(UnmanagedType.Struct)] ref ShDragImage dragImage,
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);

        void InitializeFromWindow(
            [In] IntPtr hwnd,
            [In] ref Win32Point pt,
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);
    }

    [ComVisible(true)]
    [ComImport]
    [Guid("83E07D0D-0C5F-4163-BF1A-60B274051E40")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDragSourceHelper2
    {
        void InitializeFromBitmap(
            [In, MarshalAs(UnmanagedType.Struct)] ref ShDragImage dragImage,
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);

        void InitializeFromWindow(
            [In] IntPtr hwnd,
            [In] ref Win32Point pt,
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);

        void SetFlags(
            [In] int dwFlags);
    }
}

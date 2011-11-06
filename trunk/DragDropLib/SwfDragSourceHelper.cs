namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using DragDropLib;
    using ComTypes = System.Runtime.InteropServices.ComTypes;

    /// <summary>
    /// Provides helper methods for working with the Shell drag image manager.
    /// </summary>
    public static class DragSourceHelper
    {
        #region DLL imports

        [DllImport("user32.dll")]
        private static extern void PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #endregion // DLL imports

        #region Native constants

        // This is WM_USER + 3. The window controlled by the drag image manager
        // looks for this message (WM_USER + 2 seems to work, too) to invalidate
        // the drag image.
        private const uint WM_INVALIDATEDRAGIMAGE = 0x403;

        // CFSTR_DROPDESCRIPTION
        private const string DropDescriptionFormat = "DropDescription";

        // The drag image manager sets this flag to indicate if the current
        // drop target supports drag images.
        private const string IsShowingLayeredFormat = "IsShowingLayered";

        #endregion // Native constants

        /// <summary>
        /// Internally used to track information about the current drop description.
        /// </summary>
        [Flags]
        private enum DropDescriptionFlags
        {
            None = 0,
            IsDefault = 1,
            InvalidateRequired = 2
        }

        /// <summary>
        /// Keeps a cached drag source context, keyed on the drag source control.
        /// </summary>
        private static IDictionary<Control, DragSourceEntry> s_dataContext = new Dictionary<Control, DragSourceEntry>();

        /// <summary>
        /// Represents a drag source context entry.
        /// </summary>
        private class DragSourceEntry
        {
            public IDataObject data;
            public int adviseConnection = 0;

            public DragSourceEntry(IDataObject data)
            {
                this.data = data;
            }
        }

        /// <summary>
        /// Keeps drop description info for a data object.
        /// </summary>
        private static IDictionary<IDataObject, DropDescriptionFlags> s_dropDescriptions = new Dictionary<IDataObject, DropDescriptionFlags>();

        /// <summary>
        /// Creates a default DataObject with an internal COM callable implemetation of IDataObject.
        /// </summary>
        /// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
        public static IDataObject CreateDataObject()
        {
            return new System.Windows.Forms.DataObject(new DragDropLib.DataObject());
        }

        /// <summary>
        /// Creates a DataObject with an internal COM callable implementation of IDataObject.
        /// This override also sets the drag image to the specified Bitmap and sets a flag
        /// on the system IDragSourceHelper2 to allow drop descriptions.
        /// </summary>
        /// <param name="dragImage">A Bitmap from which to create the drag image.</param>
        /// <param name="cursorOffset">The drag image cursor offset.</param>
        /// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
        public static IDataObject CreateDataObject(Bitmap dragImage, System.Drawing.Point cursorOffset)
        {
            IDataObject data = CreateDataObject();
            AllowDropDescription(true);
            data.SetDragImage(dragImage, cursorOffset);
            return data;
        }

        /// <summary>
        /// Creates a DataObject with an internal COM callable implementation of IDataObject.
        /// This override also sets the drag image to a bitmap created from the specified
        /// Control instance's UI. It also sets a flag on the system IDragSourceHelper2 to
        /// allow drop descriptions.
        /// </summary>
        /// <param name="control">A Control to initialize the drag image from.</param>
        /// <param name="cursorOffset">The drag image cursor offset.</param>
        /// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
        public static IDataObject CreateDataObject(Control control, System.Drawing.Point cursorOffset)
        {
            IDataObject data = CreateDataObject();
            AllowDropDescription(true);
            data.SetDragImage(control, cursorOffset);
            return data;
        }

        /// <summary>
        /// Registers a Control as a drag source and provides default implementations of
        /// GiveFeedback and QueryContinueDrag.
        /// </summary>
        /// <param name="control">The drag source Control instance.</param>
        /// <param name="data">The DataObject associated to the drag source.</param>
        /// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
        /// operation is complete to avoid memory leaks.</remarks>
        public static void RegisterDefaultDragSource(Control control, IDataObject data)
        {
            // Cache the drag source and the associated data object
            DragSourceEntry entry = new DragSourceEntry(data);
            if (!s_dataContext.ContainsKey(control))
                s_dataContext.Add(control, entry);
            else
                s_dataContext[control] = entry;

            // We need to listen for drop description changes. If a drop target
            // changes the drop description, we shouldn't provide a default one.
            entry.adviseConnection = ComTypes.ComDataObjectExtensions.Advise(((ComTypes.IDataObject)data), new AdviseSink(data), DropDescriptionFormat, 0);

            // Hook up the default drag source event handlers
            control.GiveFeedback += new GiveFeedbackEventHandler(DefaultGiveFeedbackHandler);
            control.QueryContinueDrag += new QueryContinueDragEventHandler(DefaultQueryContinueDragHandler);
        }

        /// <summary>
        /// Registers a Control as a drag source and provides default implementations of
        /// GiveFeedback and QueryContinueDrag. This override also handles the data object
        /// creation, including initialization of the drag image from the Control.
        /// </summary>
        /// <param name="control">The drag source Control instance.</param>
        /// <param name="cursorOffset">The drag image cursor offset.</param>
        /// <returns>The created data object.</returns>
        /// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
        /// operation is complete to avoid memory leaks.</remarks>
        public static IDataObject RegisterDefaultDragSource(Control control, System.Drawing.Point cursorOffset)
        {
            IDataObject data = CreateDataObject(control, cursorOffset);
            RegisterDefaultDragSource(control, data);
            return data;
        }

        /// <summary>
        /// Registers a Control as a drag source and provides default implementations of
        /// GiveFeedback and QueryContinueDrag. This override also handles the data object
        /// creation, including initialization of the drag image from the speicified Bitmap.
        /// </summary>
        /// <param name="control">The drag source Control instance.</param>
        /// <param name="dragImage">A Bitmap to initialize the drag image from.</param>
        /// <param name="cursorOffset">The drag image cursor offset.</param>
        /// <returns>The created data object.</returns>
        /// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
        /// operation is complete to avoid memory leaks.</remarks>
        public static IDataObject RegisterDefaultDragSource(Control control, Bitmap dragImage, System.Drawing.Point cursorOffset)
        {
            IDataObject data = CreateDataObject(dragImage, cursorOffset);
            RegisterDefaultDragSource(control, data);
            return data;
        }

        /// <summary>
        /// Unregisters a drag source from the internal cache.
        /// </summary>
        /// <param name="control">The drag source Control.</param>
        public static void UnregisterDefaultDragSource(Control control)
        {
            if (s_dataContext.ContainsKey(control))
            {
                DragSourceEntry entry = s_dataContext[control];
                ComTypes.IDataObject dataObjectCOM = (ComTypes.IDataObject)entry.data;

                // Stop listening to drop description changes
                dataObjectCOM.DUnadvise(entry.adviseConnection);

                // Unhook the default drag source event handlers
                control.GiveFeedback -= new GiveFeedbackEventHandler(DefaultGiveFeedbackHandler);
                control.QueryContinueDrag -= new QueryContinueDragEventHandler(DefaultQueryContinueDragHandler);

                // Remove the entries from our context caches
                s_dataContext.Remove(control);
                s_dropDescriptions.Remove(entry.data);
            }
        }

        /// <summary>
        /// Performs a default drag and drop operation for the specified drag source.
        /// </summary>
        /// <param name="control">The drag source Control.</param>
        /// <param name="cursorOffset">The drag image cursor offset.</param>
        /// <param name="allowedEffects">The allowed drop effects.</param>
        /// <param name="data">The associated data.</param>
        /// <returns>The accepted drop effects from the completed operation.</returns>
        public static DragDropEffects DoDragDrop(Control control, System.Drawing.Point cursorOffset, DragDropEffects allowedEffects, params KeyValuePair<string, object>[] data)
        {
            IDataObject dataObject = RegisterDefaultDragSource(control, cursorOffset);
            return DoDragDropInternal(control, dataObject, allowedEffects, data);
        }

        /// <summary>
        /// Performs a default drag and drop operation for the specified drag source.
        /// </summary>
        /// <param name="control">The drag source Control.</param>
        /// <param name="dragImage">The Bitmap to initialize the drag image from.</param>
        /// <param name="cursorOffset">The drag image cursor offset.</param>
        /// <param name="allowedEffects">The allowed drop effects.</param>
        /// <param name="data">The associated data.</param>
        /// <returns>The accepted drop effects from the completed operation.</returns>
        public static DragDropEffects DoDragDrop(Control control, Bitmap dragImage, System.Drawing.Point cursorOffset, DragDropEffects allowedEffects, params KeyValuePair<string, object>[] data)
        {
            IDataObject dataObject = RegisterDefaultDragSource(control, dragImage, cursorOffset);
            return DoDragDropInternal(control, dataObject, allowedEffects, data);
        }

        /// <summary>
        /// Performs a default drag and drop operation for the specified drag source.
        /// </summary>
        /// <param name="control">The drag source Control.</param>
        /// <param name="dataObject">The data object associated to the drag and drop operation.</param>
        /// <param name="allowedEffects">The allowed drop effects.</param>
        /// <param name="data">The associated data.</param>
        /// <returns>The accepted drop effects from the completed operation.</returns>
        private static DragDropEffects DoDragDropInternal(Control control, IDataObject dataObject, DragDropEffects allowedEffects, KeyValuePair<string, object>[] data)
        {
            // Set the data onto the data object.
            if (data != null)
            {
                foreach (KeyValuePair<string, object> dataPair in data)
                    dataObject.SetDataEx(dataPair.Key, dataPair.Value);
            }

            try
            {
                return control.DoDragDrop(dataObject, allowedEffects);
            }
            finally
            {
                UnregisterDefaultDragSource(control);
            }
        }

        /// <summary>
        /// Provides a default GiveFeedback event handler for drag sources.
        /// </summary>
        /// <param name="sender">The object that raised the event. Should be set to the drag source.</param>
        /// <param name="e">The event arguments.</param>
        public static void DefaultGiveFeedbackHandler(object sender, GiveFeedbackEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                if (s_dataContext.ContainsKey(control))
                {
                    DefaultGiveFeedback(s_dataContext[control].data, e);
                }
            }
        }

        /// <summary>
        /// Provides a default GiveFeedback event handler for drag sources.
        /// </summary>
        /// <param name="data">The associated data object for the event.</param>
        /// <param name="e">The event arguments.</param>
        public static void DefaultGiveFeedback(IDataObject data, GiveFeedbackEventArgs e)
        {
            // For drop targets that don't set the drop description, we'll
            // set a default one. Drop targets that do set drop descriptions
            // should set an invalid drop description during DragLeave.
            bool setDefaultDropDesc = false;
            bool isDefaultDropDesc = IsDropDescriptionDefault(data);
            DropImageType currentType = DropImageType.Invalid;
            if (!IsDropDescriptionValid(data) || isDefaultDropDesc)
            {
                currentType = GetDropImageType(data);
                setDefaultDropDesc = true;
            }

            if (IsShowingLayered(data))
            {
                // The default drag source implementation uses drop descriptions,
                // so we won't use default cursors.
                e.UseDefaultCursors = false;
                Cursor.Current = Cursors.Arrow;
            }
            else
                e.UseDefaultCursors = true;

            // We need to invalidate the drag image to refresh the drop description.
            // This is tricky to implement correctly, but we try to mimic the Windows
            // Explorer behavior. We internally use a flag to tell us to invalidate
            // the drag image, so if that is set, we'll invalidate. Otherwise, we
            // always invalidate if the drop description was set by the drop target,
            // *or* if the current drop image is not None. So if we set a default
            // drop description to anything but None, we'll always invalidate.
            if (InvalidateRequired(data) || !isDefaultDropDesc || currentType != DropImageType.None)
            {
                InvalidateDragImage(data);

                // The invalidate required flag only lasts for one invalidation
                SetInvalidateRequired(data, false);
            }

            // If the drop description is currently invalid, or if it is a default
            // drop description already, we should check about re-setting it.
            if (setDefaultDropDesc)
            {
                // Only change if the effect changed
                if ((DropImageType)e.Effect != currentType)
                {
                    if (e.Effect == DragDropEffects.Copy)
                        data.SetDropDescription(DropImageType.Copy, "Copy", "");
                    else if (e.Effect == DragDropEffects.Link)
                        data.SetDropDescription(DropImageType.Link, "Link", "");
                    else if (e.Effect == DragDropEffects.Move)
                        data.SetDropDescription(DropImageType.Move, "Move", "");
                    else if (e.Effect == DragDropEffects.None)
                        data.SetDropDescription(DropImageType.None, null, null);
                    SetDropDescriptionIsDefault(data, true);

                    // We can't invalidate now, because the drag image manager won't
                    // pick it up... so we set this flag to invalidate on the next
                    // GiveFeedback event.
                    SetInvalidateRequired(data, true);
                }
            }
        }

        /// <summary>
        /// Provides a default handler for the QueryContinueDrag drag source event.
        /// </summary>
        /// <param name="sender">The object that raised the event. Not used internally.</param>
        /// <param name="e">The event arguments.</param>
        public static void DefaultQueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
        {
            DefaultQueryContinueDrag(e);
        }

        /// <summary>
        /// Provides a default handler for the QueryContinueDrag drag source event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public static void DefaultQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
            }
        }

        /// <summary>
        /// Sets a flag on the system IDragSourceHelper2 object to allow drop descriptions
        /// on the drag image.
        /// </summary>
        /// <param name="allow">True to allow drop descriptions, otherwise False.</param>
        /// <remarks>Must be called before IDragSourceHelper.InitializeFromBitmap or
        /// IDragSourceHelper.InitializeFromControl is called.</remarks>
        public static void AllowDropDescription(bool allow)
        {
            IDragSourceHelper2 sourceHelper = (IDragSourceHelper2)new DragDropHelper();
            sourceHelper.SetFlags(allow ? 1 : 0);
        }

        /// <summary>
        /// Invalidates the drag image.
        /// </summary>
        /// <param name="dataObject">The data object for which to invalidate the drag image.</param>
        /// <remarks>This call tells the drag image manager to reformat the internal
        /// cached drag image, based on the already set drag image bitmap and current drop
        /// description.</remarks>
        public static void InvalidateDragImage(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent("DragWindow"))
            {
                IntPtr hwnd = GetIntPtrFromData(dataObject.GetData("DragWindow"));
                PostMessage(hwnd, WM_INVALIDATEDRAGIMAGE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #region Helper methods

        /// <summary>
        /// Gets an IntPtr from data acquired from a data object.
        /// </summary>
        /// <param name="data">The data that contains the IntPtr.</param>
        /// <returns>An IntPtr.</returns>
        private static IntPtr GetIntPtrFromData(object data)
        {
            byte[] buf = null;

            if (data is MemoryStream)
            {
                buf = new byte[4];
                if (4 != ((MemoryStream)data).Read(buf, 0, 4))
                    throw new ArgumentException("Could not read an IntPtr from the MemoryStream");
            }
            if (data is byte[])
            {
                buf = (byte[])data;
                if (buf.Length < 4)
                    throw new ArgumentException("Could not read an IntPtr from the byte array");
            }

            if (buf == null)
                throw new ArgumentException("Could not read an IntPtr from the " + data.GetType().ToString());

            int p = (buf[3] << 24) | (buf[2] << 16) | (buf[1] << 8) | buf[0];
            return new IntPtr(p);
        }

        /// <summary>
        /// Determines if the IsShowingLayered flag is set on the data object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>True if the flag is set, otherwise false.</returns>
        private static bool IsShowingLayered(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(IsShowingLayeredFormat))
            {
                object data = dataObject.GetData(IsShowingLayeredFormat);
                if (data != null)
                    return GetBooleanFromData(data);
            }

            return false;
        }

        /// <summary>
        /// Converts compatible clipboard data to a boolean value.
        /// </summary>
        /// <param name="data">The clipboard data.</param>
        /// <returns>True if the data can be converted to a boolean and is set, otherwise False.</returns>
        private static bool GetBooleanFromData(object data)
        {
            if (data is Stream)
            {
                Stream stream = data as Stream;
                BinaryReader reader = new BinaryReader(stream);
                return reader.ReadBoolean();
            }
            
            // Anything else isn't supported for now
            return false;
        }

        /// <summary>
        /// Checks if the current drop description, if any, is valid.
        /// </summary>
        /// <param name="dataObject">The DataObject from which to get the drop description.</param>
        /// <returns>True if the drop description is set, and the 
        /// DropImageType is not DropImageType.Invalid.</returns>
        private static bool IsDropDescriptionValid(IDataObject dataObject)
        {
            object data = ComTypes.ComDataObjectExtensions.GetDropDescription((ComTypes.IDataObject)dataObject);
            if (data is DropDescription)
                return (DropImageType)((DropDescription)data).type != DropImageType.Invalid;
            return false;
        }

        /// <summary>
        /// Checks if the IsDefault drop description flag is set for the associated DataObject.
        /// </summary>
        /// <param name="dataObject">The associated DataObject.</param>
        /// <returns>True if the IsDefault flag is set, otherwise False.</returns>
        private static bool IsDropDescriptionDefault(IDataObject dataObject)
        {
            if (s_dropDescriptions.ContainsKey(dataObject))
                return (s_dropDescriptions[dataObject] & DropDescriptionFlags.IsDefault) == DropDescriptionFlags.IsDefault;
            return false;
        }

        /// <summary>
        /// Checks if the InvalidateRequired drop description flag is set for the associated DataObject.
        /// </summary>
        /// <param name="dataObject">The associated DataObject.</param>
        /// <returns>True if the InvalidateRequired flag is set, otherwise False.</returns>
        private static bool InvalidateRequired(IDataObject dataObject)
        {
            if (s_dropDescriptions.ContainsKey(dataObject))
                return (s_dropDescriptions[dataObject] & DropDescriptionFlags.InvalidateRequired) == DropDescriptionFlags.InvalidateRequired;
            return false;
        }

        /// <summary>
        /// Sets the IsDefault drop description flag for the associated DataObject.
        /// </summary>
        /// <param name="dataObject">The associdated DataObject.</param>
        /// <param name="isDefault">True to set the flag, False to unset it.</param>
        private static void SetDropDescriptionIsDefault(IDataObject dataObject, bool isDefault)
        {
            if (isDefault)
                SetDropDescriptionFlag(dataObject, DropDescriptionFlags.IsDefault);
            else
                UnsetDropDescriptionFlag(dataObject, DropDescriptionFlags.IsDefault);
        }

        /// <summary>
        /// Sets the InvalidatedRequired drop description flag for the associated DataObject.
        /// </summary>
        /// <param name="dataObject">The associdated DataObject.</param>
        /// <param name="isDefault">True to set the flag, False to unset it.</param>
        private static void SetInvalidateRequired(IDataObject dataObject, bool required)
        {
            if (required)
                SetDropDescriptionFlag(dataObject, DropDescriptionFlags.InvalidateRequired);
            else
                UnsetDropDescriptionFlag(dataObject, DropDescriptionFlags.InvalidateRequired);
        }

        /// <summary>
        /// Sets a drop description flag.
        /// </summary>
        /// <param name="dataObject">The associated DataObject.</param>
        /// <param name="flag">The drop description flag to set.</param>
        private static void SetDropDescriptionFlag(IDataObject dataObject, DropDescriptionFlags flag)
        {
            if (s_dropDescriptions.ContainsKey(dataObject))
                s_dropDescriptions[dataObject] |= flag;
            else
                s_dropDescriptions.Add(dataObject, flag);
        }

        /// <summary>
        /// Unsets a drop description flag.
        /// </summary>
        /// <param name="dataObject">The associated DataObject.</param>
        /// <param name="flag">The drop description flag to unset.</param>
        private static void UnsetDropDescriptionFlag(IDataObject dataObject, DropDescriptionFlags flag)
        {
            if (s_dropDescriptions.ContainsKey(dataObject))
            {
                DropDescriptionFlags current = s_dropDescriptions[dataObject];
                s_dropDescriptions[dataObject] = (current | flag) ^ flag;
            }
        }

        /// <summary>
        /// Gets the current DropDescription's drop image type.
        /// </summary>
        /// <param name="dataObject">The DataObject.</param>
        /// <returns>The current drop image type.</returns>
        private static DropImageType GetDropImageType(IDataObject dataObject)
        {
            object data = ComTypes.ComDataObjectExtensions.GetDropDescription((ComTypes.IDataObject)dataObject);
            if (data is DropDescription)
                return (DropImageType)((DropDescription)data).type;
            return DropImageType.Invalid;
        }

        #endregion // Helper methods

        #region AdviseSink class

        /// <summary>
        /// Provides an advisory sink for the COM IDataObject implementation.
        /// </summary>
        private class AdviseSink : ComTypes.IAdviseSink
        {
            // The associated data object
            private IDataObject data;

            /// <summary>
            /// Creates an AdviseSink associated to the specified data object.
            /// </summary>
            /// <param name="data">The data object.</param>
            public AdviseSink(IDataObject data)
            {
                this.data = data;
            }

            /// <summary>
            /// Handles DataChanged events from a COM IDataObject.
            /// </summary>
            /// <param name="format">The data format that had a change.</param>
            /// <param name="stgmedium">The data value.</param>
            public void OnDataChange(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM stgmedium)
            {
                // We listen to DropDescription changes, so that we can unset the IsDefault
                // drop description flag.
                object odd = ComTypes.ComDataObjectExtensions.GetDropDescription((ComTypes.IDataObject)data);
                if (odd != null)
                    DragSourceHelper.SetDropDescriptionIsDefault(data, false);
            }

            #region Unsupported callbacks

            public void OnClose()
            {
                throw new NotImplementedException();
            }

            public void OnRename(System.Runtime.InteropServices.ComTypes.IMoniker moniker)
            {
                throw new NotImplementedException();
            }

            public void OnSave()
            {
                throw new NotImplementedException();
            }

            public void OnViewChange(int aspect, int index)
            {
                throw new NotImplementedException();
            }

            #endregion // Unsupported callbacks
        }

        #endregion // AdviseSink class
    }
}

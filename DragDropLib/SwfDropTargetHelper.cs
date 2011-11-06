namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using DragDropLib;

    public static class DropTargetHelper
    {
        /// <summary>
        /// Internal instance of the DragDropHelper.
        /// </summary>
        private static IDropTargetHelper s_instance = (IDropTargetHelper)new DragDropHelper();

        /// <summary>
        /// Internal cache of IDataObjects related to drop targets.
        /// </summary>
        private static IDictionary<Control, IDataObject> s_dataContext = new Dictionary<Control, IDataObject>();

        static DropTargetHelper()
        {
        }

        /// <summary>
        /// Notifies the DragDropHelper that the specified Control received
        /// a DragEnter event.
        /// </summary>
        /// <param name="control">The Control the received the DragEnter event.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragEnter(Control control, IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect)
        {
            SwfDropTargetHelperExtensions.DragEnter(s_instance, control, data, cursorOffset, effect);
        }

        /// <summary>
        /// Sets the drop description of the IDataObject and then notifies the
        /// DragDropHelper that the specified Control received a DragEnter event.
        /// </summary>
        /// <param name="control">The Control the received the DragEnter event.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        /// <param name="descriptionMessage">The drop description message.</param>
        /// <param name="descriptionInsert">The drop description insert.</param>
        /// <remarks>
        /// Because the DragLeave event in SWF does not provide the IDataObject in the
        /// event args, this DragEnter override of the DropTargetHelper will cache a
        /// copy of the IDataObject based on the provided Control so that it may be
        /// cleared using the DragLeave override that takes a Control parameter. Note that
        /// if you use this override of DragEnter, you must call the DragLeave override
        /// that takes a Control parameter to avoid a possible memory leak. However, calling
        /// this method multiple times with the same Control parameter while not calling the
        /// DragLeave method will not leak memory.
        /// </remarks>
        public static void DragEnter(Control control, IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect, string descriptionMessage, string descriptionInsert)
        {
            data.SetDropDescription((DropImageType)effect, descriptionMessage, descriptionInsert);
            DragEnter(control, data, cursorOffset, effect);

            if (!s_dataContext.ContainsKey(control))
                s_dataContext.Add(control, data);
            else
                s_dataContext[control] = data;
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a DragOver event.
        /// </summary>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragOver(System.Drawing.Point cursorOffset, DragDropEffects effect)
        {
            SwfDropTargetHelperExtensions.DragOver(s_instance, cursorOffset, effect);
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a DragLeave event.
        /// </summary>
        public static void DragLeave()
        {
            s_instance.DragLeave();
        }

        /// <summary>
        /// Clears the drop description of the IDataObject previously associated to the
        /// provided control, then notifies the DragDropHelper that the current control
        /// received a DragLeave event.
        /// </summary>
        /// <remarks>
        /// Because the DragLeave event in SWF does not provide the IDataObject in the
        /// event args, this DragLeave override of the DropTargetHelper will lookup a
        /// cached copy of the IDataObject based on the provided Control and clear
        /// the drop description. Note that the underlying DragLeave call of the
        /// Shell IDropTargetHelper object keeps the current control cached, so the
        /// control passed to this method is only relevant to looking up the IDataObject
        /// on which to clear the drop description.
        /// </remarks>
        public static void DragLeave(Control control)
        {
            if (s_dataContext.ContainsKey(control))
            {
                s_dataContext[control].SetDropDescription(DropImageType.Invalid, null, null);
                s_dataContext.Remove(control);
            }

            DragLeave();
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a DragOver event.
        /// </summary>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void Drop(IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect)
        {
            // No need to clear the drop description, but don't keep it stored to avoid memory leaks
            foreach (KeyValuePair<Control, IDataObject> pair in s_dataContext)
            {
                if (object.ReferenceEquals(pair.Value, data))
                {
                    s_dataContext.Remove(pair);
                    break;
                }
            }

            SwfDropTargetHelperExtensions.Drop(s_instance, data, cursorOffset, effect);
        }

        /// <summary>
        /// Tells the DragDropHelper to show or hide the drag image.
        /// </summary>
        /// <param name="show">True to show the image. False to hide it.</param>
        public static void Show(bool show)
        {
            s_instance.Show(show);
        }
    }
}

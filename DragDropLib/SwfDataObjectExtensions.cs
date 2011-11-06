namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using DragDropLib;
    using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
    using ComTypes = System.Runtime.InteropServices.ComTypes;

    public enum DropImageType
    {
        Invalid = -1,
        None = 0,
        Copy = (int)DragDropEffects.Copy,
        Move = (int)DragDropEffects.Move,
        Link = (int)DragDropEffects.Link,
        Label = 6,
        Warning = 7
    }

    /// <summary>
    /// Provides extended functionality to the System.Windows.Forms.IDataObject interface.
    /// </summary>
    public static class SwfDataObjectExtensions
    {
        #region DLL imports

        [DllImport("gdiplus.dll")]
        private static extern bool DeleteObject(IntPtr hgdi);

        [DllImport("ole32.dll")]
        private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

        #endregion // DLL imports

        /// <summary>
        /// Sets the drag image as the rendering of a control.
        /// </summary>
        /// <param name="dataObject">The DataObject to set the drag image on.</param>
        /// <param name="control">The Control to render as the drag image.</param>
        /// <param name="cursorOffset">The location of the cursor relative to the control.</param>
        public static void SetDragImage(this IDataObject dataObject, Control control, System.Drawing.Point cursorOffset)
        {
            int width = control.Width;
            int height = control.Height;

            Bitmap bmp = new Bitmap(width, height);
            control.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));

            SetDragImage(dataObject, bmp, cursorOffset);
        }

        /// <summary>
        /// Sets the drag image.
        /// </summary>
        /// <param name="dataObject">The DataObject to set the drag image on.</param>
        /// <param name="image">The drag image.</param>
        /// <param name="cursorOffset">The location of the cursor relative to the image.</param>
        public static void SetDragImage(this IDataObject dataObject, Image image, System.Drawing.Point cursorOffset)
        {
            ShDragImage shdi = new ShDragImage();

            Win32Size size;
            size.cx = image.Width;
            size.cy = image.Height;
            shdi.sizeDragImage = size;

            Win32Point wpt;
            wpt.x = cursorOffset.X;
            wpt.y = cursorOffset.Y;
            shdi.ptOffset = wpt;

            shdi.crColorKey = Color.Magenta.ToArgb();

            // This HBITMAP will be managed by the DragDropHelper
            // as soon as we pass it to InitializeFromBitmap. If we fail
            // to make the hand off, we'll delete it to prevent a mem leak.
            IntPtr hbmp = GetHbitmapFromImage(image);
            shdi.hbmpDragImage = hbmp;

            try
            {
                IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();

                try
                {
                    sourceHelper.InitializeFromBitmap(ref shdi, (ComIDataObject)dataObject);
                }
                catch (NotImplementedException ex)
                {
                    throw new Exception("A NotImplementedException was caught. This could be because you forgot to construct your DataObject using a DragDropLib.DataObject", ex);
                }
            }
            catch
            {
                DeleteObject(hbmp);
            }
        }

        /// <summary>
        /// Gets an HBITMAP from any image.
        /// </summary>
        /// <param name="image">The image to get an HBITMAP from.</param>
        /// <returns>An HBITMAP pointer.</returns>
        /// <remarks>
        /// The caller is responsible to call DeleteObject on the HBITMAP.
        /// </remarks>
        private static IntPtr GetHbitmapFromImage(Image image)
        {
            if (image is Bitmap)
            {
                return ((Bitmap)image).GetHbitmap();
            }
            else
            {
                Bitmap bmp = new Bitmap(image);
                return bmp.GetHbitmap();
            }
        }

        /// <summary>
        /// Sets the drop description for the drag image manager.
        /// </summary>
        /// <param name="dataObject">The DataObject to set.</param>
        /// <param name="type">The type of the drop image.</param>
        /// <param name="format">The format string for the description.</param>
        /// <param name="insert">The parameter for the drop description.</param>
        /// <remarks>
        /// When setting the drop description, the text can be set in two part,
        /// which will be rendered slightly differently to distinguish the description
        /// from the subject. For example, the format can be set as "Move to %1" and
        /// the insert as "Temp". When rendered, the "%1" in format will be replaced
        /// with "Temp", but "Temp" will be rendered slightly different from "Move to ".
        /// </remarks>
        public static void SetDropDescription(this IDataObject dataObject, DropImageType type, string format, string insert)
        {
            if (format != null && format.Length > 259)
                throw new ArgumentException("Format string exceeds the maximum allowed length of 259.", "format");
            if (insert != null && insert.Length > 259)
                throw new ArgumentException("Insert string exceeds the maximum allowed length of 259.", "insert");

            // Fill the structure
            DropDescription dd;
            dd.type = (int)type;
            dd.szMessage = format;
            dd.szInsert = insert;

            ComTypes.ComDataObjectExtensions.SetDropDescription((ComTypes.IDataObject)dataObject, dd);
        }

        /// <summary>
        /// Sets managed data to a clipboard DataObject.
        /// </summary>
        /// <param name="dataObject">The DataObject to set the data on.</param>
        /// <param name="format">The clipboard format.</param>
        /// <param name="data">The data object.</param>
        /// <remarks>
        /// Because the underlying data store is not storing managed objects, but
        /// unmanaged ones, this function provides intelligent conversion, allowing
        /// you to set unmanaged data into the COM implemented IDataObject.</remarks>
        public static void SetDataEx(this IDataObject dataObject, string format, object data)
        {
            DataFormats.Format dataFormat = DataFormats.GetFormat(format);

            // Initialize the format structure
            ComTypes.FORMATETC formatETC = new ComTypes.FORMATETC();
            formatETC.cfFormat = (short)dataFormat.Id;
            formatETC.dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT;
            formatETC.lindex = -1;
            formatETC.ptd = IntPtr.Zero;

            // Try to discover the TYMED from the format and data
            ComTypes.TYMED tymed = GetCompatibleTymed(format, data);
            // If a TYMED was found, we can use the system DataObject
            // to convert our value for us.
            if (tymed != ComTypes.TYMED.TYMED_NULL)
            {
                formatETC.tymed = tymed;

                // Set data on an empty DataObject instance
                DataObject conv = new DataObject();
                conv.SetData(format, true, data);

                // Now retrieve the data, using the COM interface.
                // This will perform a managed to unmanaged conversion for us.
                ComTypes.STGMEDIUM medium;
                ((ComTypes.IDataObject)conv).GetData(ref formatETC, out medium);
                try
                {
                    // Now set the data on our data object
                    ((ComTypes.IDataObject)dataObject).SetData(ref formatETC, ref medium, true);
                }
                catch
                {
                    // On exceptions, release the medium
                    ReleaseStgMedium(ref medium);
                    throw;
                }
            }
            else
            {
                // Since we couldn't determine a TYMED, this data
                // is likely custom managed data, and won't be used
                // by unmanaged code, so we'll use our custom marshaling
                // implemented by our COM IDataObject extensions.

                ComTypes.ComDataObjectExtensions.SetManagedData((ComTypes.IDataObject)dataObject, format, data);
            }
        }

        /// <summary>
        /// Gets a system compatible TYMED for the given format.
        /// </summary>
        /// <param name="format">The data format.</param>
        /// <param name="data">The data.</param>
        /// <returns>A TYMED value, indicating a system compatible TYMED that can
        /// be used for data marshaling.</returns>
        private static ComTypes.TYMED GetCompatibleTymed(string format, object data)
        {
            if (IsFormatEqual(format, DataFormats.Bitmap) && data is System.Drawing.Bitmap)
                return ComTypes.TYMED.TYMED_GDI;
            if (IsFormatEqual(format, DataFormats.EnhancedMetafile))
                return ComTypes.TYMED.TYMED_ENHMF;
            if (data is Stream
                || IsFormatEqual(format, DataFormats.Html)
                || IsFormatEqual(format, DataFormats.Text) || IsFormatEqual(format, DataFormats.Rtf)
                || IsFormatEqual(format, DataFormats.OemText)
                || IsFormatEqual(format, DataFormats.UnicodeText) || IsFormatEqual(format, "ApplicationTrust")
                || IsFormatEqual(format, DataFormats.FileDrop)
                || IsFormatEqual(format, "FileName")
                || IsFormatEqual(format, "FileNameW"))
                return ComTypes.TYMED.TYMED_HGLOBAL;
            if (IsFormatEqual(format, DataFormats.Dib) && data is System.Drawing.Image)
                return System.Runtime.InteropServices.ComTypes.TYMED.TYMED_NULL;
            if (IsFormatEqual(format, typeof(System.Drawing.Bitmap).FullName))
                return ComTypes.TYMED.TYMED_HGLOBAL;
            if (IsFormatEqual(format, DataFormats.EnhancedMetafile) || data is System.Drawing.Imaging.Metafile)
                return System.Runtime.InteropServices.ComTypes.TYMED.TYMED_NULL;
            if (IsFormatEqual(format, DataFormats.Serializable) || (data is System.Runtime.Serialization.ISerializable)
                || ((data != null) && data.GetType().IsSerializable))
                return ComTypes.TYMED.TYMED_HGLOBAL;

            return ComTypes.TYMED.TYMED_NULL;
        }

        /// <summary>
        /// Compares the equality of two clipboard formats.
        /// </summary>
        /// <param name="formatA">First format.</param>
        /// <param name="formatB">Second format.</param>
        /// <returns>True if the formats are equal. False otherwise.</returns>
        private static bool IsFormatEqual(string formatA, string formatB)
        {
            return string.CompareOrdinal(formatA, formatB) == 0;
        }

        /// <summary>
        /// Gets managed data from a clipboard DataObject.
        /// </summary>
        /// <param name="dataObject">The DataObject to obtain the data from.</param>
        /// <param name="format">The format for which to get the data in.</param>
        /// <returns>The data object instance.</returns>
        public static object GetDataEx(this IDataObject dataObject, string format)
        {
            // Get the data
            object data = dataObject.GetData(format, true);

            // If the data is a stream, we'll check to see if it
            // is stamped by us for custom marshaling
            if (data is Stream)
            {
                object data2 = ComTypes.ComDataObjectExtensions.GetManagedData((ComTypes.IDataObject)dataObject, format);
                if (data2 != null)
                    return data2;
            }

            return data;
        }
    }
}

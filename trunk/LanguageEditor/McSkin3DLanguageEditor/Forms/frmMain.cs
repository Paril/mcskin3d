using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MCSkin3DLanguageEditor.Forms;
using System.Globalization;

namespace MCSkin3DLanguageEditor
{
    public partial class frmMain : Form
    {
        #region Variables

        Size openFull = new Size(481, 556);
        Size openHalf = new Size(481, 473);
        Size closedAll = new Size(481, 331);

        private bool expanded;
        private Language.Language curLang;
        private Language.Language loadedLang;
        private string langPath;
        private Dictionary<string, ListViewGroup> groups;

        private static CultureInfo[] cultureInfos;
        private List<string> cultureItems;

        private bool savedSinceLastEdit = true;

        #endregion

        public frmMain()
        {
            InitializeComponent();
            cultureInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            cultureItems = new List<string>();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                e.SuppressKeyPress = true;
                performOpen();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                performSave();
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Enter && lvItems.Focused) || (e.KeyCode == Keys.Enter && lvItemsOthers.Focused))
            {
                e.SuppressKeyPress = true;
                performEdit();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cultureItems.Clear();
            foreach (CultureInfo cultureInfo in cultureInfos)
            {
                if (cultureInfo.TwoLetterISOLanguageName != "iv")
                {
                    cultureItems.Add(getCultureString(cultureInfo));
                }
            }
            setFormMode(formMode.closedAll);
        }

        #region Private Methods

        enum formMode { openFull, openHalf, closedAll };

        private string getCultureString(String name)
        {
            CultureInfo ci = new CultureInfo(name);
            string nativeName = ci.NativeName;
            if (!char.IsUpper(nativeName, 0))
            {
                nativeName = char.ToUpper(nativeName[0], ci) + nativeName.Remove(0, 1);
            }
            return nativeName + " (" + ci.Name + ")";
        }

        private string getCultureString(CultureInfo ci)
        {
            string nativeName = ci.NativeName;
            if (!char.IsUpper(nativeName, 0))
            {
                nativeName = char.ToUpper(nativeName[0], ci) + nativeName.Remove(0, 1);
            }
            return nativeName + " (" + ci.Name + ")";
        }

        private void setFormMode(formMode fm)
        {
            switch (fm)
            {
                case formMode.openFull:
                    this.Size = openFull;
                    break;
                case formMode.openHalf:
                    this.Size = openHalf;
                    break;
                case formMode.closedAll:
                    this.Size = closedAll;
                    break;
            }
        }

        private void performNew()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "";
            dlg.Filter = "MCSkin3D Language Files (*.lang)|*.lang";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                newFile(dlg.FileName);
            }
        }

        private void performOpen()
        {
            if (!savedSinceLastEdit)
            {
                DialogResult result = MessageBox.Show("Some changes has been made to the language file.\nWould you like to save them before opening another language file?", "Error!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                bool open = false;
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        performSave();
                        open = true;
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        open = true;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        break;
                }
                if (open)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.FileName = "";
                    dlg.Filter = "McSkin3D Language Files (*.lang)|*.lang";
                    dlg.Multiselect = false;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        loadFile(dlg.FileName);
                    }
                }
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "";
                dlg.Filter = "McSkin3D Language Files (*.lang)|*.lang";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    loadFile(dlg.FileName);
                }
            }
        }

        private void performSave()
        {
            curLang.SaveLanguage(loadedLang, curLang.OtherTable, curLang.StringTable, langPath);
            savedSinceLastEdit = true;
            MessageBox.Show("Successfully saved lang-file to \"" + langPath + "\"", "Done");
        }

        private void performEdit()
        {
            bool anyChanges = false;
            if (lvItems.Focused)
            {
                string[] splittedTag = lvItems.SelectedItems[0].Tag.ToString().Split(new char[] { ':' });
                frmEdit dlg = new frmEdit(lvItems.SelectedItems[0].SubItems[1].Text, frmEdit.EditModes.TextBox);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    anyChanges = (lvItems.SelectedItems[0].SubItems[1].Text != dlg.txtValue.Text);
                    lvItems.SelectedItems[0].SubItems[1].Text = dlg.txtValue.Text;
                    curLang.StringTable[splittedTag[1]] = dlg.txtValue.Text;
                }
            }
            else if (lvItemsOthers.Focused)
            {
                if (lvItemsOthers.SelectedItems[0].Tag.Equals("cb"))
                {
                    frmEdit dlg = new frmEdit(cultureItems.IndexOf(getCultureString(lvItemsOthers.SelectedItems[0].SubItems[1].Text)), frmEdit.EditModes.ComboBox, cultureItems);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        if (dlg.cbValue.SelectedIndex > 0)
                        {
                            anyChanges = (lvItemsOthers.SelectedItems[0].SubItems[1].Text != cultureInfos[dlg.cbValue.SelectedIndex].Name);
                            lvItemsOthers.SelectedItems[0].SubItems[1].Text = cultureInfos[dlg.cbValue.SelectedIndex].Name;
                            curLang.OtherTable[lvItemsOthers.SelectedItems[0].Text] = cultureInfos[dlg.cbValue.SelectedIndex].Name;
                        }
                        else
                        {
                            anyChanges = false;
                            lvItemsOthers.SelectedItems[0].SubItems[1].Text = "";
                            curLang.OtherTable[lvItemsOthers.SelectedItems[0].Text] = "";
                        }
                    }
                }
                else if (lvItemsOthers.SelectedItems[0].Tag.Equals("txt"))
                {
                    frmEdit dlg = new frmEdit(lvItemsOthers.SelectedItems[0].SubItems[1].Text, frmEdit.EditModes.TextBox);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        anyChanges = (lvItemsOthers.SelectedItems[0].SubItems[1].Text != dlg.txtValue.Text);
                        lvItemsOthers.SelectedItems[0].SubItems[1].Text = dlg.txtValue.Text;
                        curLang.OtherTable[lvItemsOthers.SelectedItems[0].Text] = dlg.txtValue.Text;
                    }
                }
            }
            savedSinceLastEdit = !anyChanges;
        }

        private ListViewGroup getGroupFromHeader(string h, ListView lv)
        {
            ListViewGroup g = null;
            foreach (ListViewGroup group in lv.Groups)
            {
                if (group.Header == h)
                {
                    g = group;
                    break;
                }
            }
            return g;
        }

        private void newFile(string path)
        {
            string fileText = Encoding.Unicode.GetString(Properties.Resources.Template);
            StreamWriter sw = new StreamWriter(File.Create(path), Encoding.Unicode);
            sw.Write(fileText);
            sw.Close();
            loadFile(path);
        }

        private void loadFile(string path)
        {
            langPath = path;
            StreamReader sr = new StreamReader(langPath, System.Text.Encoding.Unicode);
            try
            {
                loadedLang = Language.Language.Parse(sr);
                curLang = loadedLang;
            }
            catch (NoHeaderException ex)
            {
            }
            catch (ParseErrorException ex)
            {
            }
            sr.Close();
            sr = null;
            groups = new Dictionary<string, ListViewGroup>();

            lvItemsOthers.Items.Clear();
            lvItemsOthers.Groups.Clear();
            foreach (KeyValuePair<string, string> o in curLang.OtherTable)
            {
                string name = o.Key;
                string value = o.Value;
                string groupstring = "Others";
                ListViewGroup grFromHeader = getGroupFromHeader(groupstring, lvItemsOthers);
                if (grFromHeader != null)
                {
                    ListViewItem item = new ListViewItem();
                    item.Group = grFromHeader;
                    item.Text = name;
                    item.Tag = (name == "Culture") ? "cb" : "txt";
                    item.SubItems.Add(value);
                    lvItemsOthers.Items.Add(item);
                }
                else
                {
                    ListViewGroup lvg = new ListViewGroup(groupstring);
                    ListViewItem item = new ListViewItem();
                    if (!groups.ContainsKey(groupstring))
                    {
                        groups.Add(groupstring, lvg);
                        lvItemsOthers.Groups.Add(lvg);
                    }
                    item.Group = lvg;
                    item.Text = name;
                    item.Tag = (name == "Culture") ? "cb" : "txt";
                    item.SubItems.Add(value);
                    lvItemsOthers.Items.Add(item);
                }
            }

            groups = new Dictionary<string, ListViewGroup>();
            lvItems.Items.Clear();
            lvItems.Groups.Clear();
            foreach (KeyValuePair<string, string> l in curLang.StringTable)
            {
                string nonTrimmedName = l.Key;
                string name = l.Key.Trim();
                string value = l.Value;
                string groupChar = name.Substring(0, 1);
                ListViewGroup grFromHeader = getGroupFromHeader(groupChar, lvItems);
                if (grFromHeader != null)
                {
                    ListViewItem item = new ListViewItem();
                    item.Group = grFromHeader;
                    item.Text = name;
                    item.Tag = "txt:" + nonTrimmedName;
                    item.SubItems.Add(value);
                    lvItems.Items.Add(item);
                }
                else
                {
                    ListViewGroup lvg = new ListViewGroup(groupChar);
                    ListViewItem item = new ListViewItem();
                    if (!groups.ContainsKey(groupChar))
                    {
                        groups.Add(groupChar, lvg);
                        lvItems.Groups.Add(lvg);
                    }
                    item.Group = lvg;
                    item.Text = name;
                    item.Tag = "txt:" + nonTrimmedName;
                    item.SubItems.Add(value);
                    lvItems.Items.Add(item);
                }
            }
            setFormMode(formMode.openHalf);
            pSave.Enabled = true;
            linkSave.Enabled = true;
        }

        private void performExpand()
        {
            expanded = !expanded;
            if (expanded)
                setFormMode(formMode.openFull);
            else
                setFormMode(formMode.openHalf);
        }

        #endregion

        private void pTop_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.White), 1.0f), 0, pTop.Height - 2, pTop.Width - 1, pTop.Height - 2);
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(160, 160, 160)), 1.0f), 0, pTop.Height - 1, pTop.Width - 1, pTop.Height - 1);
        }

        private void linkOpen_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            performNew();
        }

        private void pNew_Click(object sender, EventArgs e)
        {
            performNew();
        }

        private void linkOpenFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            performOpen();
        }

        private void pOpen_Click(object sender, EventArgs e)
        {
            performOpen();
        }

        private void linkSave_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            performSave();
        }

        private void pSave_Click(object sender, EventArgs e)
        {
            performSave();
        }

        private void lvItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                performEdit();
        }

        private void lvItemsOthers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                performEdit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!savedSinceLastEdit)
            {
                DialogResult result = MessageBox.Show("Some changes has been made to the language file.\nWould you like to save them before exiting the applicaiton?", "Error!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        performSave();
                        e.Cancel = false;
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        e.Cancel = false;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}
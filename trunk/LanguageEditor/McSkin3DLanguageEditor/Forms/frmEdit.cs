using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3DLanguageEditor.Forms
{
    public partial class frmEdit : Form
    {
        public enum EditModes { TextBox, ComboBox };

        public EditModes editMode = EditModes.TextBox;

        public frmEdit()
        {
            InitializeComponent();
        }

        public frmEdit(object o, EditModes em)
        {
            InitializeComponent();
            editMode = em;
            txtValue.Visible = (em == EditModes.TextBox);
            cbValue.Visible = (em == EditModes.ComboBox);
            switch (editMode)
            {
                case EditModes.TextBox:
                    txtValue.Text = (string)o;
                    txtValue.Select();
                    txtValue.Focus();
                    if ((txtValue.Text != ""))
                        txtValue.SelectionStart = txtValue.Text.Length - 1;
                    break;
                case EditModes.ComboBox:
                    cbValue.SelectedIndex = (int)o;
                    break;
            }
        }

        public frmEdit(object o, EditModes em, List<string> s)
        {
            InitializeComponent();
            editMode = em;
            txtValue.Visible = (em == EditModes.TextBox);
            cbValue.Visible = (em == EditModes.ComboBox);
            switch (editMode)
            {
                case EditModes.TextBox:
                    txtValue.Text = (string)o;
                    txtValue.Select();
                    txtValue.Focus();
                    if ((txtValue.Text != ""))
                        txtValue.SelectionStart = txtValue.Text.Length - 1;
                    break;
                case EditModes.ComboBox:
                    foreach (object str in s)
                        cbValue.Items.Add(str);
                    o = (int)o + 1;
                    cbValue.SelectedIndex = (int)o;
                    break;
            }
        }

        private void frmEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnOK.PerformClick();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}

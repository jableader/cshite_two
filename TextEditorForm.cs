using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace cshite_ass_2
{
    public partial class TextEditorForm : Form
    {
        readonly User user; // The currently logged in user

        string CurrentFile
        {
            get => _currentFile; // The currently loaded file
            set
            {
                _currentFile = value;

                // We must force the user to "Save As" before we can save
                saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = (_currentFile != null) && user.Type == UserType.Edit;
            }
        }
        string _currentFile;

        [Obsolete("This constructor is for the designer only, use the one which accepts a User")]
        internal TextEditorForm() // To use the WinForms designer you must provide a parameterless constructor. The ObsoleteAttribute will warn us.
        {
            InitializeComponent();
        }

        public TextEditorForm(User user)
        {
            InitializeComponent();

            this.user = user ?? throw new ArgumentNullException(nameof(user)); // You must provide a valid user for the form

            lblName.Text = user.Username; // Show the users name in the label
            CurrentFile = null; // No files loaded yet

            cboFontSize.Items.AddRange(Enumerable.Range(8, 20).Cast<object>().ToArray()); // Add the font sizes
            UpdateFontControls(); // Make sure all the font details are correct

            if (user.Type == UserType.View) // When the user is View we want to disable all the controls that edit the file
            {
                DisableEditControls();
            }
        }

        void DisableEditControls()
        {
            richTextBox.ReadOnly = true;

            // We can loop over and disable all the buttons
            var editButtons = new ToolStripItem[] { btnBold, btnItalic, btnUnderline, cboFontSize, btnCut, pasteToolStripMenuItem, btnSaveAs, saveToolStripButton, saveAsToolStripMenuItem, saveToolStripMenuItem, cutToolStripMenuItem, btnPaste };
            foreach (var button in editButtons)
            {
                button.Enabled = false;
            }
        }

        void NewFile()
        {
            // Creates a new file
            richTextBox.Text = string.Empty;
            CurrentFile = null;
        }

        void SaveFile()
           => richTextBox.SaveFile(CurrentFile); // Saves the file to it's last saved location

        void OpenFile() // Open a file from disk using the openfile dialog
            => ShowFileDialog<OpenFileDialog>(d => d.OpenFile(), stream => richTextBox.LoadFile(stream, RichTextBoxStreamType.RichText));

        void SaveFileAs() // Save the file to disk, allowing the user to select where/how
            => ShowFileDialog<SaveFileDialog>(d => d.OpenFile(), stream => richTextBox.SaveFile(stream, RichTextBoxStreamType.RichText));

        void ShowFileDialog<T>(Func<T, Stream> open, Action<Stream> actionForStream) where T : FileDialog, new() // Open and SaveAs have near identical functionality, leverage anonymous functions and generics to avoid duplication
        {
            using (var fileDialog = new T())
            {
                fileDialog.Filter = "RTF Files (*.rtf)|*.rtf";

                if (CurrentFile != null) // If there is a CurrentFile set we should default to that for convenience
                {
                    fileDialog.InitialDirectory = Path.GetDirectoryName(CurrentFile);
                    fileDialog.FileName = Path.GetFileName(CurrentFile);
                }

                if (fileDialog.ShowDialog() == DialogResult.OK) // Show the form and let the user make their choice
                {
                    CurrentFile = fileDialog.FileName;

                    using (var s = open(fileDialog)) // Open & act
                    {
                        actionForStream(s);
                    }
                }
            }
        }

        void ToggleFont(FontStyle toggle) // Toggles the specified style for the selected font
        {
            var font = richTextBox.SelectionFont;
            var newStyle = font.Style.HasFlag(toggle) ?
                font.Style & ~toggle : // Simple bitmask operations
                font.Style | toggle;

            richTextBox.SelectionFont = new Font(font, newStyle);
            UpdateFontControls(); // Update the font controls to match the font
        }

        void UpdateFontControls()
        {
            // Pretty straightforward, show the 'checked' decorations on the buttons to match the current font
            var currentFont = richTextBox.SelectionFont;
            btnBold.Checked = currentFont.Style.HasFlag(FontStyle.Bold);
            btnItalic.Checked = currentFont.Style.HasFlag(FontStyle.Italic);
            btnUnderline.Checked = currentFont.Style.HasFlag(FontStyle.Underline);
            cboFontSize.SelectedItem = (int)currentFont.Size; // Set the combobox to match the font size
        }

        void Cut() => richTextBox.Cut();
        void Paste() => richTextBox.Paste();
        void Copy() => richTextBox.Copy();

        void ShowAbout()
            => MessageBox.Show( // The spec didn't really say much about this, just added random crap
@"Text Editor V1
Created by Jacob Dunk", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);

        #region Event Handlers

        private void NewToolStripButton_Click(object sender, EventArgs e) => NewFile();
        private void NewToolStripMenuItem_Click(object sender, EventArgs e) => NewFile();

        private void OpenToolStripButton_Click(object sender, EventArgs e) => OpenFile();
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e) => OpenFile();

        private void SaveToolStripButton_Click(object sender, EventArgs e) => SaveFile();
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();

        private void BtnSaveAs_Click(object sender, EventArgs e) => SaveFileAs();
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) => SaveFileAs();

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void BtnBold_Click(object sender, EventArgs e) => ToggleFont(FontStyle.Bold);
        private void BtnItalic_Click(object sender, EventArgs e) => ToggleFont(FontStyle.Italic);
        private void BtnUnderline_Click(object sender, EventArgs e) => ToggleFont(FontStyle.Underline);
        private void RichTextBox_SelectionChanged(object sender, EventArgs e) => UpdateFontControls(); // When the user clicks around the form we want the font controls to reflect their current font

        private void CutToolStripButton1_Click(object sender, EventArgs e) => Cut();
        private void CutToolStripMenuItem_Click(object sender, EventArgs e) => Cut();

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e) => Copy();
        private void CopyToolStripButton1_Click(object sender, EventArgs e) => Copy();

        private void PasteToolStripButton1_Click(object sender, EventArgs e) => Paste();
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e) => Paste();

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) => ShowAbout();
        private void HelpToolStripButton_Click(object sender, EventArgs e) => ShowAbout();

        private void CboFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFontSize.SelectedItem is int size) // We only accept the change if the user selected a valid value
            {
                richTextBox.SelectionFont = new Font(richTextBox.SelectionFont.FontFamily, size, richTextBox.SelectionFont.Style);
            }

            UpdateFontControls();
        }

        #endregion
    }

}

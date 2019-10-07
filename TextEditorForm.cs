using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace cshite_ass_2
{
    public partial class TextEditorForm : Form
    {
        readonly User user;

        string CurrentFile
        {
            get => _currentFile;
            set
            {
                _currentFile = value;

                // We must force the user to "Save As" before we can save
                saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = (_currentFile != null) && user.Type == UserType.Edit;
            }
        }
        string _currentFile;

        internal TextEditorForm()
        {
            InitializeComponent();
        }

        public TextEditorForm(User user)
            : this()
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));

            lblName.Text = user.Username;
            CurrentFile = null;

            cboFontSize.Items.AddRange(Enumerable.Range(8, 20).Cast<object>().ToArray());
            UpdateFontControls();

            if (user.Type == UserType.View)
            {
                DisableEditControls();
            }
        }

        void DisableEditControls()
        {
            richTextBox.ReadOnly = true;

            var editButtons = new ToolStripItem[] { btnBold, btnItalic, btnUnderline, cboFontSize, btnCut, pasteToolStripMenuItem, btnSaveAs, saveToolStripButton, saveAsToolStripMenuItem, saveToolStripMenuItem, cutToolStripMenuItem, btnPaste };
            foreach (var button in editButtons)
            {
                button.Enabled = false;
            }
        }

        void NewFile()
        {
            richTextBox.Text = string.Empty;
            CurrentFile = null;
        }

        void SaveFile()
           => richTextBox.SaveFile(CurrentFile);

        void OpenFile()
            => ShowFileDialog<OpenFileDialog>(d => d.OpenFile(), stream => richTextBox.LoadFile(stream, RichTextBoxStreamType.RichText));

        void SaveFileAs()
            => ShowFileDialog<SaveFileDialog>(d => d.OpenFile(), stream => richTextBox.SaveFile(stream, RichTextBoxStreamType.RichText));

        void ShowFileDialog<T>(Func<T, Stream> open, Action<Stream> actionForStream) where T : FileDialog, new()
        {
            using (var fileDialog = new T())
            {
                fileDialog.Filter = "RTF Files (*.rtf)|*.rtf";

                if (CurrentFile != null)
                {
                    fileDialog.InitialDirectory = Path.GetDirectoryName(CurrentFile);
                    fileDialog.FileName = Path.GetFileName(CurrentFile);
                }

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    CurrentFile = fileDialog.FileName;

                    using (var s = open(fileDialog))
                    {
                        actionForStream(s);
                    }
                }
            }
        }

        void ToggleFont(FontStyle toggle)
        {
            var font = richTextBox.SelectionFont;
            var newStyle = font.Style.HasFlag(toggle) ?
                font.Style & ~toggle :
                font.Style | toggle;

            richTextBox.SelectionFont = new Font(font, newStyle);
            UpdateFontControls();
        }

        void UpdateFontControls()
        {
            var currentFont = richTextBox.SelectionFont;
            btnBold.Checked = currentFont.Style.HasFlag(FontStyle.Bold);
            btnItalic.Checked = currentFont.Style.HasFlag(FontStyle.Italic);
            btnUnderline.Checked = currentFont.Style.HasFlag(FontStyle.Underline);
            cboFontSize.SelectedItem = (int)currentFont.Size;
        }

        void Cut() => richTextBox.Cut();
        void Paste() => richTextBox.Paste();
        void Copy() => richTextBox.Copy();

        void ShowAbout()
            => MessageBox.Show(
@"Text Editor V1
Created by Jacob Dunk", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
        private void RichTextBox_SelectionChanged(object sender, EventArgs e) => UpdateFontControls();

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
            if (cboFontSize.SelectedItem is int size)
            {
                richTextBox.SelectionFont = new Font(richTextBox.SelectionFont.FontFamily, size, richTextBox.SelectionFont.Style);
            }

            UpdateFontControls();
        }
    }
}

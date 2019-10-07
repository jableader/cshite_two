using System;
using System.Windows.Forms;

namespace cshite_ass_2
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            AcceptButton = btnSignIn;
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            new NewUserForm().ShowDialog();
        }

        private void BtnExit_Click(object sender, EventArgs e)
            => Environment.Exit(0);

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
            var user = User.Authenticate(txtUsername.Text, txtPassword.Text);
            if (user == null)
            {
                MessageBox.Show("A user matching this username/password combination could not be found.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Visible = false;

                try
                {
                    new TextEditorForm(user).ShowDialog(this);
                }
                finally
                {
                    txtPassword.Text = string.Empty;
                    Visible = true;
                }
            }
        }
    }
}

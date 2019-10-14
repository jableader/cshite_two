using System;
using System.Windows.Forms;

namespace cshite_ass_2
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            AcceptButton = btnSignIn; // When the user hits enter we should attempt to sign them in
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
            => new NewUserForm().ShowDialog(this); // Nice n simple, just show the form

        private void BtnExit_Click(object sender, EventArgs e)
            => Environment.Exit(0);

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
            var user = User.Authenticate(txtUsername.Text, txtPassword.Text);
            if (user == null) // The users details didn't match any user
            {
                MessageBox.Show("A user matching this username/password combination could not be found.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Visible = false; // Hide the login form while the user is using the editor

                try
                {
                    new TextEditorForm(user).ShowDialog(this);
                }
                finally
                {
                    txtPassword.Text = string.Empty; // When the users finished we should blank their password so that the next person can't just log in
                    Visible = true; // When the text editor is closed reshow this form
                }
            }
        }
    }
}

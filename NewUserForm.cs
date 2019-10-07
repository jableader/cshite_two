using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cshite_ass_2
{
    public partial class NewUserForm : Form
    {
        static object[] UserTypes => Enum.GetValues(typeof(UserType)).Cast<object>().ToArray();

        public NewUserForm()
        {
            InitializeComponent();

            userTypeControl.Items.AddRange(UserTypes);
            userTypeControl.SelectedItem = UserType.View;

            AcceptButton = btnSubmit;
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (CheckValidation())
            {
                var user = User.Create(txtUsername.Text, txtPassword.Text, (UserType)userTypeControl.SelectedItem, txtFirstName.Text, txtLastName.Text, dateOfBirthControl.Value);
                MessageBox.Show($"New user {user.Username} created successfully!", "New User", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                DialogResult = DialogResult.OK;
            }
        }

        bool CheckValidation()
        {
            var errors = ValidateUserData();
            if (!errors.Any())
            {
                return true;
            }

            var message = new StringBuilder().AppendLine("Please fix the following errors before creating a new user:");
            foreach (var field in errors)
            {
                message.AppendLine().Append(field.Key).AppendLine(":");

                foreach (var error in field)
                {
                    message.Append("* ").AppendLine(error);
                }
            }

            MessageBox.Show(message.ToString(), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        IEnumerable<IGrouping<string, string>> ValidateUserData()
        {
            var errors = GetMissingFields().ToList();
            if (!errors.Any())
            {
                if (txtPassword.Text != txtReenterPassword.Text)
                {
                    errors.Add(Tuple.Create((string)txtPassword.Tag, "Passwords do not match"));
                }

                if (User.AllUsers.Any(u => u.Username == txtUsername.Text))
                {
                    errors.Add(Tuple.Create((string)txtUsername.Tag, "Username already taken"));
                }

                if (!UserTypes.Contains(userTypeControl.SelectedItem))
                {
                    errors.Add(Tuple.Create((string)userTypeControl.Tag, "Not a valid user type"));
                }
            }

            return errors.GroupBy(e => e.Item1, e => e.Item2);
        }

        IEnumerable<Tuple<string, string>> GetMissingFields()
            => groupBox1.Controls.OfType<TextBox>()
                .Where(t => string.IsNullOrEmpty(t.Text) || t.Text.Contains(","))
                .Select(t => Tuple.Create((string)t.Tag, "Please input a valid value"));
    }
}

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
        static object[] UserTypes => Enum.GetValues(typeof(UserType)).Cast<object>().ToArray(); // Array containing all values of UserType as `object` to make it easier to work with all the non-generics winforms stuff

        public NewUserForm()
        {
            InitializeComponent();

            userTypeControl.Items.AddRange(UserTypes); // Add the usertypes as selections to the combobox
            userTypeControl.SelectedItem = UserType.View; // Default to View

            AcceptButton = btnSubmit;
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (CheckValidation())
            {
                // Validation passed, lets create the new user
                var user = User.Create(txtUsername.Text, txtPassword.Text, (UserType)userTypeControl.SelectedItem, txtFirstName.Text, txtLastName.Text, dateOfBirthControl.Value);
                MessageBox.Show($"New user {user.Username} created successfully!", "New User", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                DialogResult = DialogResult.OK; // Closes the form
            }
        }

        bool CheckValidation()
        {
            var errors = ValidateUserData(); // Collect all the validation errors
            if (!errors.Any())
            {
                return true; // No errors, hooray
            }

            // There were errors, lets tell the user what they did wrong
            var message = new StringBuilder().AppendLine("Please fix the following errors before creating a new user:");
            foreach (var field in errors)
            {
                message.AppendLine().Append(field.Key).AppendLine(":"); // The Key property is the field name, this would make something like "Username: "

                foreach (var error in field) // Append each of the errors one by one
                {
                    message.Append("* ").AppendLine(error);
                }
            }

            MessageBox.Show(message.ToString(), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false; // Let the caller know there are validation errors
        }

        IEnumerable<IGrouping<string, string>> ValidateUserData()
        {
            var errors = GetMissingFields().ToList();
            if (!errors.Any()) // No point putting more specific errors in if the values are blank anyway
            {
                if (txtPassword.Text != txtReenterPassword.Text)
                {
                    errors.Add(Tuple.Create((string)txtPassword.Tag, "Passwords do not match"));
                }

                if (User.AllUsers.Any(u => u.Username == txtUsername.Text)) // Check if any other user has this username
                {
                    errors.Add(Tuple.Create((string)txtUsername.Tag, "Username already taken"));
                }

                if (!UserTypes.Contains(userTypeControl.SelectedItem)) // Ensure they actually selected a UserType
                {
                    errors.Add(Tuple.Create((string)userTypeControl.Tag, "Not a valid user type"));
                }
            }

            return errors.GroupBy(e => e.Item1, e => e.Item2); // Group errors by their field, with their errors as an IEnumerable
        }

        IEnumerable<Tuple<string, string>> GetMissingFields()
            => groupBox1.Controls.OfType<TextBox>()
                .Where(t => string.IsNullOrEmpty(t.Text) || t.Text.Contains(",")) // We can't accept commas since it destroys our data storage
                .Select(t => Tuple.Create((string)t.Tag, "Please input a valid value")); // The tag contains the field name
    }
}

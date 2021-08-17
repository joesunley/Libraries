using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sunley.Security
{
    public partial class Login : Form
    {
        public static LoginAttempt ReturnValue { get; set; }

        public Login()
        {
            throw new NotImplementedException();
        }

        public Login(LoginDetails login)
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';

            if (login.Email) lblUsername.Text = "Email";
            else lblUsername.Text = "Username";
        }
        public Login(LoginDetails login, bool lightTheme, Color primary)
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';

            if (lightTheme) SetLightTheme(primary);
            else SetDarkTheme(primary);

            if (login.Email) lblUsername.Text = "Email";
            else lblUsername.Text = "Username";
        }

        private void SetLightTheme(Color primary)
        {
            SetPrimary(primary);

            BackColor = SystemColors.ButtonHighlight;
            txtUsername.BackColor = Color.FromArgb(229, 231, 228);
            txtUsername.ForeColor = Color.Black;
            txtPassword.BackColor = Color.FromArgb(229, 231, 228);
            txtPassword.ForeColor = Color.Black;
            btnLogin.ForeColor = Color.White;

            pbExit.Image = Resources.Resources.Cross_Black;
        }
        private void SetDarkTheme(Color primary)
        {
            SetPrimary(primary);

            BackColor = SystemColors.ControlDarkDark;
            txtUsername.BackColor = SystemColors.ControlDark;
            txtUsername.ForeColor = Color.White;
            txtPassword.BackColor = SystemColors.ControlDark;
            txtPassword.ForeColor = Color.White;
            btnLogin.ForeColor = Color.White;

            pbExit.Image = Resources.Resources.Cross_White;
        }
        private void SetPrimary(Color primary)
        {
            lblTitle.ForeColor = primary;
            lblUsername.ForeColor = primary;
            lblPassword.ForeColor = primary;
            btnLogin.BackColor = primary;
            btnRegister.ForeColor = primary;
        }


        // // Events // //

        private void PasswordShowChange(object sender, EventArgs e)
        {
            if (cbShowP.Checked) { txtPassword.PasswordChar = '\0'; }
            else { txtPassword.PasswordChar = '*'; }
        }
        private void Login_Clicked(object sender, EventArgs e)
        {

            ReturnValue.NewAttempt(txtUsername.Text, txtPassword.Text);

            Close();
        }
        private void Register_Clicked(object sender, EventArgs e)
        {

        }

        private bool mouseDown;
        private Point lastLocation;

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }
        private void Login_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
                Location = new Point(
                    Location.X - lastLocation.X + e.X,
                    Location.Y - lastLocation.Y + e.Y
                    );
            Update();
        }
        private void Login_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}

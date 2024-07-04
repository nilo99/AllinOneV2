using AllinOne.helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllinOne
{
    public partial class Form1 : Form
    {
        private string otp;
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            pictureBox2.Hide();
            panel1.BringToFront();
            guna2DragControl1.TargetControl = panel1;
            guna2DragControl2.TargetControl = pictureBox1;
            label7.Text = "";
            label8.Text = "";
            label9.Text = "";
        }

        public bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public bool IsValidUsername(string username)
        {
            return username.Length >= 4;
        }

        public bool IsStrongPassword(string password)
        {
            if (password.Length < 8) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            if (!Regex.IsMatch(password, @"[a-z]")) return false;
            if (!Regex.IsMatch(password, @"[0-9]")) return false;
            if (!Regex.IsMatch(password, @"[\W]")) return false;
            return true;
        }


        private void label6_Click(object sender, EventArgs e)
        {
            guna2Transition1.Hide(pictureBox1);
            panel1.SendToBack();
            pictureBox2.BringToFront();
            guna2Transition2.Show(pictureBox2);
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text;
            string email = guna2TextBox2.Text;
            string password = guna2TextBox3.Text;

            FirebaseHelper firebaseHelper = new FirebaseHelper();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return;
            }

            if (!IsValidUsername(username))
            {
                return;
            }

            if (!IsValidEmail(email))
            {
                return;
            }

            if (!IsStrongPassword(password))
            {
                return;
            }

            if (await firebaseHelper.UsernameExists(username))
            {
                label7.Text = ($"Username already exists.");
                return;
            }
            else
            {
                label7.Text = "";
            }


            if (await firebaseHelper.EmailExists(email))
            {
                label8.Text = "Email already exists.";
                return;
            }

            else
            {
                label8.Text = "";
            }

            otp = GenerateOtp();
            SendOtpEmail(email, otp);

            OtpForm otpForm = new OtpForm(otp);
            if (otpForm.ShowDialog() == DialogResult.OK)
            {
                await firebaseHelper.RegisterUser(username, email, password);
                MessageBox.Show("Registration successful!");
            }
            else
            {
                MessageBox.Show("OTP verification failed.");
            }
        }

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private void SendOtpEmail(string recipientEmail, string otp)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("allinoneappv2@gmail.com", "AllinOneV2");
                    message.To.Add(new MailAddress(recipientEmail));
                    message.Subject = "Your OTP Code";
                    message.Body = $"Your OTP code is {otp}";

                    using (var client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential("allinoneappv2@gmail.com", "oolofmztxmwylyno");

                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send OTP email: {ex.Message}");
            }
        }

        private void guna2TextBox1_Leave(object sender, EventArgs e)
        {
            if(!IsValidUsername(guna2TextBox1.Text))
            {
                label7.Text = "Username must be at least 4 characters long.";
                return;
            }
            else
            {
                label7.Text = "";
            }
        }

        private void guna2TextBox2_Leave(object sender, EventArgs e)
        {
            if (!IsValidEmail(guna2TextBox2.Text))
            {
                label8.Text = "Invalid email format.";
                return;
            }
            else
            {
                label8.Text = "";
            }
        }

        private void guna2TextBox3_Leave(object sender, EventArgs e)
        {
            if (!IsStrongPassword(guna2TextBox3.Text))
            {
                label9.Text = "Password is not strong enough.";
                return;
            }

            else
            {
                label9.Text = "";
            }
        }
    }
}

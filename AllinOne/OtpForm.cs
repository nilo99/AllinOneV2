using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllinOne
{
    public partial class OtpForm : Form
    {
        private string _otp;
        private Timer _timer;
        private DateTime _expiryTime;
        public OtpForm(string otp)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            _otp = otp;
            _expiryTime = DateTime.Now.AddMinutes(3);
            StartTimer();
            guna2TextBox1.Focus();

            guna2TextBox1.KeyPress += Guna2TextBox_KeyPress;
            guna2TextBox2.KeyPress += Guna2TextBox_KeyPress;
            guna2TextBox3.KeyPress += Guna2TextBox_KeyPress;
            guna2TextBox4.KeyPress += Guna2TextBox_KeyPress;
            guna2TextBox5.KeyPress += Guna2TextBox_KeyPress;
            guna2TextBox6.KeyPress += Guna2TextBox_KeyPress;

        }

        private void Guna2TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Guna2TextBox currentTextBox = (Guna2TextBox)sender;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '\b' && currentTextBox.Text.Length == 0)
            {
                MoveFocusToPreviousTextBox(currentTextBox);
                return;
            }

            if (currentTextBox.Text.Length > 0 && e.KeyChar != '\b' && e.KeyChar != '\u007F')
            {
                e.Handled = true;
                return;
            }

            if (char.IsDigit(e.KeyChar))
            {
                MoveFocusToNextTextBox(currentTextBox);
            }
        }

        private void MoveFocusToNextTextBox(Guna2TextBox currentTextBox)
        {
            if (currentTextBox != guna2TextBox6)
            {
                SelectNextControl(currentTextBox, true, true, true, true);
            }
        }

        private void MoveFocusToPreviousTextBox(Guna2TextBox currentTextBox)
        {
            if (currentTextBox != guna2TextBox1)
            {
                SelectNextControl(currentTextBox, false, true, true, true);
            }
        }

        private void StartTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += timer1_Tick;
            _timer.Start();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string enteredOtp = $"{guna2TextBox1.Text}{guna2TextBox2.Text}{guna2TextBox3.Text}{guna2TextBox4.Text}{guna2TextBox5.Text}{guna2TextBox6.Text}";
            if (enteredOtp == _otp)
            {
                _timer.Stop();
                MessageBox.Show("OTP is valid.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please try again.");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var remainingTime = _expiryTime - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                _timer.Stop();
                MessageBox.Show("The OTP has expired. Please request a new OTP.");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                label1.Text = remainingTime.ToString(@"mm\:ss");
            } 
        }
    }
}

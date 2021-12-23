using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aerospike.Client;

namespace TestApp
{
    public partial class SignUp : Form
    {
        const string IP = "127.0.0.2";
        const int PORT = 3000;

        const string namespace_name = "test";
        const string userinfo_set_name = "users_information";
        const string image_set_name = "users_images";

        AerospikeClient client = null;

        public SignUp()
        {
            InitializeComponent();
            client = new AerospikeClient(IP, PORT);
        }



        ~SignUp()
        {
            client.Close();
        }

        private void signUpButton_Click(object sender, EventArgs e)
        {
            WritePolicy policy = new WritePolicy();

            string username = usernameTextBox.Text;
            string fullname = fullnameTextBox.Text;
            string password = passwordTextBox.Text;
            string email = emailTextBox.Text;
            string country = countryTextBox.Text;

            Key user_key = new Key(namespace_name, userinfo_set_name, username);


            Bin bin1 = new Bin("fullname", fullname);
            Bin bin2 = new Bin("password", password);
            Bin bin3 = new Bin("email", email);
            Bin bin4 = new Bin("country", country);
           
            //client = new AerospikeClient(IP, PORT);
            
            try
            {
                //Thêm tài khoản mới
                client.Put(policy, user_key, bin1, bin2, bin3, bin4);
                MessageBox.Show("Create account successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops! Something happened. Cannot create your account.\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //client.Close();

        }
    }
}

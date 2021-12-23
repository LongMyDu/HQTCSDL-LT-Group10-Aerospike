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
    public partial class SignIn : Form
    {
        const string IP = "127.0.0.2";
        const int PORT = 3000;

        const string namespace_name = "test";
        const string userinfo_set_name = "users_information";
        const string image_set_name = "users_images";

        AerospikeClient client = null;
        String username = null;

        public bool verification = false;
        public SignIn()
        {
            InitializeComponent();
            client = new AerospikeClient(IP, PORT);
        }

        public string getUsername()
        {
            return username;
        }

        private bool signIn()
        {
            QueryPolicy policy = new QueryPolicy();

            this.username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            
            Record record = client.Get(policy, new Key(namespace_name, userinfo_set_name, username));
            
            if (record.GetString("password") == password)
                verification = true;

            return verification;
        }

        private void signInButton_Click(object sender, EventArgs e)
        {
            if (signIn() == true)
            {
                Form form = new MyForm(username);
                this.Close();
            }
        }

        ~SignIn()
        {
            client.Close();
        }

        private void signUpButton_Click(object sender, EventArgs e)
        {
            (new SignUp()).Show();
        }
    }
}

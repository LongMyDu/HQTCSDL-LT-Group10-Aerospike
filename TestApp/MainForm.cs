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
    public partial class MainForm : Form
    {
        Form currentForm = null;
        AerospikeClient client = null;
        public MainForm()
        {
            InitializeComponent();

            client = new AerospikeClient("127.0.0.1", 3000);
            currentForm = new SignIn();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aerospike.Client;

namespace TestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
            SignIn signInForm = new SignIn();
            Application.Run(signInForm);

            if (signInForm.verification == true)
            {
                String username = signInForm.getUsername();

                Application.Run(new MyForm(username));
            }
           
        }
    }
}

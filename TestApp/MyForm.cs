using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Aerospike.Client;


namespace TestApp
{
    public partial class MyForm : Form
    {
        const string IP = "127.0.0.2";
        const int PORT = 3000;

        const string namespace_name = "test";
        const string userinfo_set_name = "users_information";
        const string image_set_name = "users_images";

        AerospikeClient client = null;
        String username = null;
        int imageCount = -1;
        WritePolicy policy = null;

        public MyForm(string username)
        {
            InitializeComponent();
            client = new AerospikeClient(IP, PORT);
            policy = new WritePolicy();

            this.username = username;
            this.imageCount = getImageCount();
        }

        private int getImageCount()
        {
            // = new AerospikeClient(IP, PORT);
            Record record = client.Get(policy, new Key(namespace_name, userinfo_set_name, username));
            //client.Close();

            return record.GetInt("image_count");
        }

        private bool saveImageToAerospike(byte[] BlobValue)
        {
            int next_image_index = this.imageCount + 1;
            string next_image_key = username + ":" + next_image_index.ToString();

            //MessageBox.Show(next_image_key);

            Key image_key = new Key(namespace_name, image_set_name, next_image_key);
            Key user_key = new Key(namespace_name, userinfo_set_name, username);


            Bin bin1 = new Bin("image", BlobValue);
            Bin bin2 = new Bin("date_upload", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds());
            Bin bin3 = new Bin("username", username);

            //client = new AerospikeClient(IP, PORT);
            //Thêm image mới 
            client.Put(policy, image_key, bin1, bin2, bin3);

            // Cập nhật lại imageCount của user đó
            client.Put(policy, user_key, new Bin("image_count", next_image_index));

            //client.Close();

            this.imageCount = next_image_index;
            return true;
        }

        private Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            mStream.Write(blob, 0, Convert.ToInt32(blob.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }


        private byte[] readImage(String filePath)
        {
            //A stream of bytes that represents the binary file  
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //The reader reads the binary data from the file stream  
            BinaryReader reader = new BinaryReader(fs);

            //Bytes from the binary reader stored in BlobValue array  
            byte[] BlobValue = reader.ReadBytes((int)fs.Length);

            fs.Close();
            reader.Close();

            return BlobValue;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sFileName = openFileDialog.FileName;
                  
                Bitmap img = new Bitmap(openFileDialog.FileName);
                pictureBox1.Image = img;

                filePathLabel.Text = sFileName;
            }
        }

        ~MyForm()
        {
            client.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (filePathLabel.Text.Length > 0)
            {
                saveImageToAerospike(readImage(filePathLabel.Text));
                MessageBox.Show("Saved image successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No file chosen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void myImagesButton_Click(object sender, EventArgs e)
        {
            Form imageList = new ImageSlideshow(username);
            imageList.Show();
        }
    }
}

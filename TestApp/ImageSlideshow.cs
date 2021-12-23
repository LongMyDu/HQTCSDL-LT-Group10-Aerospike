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
using System.IO;


namespace TestApp
{
    public partial class ImageSlideshow : Form
    {
        const string IP = "127.0.0.2";
        const int PORT = 3000;

        const string namespace_name = "test";
        const string userinfo_set_name = "users_information";
        const string image_set_name = "users_images";

        AerospikeClient client = null;
        string username = null;
        WritePolicy policy = null;
        int imageCount = -1;
        int current_index = -1;

        public ImageSlideshow(string username)
        {
            InitializeComponent();
            policy = new WritePolicy();

            client = new AerospikeClient(IP, PORT);

            this.username = username;
            this.imageCount = getImageCount();
        }

        private void ImageSlideshow_Load(object sender, EventArgs e)
        {
            
            current_index = nextImageIndex();
            if (current_index > 0)
                showImage();
        }

        private int nextImageIndex()
        {
            if (current_index == imageCount)
            {
                return -1;
            }

            Bitmap bm = null;
            int next_index = current_index;
            do
            {
                next_index += 1;
                bm = getImageFromAerospike(next_index);
            }
            while (bm == null && next_index < imageCount);

            if (bm == null)
                return -1;

            return next_index;
        }

        private int previousImageIndex() 
        {
            if (current_index == 1)
            {
                return -1;
            }
            Bitmap bm = null;
            int pre_index = current_index;
            do
            {
                pre_index -= 1;
                bm = getImageFromAerospike(pre_index);
            }
            while (bm == null && pre_index > 1);

            if (bm == null)
                return -1;

            return pre_index;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            int next_index = nextImageIndex();
            if (next_index != -1)
            {
                current_index = next_index;
                showImage();
            }
            else
            {
                MessageBox.Show("No more images");
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            int pre_index = previousImageIndex();
            if (pre_index != -1)
            {
                current_index = pre_index;
                showImage();
            }
            else
            {
                MessageBox.Show("No more images");
            }
        }

        private void showImage()
        {
            pictureBox1.Image = getImageFromAerospike(current_index);
            DateUploadLabel.Text = getDateUpload(current_index);
        }

        private int getImageCount()
        {
            //client = new AerospikeClient(IP, PORT);
            Record record = client.Get(policy, new Key(namespace_name, userinfo_set_name, username));
            //client.Close();
            return record.GetInt("image_count");
        }

        private Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            mStream.Write(blob, 0, Convert.ToInt32(blob.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        private string getDateUpload(int index)
        {
            //client = new AerospikeClient(IP, PORT);
            string current_img_key = username + ":" + index;
            Key image_key = new Key(namespace_name, image_set_name, current_img_key);
            String date = null;

            try
            {
                Record record = client.Get(policy, new Key(namespace_name, image_set_name, current_img_key), "date_upload");
                long seconds = record.GetLong("date_upload");

                date = DateTimeOffset.FromUnixTimeSeconds(seconds).ToString();
            } 
            catch (Exception e)
            {
            }

            //client.Close();
            return date;
        }

        private Bitmap getImageFromAerospike(int index)
        {
            //client = new AerospikeClient(IP, PORT);

            string current_img_key = username + ":" + index;
            Key image_key = new Key(namespace_name, image_set_name, current_img_key);
            Bitmap bm = null;

            try
            {
                Record record = client.Get(policy, new Key(namespace_name, image_set_name, current_img_key), "image");
                if (record != null)
                {
                    object image = record.GetValue("image");
                    bm = ByteToImage((byte[])image);
                }

               
            }
            catch (Exception e)
            {
            }

            //client.Close();
            return bm;
        }

        ~ImageSlideshow()
        {
            client.Close();
        }
    }
}

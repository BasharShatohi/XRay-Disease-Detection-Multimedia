
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageApp
{
    public class CompareForm : Form
    {
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Button loadImage1Button;
        private Button loadImage2Button;
        private Button compareButton;

        public CompareForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Compare Images";
            this.Size = new Size(800, 600);

            loadImage1Button = new Button();
            loadImage1Button.Text = "Load Image 1";
            loadImage1Button.Size = new Size(100, 30);
            loadImage1Button.Location = new Point(10, 10);
            loadImage1Button.Click += LoadImage1Button_Click;
            this.Controls.Add(loadImage1Button);

            loadImage2Button = new Button();
            loadImage2Button.Text = "Load Image 2";
            loadImage2Button.Size = new Size(100, 30);
            loadImage2Button.Location = new Point(120, 10);
            loadImage2Button.Click += LoadImage2Button_Click;
            this.Controls.Add(loadImage2Button);

            compareButton = new Button();
            compareButton.Text = "Compare";
            compareButton.Size = new Size(100, 30);
            compareButton.Location = new Point(230, 10);
            compareButton.Click += CompareButton_Click;
            this.Controls.Add(compareButton);

            pictureBox1 = new PictureBox();
            pictureBox1.Size = new Size(350, 500);
            pictureBox1.Location = new Point(10, 50);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pictureBox1);

            pictureBox2 = new PictureBox();
            pictureBox2.Size = new Size(350, 500);
            pictureBox2.Location = new Point(370, 50);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pictureBox2);
        }

        private void LoadImage1Button_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = LoadImage();
        }

        private void LoadImage2Button_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = LoadImage();
        }

        private Image LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.Title = "Select an Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return Image.FromFile(openFileDialog.FileName);
            }
            return null;
        }

        private void CompareButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && pictureBox2.Image != null)
            {
                bool areImagesSame = CompareImages(new Bitmap(pictureBox1.Image), new Bitmap(pictureBox2.Image));
                MessageBox.Show(areImagesSame ? "There is no progress in treatment or progression of the disease." : "Progress in treatment or development of the disease.");
            }
            else
            {
                MessageBox.Show("Please load both images to compare.");
            }
        }

        private bool CompareImages(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return false;
            }

            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    if (image1.GetPixel(x, y) != image2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}


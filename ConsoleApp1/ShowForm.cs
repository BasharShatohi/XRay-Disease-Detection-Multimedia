using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageApp
{
    public class ShowForm : Form
    {
        private PictureBox pictureBox;

        public ShowForm(string imagePath)
        {
            InitializeComponents();
            LoadImage(imagePath);
        }

        private void InitializeComponents()
        {
            this.Text = "Display Image";
            this.Size = new Size(800, 600);

            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Adjust the size mode as needed
            this.Controls.Add(pictureBox);
        }

        private void LoadImage(string imagePath)
        {
            pictureBox.Image = Image.FromFile(imagePath);
        }
    }
}

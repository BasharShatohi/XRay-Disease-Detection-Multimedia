using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ImageApp
{
    public class MainForm : Form
    {
        private Button openButton;
        private Button displayButton;
        private Button compareButton;
        private Button classifyButton;
        private Button searchButton;
        private Button fourierButton;
        private Button cropButton;
        private Button addTextButton;
        private Button recordAudioButton;
        private Button drawShapesButton;
        private Button createReportButton;
        private Button compressButton;
        private Button shareOnTelegramButton;
        private Button shareOnWhatsAppButton;
        private Button exitButton;

        private TelegramService telegramService;

        public MainForm()
        {
            telegramService = new TelegramService("7305160519:AAGEzSHODd-2gL2rdv6ONRwpd_nF9l_54XU", "1340367516");
            InitializeComponents();
            SetBackgroundImage();
        }

        private void InitializeComponents()
        {
            this.Text = "Image Application";
            this.Size = new Size(700, 700);
            this.BackColor = Color.White;

            openButton = CreateButton("Open Image", new Point(275, 20), OpenButton_Click);
            displayButton = CreateButton("Display Image", new Point(275, 60), DisplayButton_Click);
            compareButton = CreateButton("Compare Images", new Point(275, 100), CompareButton_Click);
            classifyButton = CreateButton("Classify Image", new Point(275, 140), ClassifyButton_Click);
            searchButton = CreateButton("Search Images", new Point(275, 180), SearchButton_Click);
            fourierButton = CreateButton("Apply Fourier Transform", new Point(275, 220), FourierButton_Click);
            cropButton = CreateButton("Crop Image", new Point(275, 260), CropButton_Click);
            addTextButton = CreateButton("Add Text", new Point(275, 300), AddTextButton_Click);
            recordAudioButton = CreateButton("Record Audio", new Point(275, 340), RecordAudioButton_Click);
            drawShapesButton = CreateButton("Draw Shapes", new Point(275, 380), DrawShapesButton_Click);
            createReportButton = CreateButton("Create Report", new Point(275, 420), CreateReportButton_Click);
            compressButton = CreateButton("Compress", new Point(275, 460), CreatecompressButton_Click);
            shareOnTelegramButton = CreateButton("Share on Telegram", new Point(275, 500), ShareOnTelegramButton_Click);
            exitButton = CreateButton("Exit", new Point(275, 540), ExitButton_Click);

            this.Controls.Add(openButton);
            this.Controls.Add(displayButton);
            this.Controls.Add(compareButton);
            this.Controls.Add(classifyButton);
            this.Controls.Add(searchButton);
            this.Controls.Add(fourierButton);
            this.Controls.Add(cropButton);
            this.Controls.Add(addTextButton);
            this.Controls.Add(recordAudioButton);
            this.Controls.Add(drawShapesButton);
            this.Controls.Add(createReportButton);
            this.Controls.Add(compressButton);
            this.Controls.Add(shareOnTelegramButton);
            this.Controls.Add(exitButton);
        }

        private Button CreateButton(string text, Point location, EventHandler clickEvent)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(120, 30);
            button.Location = location;
            button.Click += clickEvent;
            return button;
        }

        private void SetBackgroundImage()
        {
            string imagePath = "Image//new.jpg";
            if (File.Exists(imagePath))
            {
                this.BackgroundImage = Image.FromFile(imagePath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                MessageBox.Show("Failed to load background image.");
            }
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            string imagePath = OpenImage();
            if (!string.IsNullOrEmpty(imagePath))
            {
                DisplayForm displayForm = new DisplayForm(imagePath);
                displayForm.ShowDialog();
            }
        }

        private string OpenImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.Title = "Select an Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private void DisplayButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Display Button Clicked");
        }

        private void CompareButton_Click(object sender, EventArgs e)
        {
            CompareForm compareForm = new CompareForm();
            compareForm.ShowDialog();
        }

        private void ClassifyButton_Click(object sender, EventArgs e)
        {
            string imagePath = OpenImage();
            if (!string.IsNullOrEmpty(imagePath))
            {
                ClassificationForm classificationForm = new ClassificationForm(imagePath);
                classificationForm.ShowDialog();
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SearchForm searchForm = new SearchForm();
            searchForm.ShowDialog();
        }

        private void FourierButton_Click(object sender, EventArgs e)
        {
            FourierTransform fourierTransformForm = new FourierTransform();
            fourierTransformForm.Show();
        }

        private void CropButton_Click(object sender, EventArgs e)
        {
            CropForm cropForm = new CropForm();
            cropForm.ShowDialog();
        }

        private void AddTextButton_Click(object sender, EventArgs e)
        {
            AddTextForm addTextForm = new AddTextForm();
            addTextForm.ShowDialog();
        }

        private void RecordAudioButton_Click(object sender, EventArgs e)
        {
            AudioForm audioForm = new AudioForm();
            audioForm.ShowDialog();
        }

        private void DrawShapesButton_Click(object sender, EventArgs e)
        {
            DrawShapesForm drawShapesForm = new DrawShapesForm();
            drawShapesForm.ShowDialog();
        }

        private void CreateReportButton_Click(object sender, EventArgs e)
        {
            CreateReportForm createReportForm = new CreateReportForm();
            createReportForm.ShowDialog();
        }

        private void CreatecompressButton_Click(object sender, EventArgs e)
        {
            using (CreatecompressForm createcompressForm = new CreatecompressForm())
            {
                createcompressForm.ShowDialog();
            }
        }

        private async void ShareOnTelegramButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files|*.*",
                Title = "Select a File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                await telegramService.ShareFileAsync(filePath);
            }
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ImageApp
{
    public class CreateReportForm : Form
    {
        private PictureBox pictureBox;
        private TextBox reportTextBox;
        private Button loadImageButton;
        private Button saveReportButton;
        private Button exportButton;
        private string loadedImagePath;

        public CreateReportForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Create Medical Report";
            this.Size = new Size(900, 600);

            loadImageButton = new Button();
            loadImageButton.Text = "Load Image";
            loadImageButton.Size = new Size(100, 30);
            loadImageButton.Location = new Point(50, 10);
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            reportTextBox = new TextBox();
            reportTextBox.Multiline = true;
            reportTextBox.Size = new Size(600, 100);
            reportTextBox.Location = new Point(50, 50);
            this.Controls.Add(reportTextBox);

            saveReportButton = new Button();
            saveReportButton.Text = "Save Report as TXT";
            saveReportButton.Size = new Size(150, 30);
            saveReportButton.Location = new Point(670, 50);
            saveReportButton.Click += SaveReportButton_Click;
            this.Controls.Add(saveReportButton);

            exportButton = new Button();
            exportButton.Text = "Save Report as PDF";
            exportButton.Size = new Size(150, 30);
            exportButton.Location = new Point(670, 90);
            exportButton.Click += ExportButton_Click;
            this.Controls.Add(exportButton);

            pictureBox = new PictureBox();
            pictureBox.Size = new Size(700, 300);
            pictureBox.Location = new Point(50, 160);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pictureBox);
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.Title = "Select an Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                loadedImagePath = openFileDialog.FileName;
                pictureBox.Image = Image.FromFile(loadedImagePath);
            }
        }

        private void SaveReportButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(loadedImagePath) && !string.IsNullOrEmpty(reportTextBox.Text))
            {
                string reportFileName = Path.Combine(Path.GetDirectoryName(loadedImagePath), Path.GetFileNameWithoutExtension(loadedImagePath) + ".txt");
                File.WriteAllText(reportFileName, reportTextBox.Text);
                MessageBox.Show($"Report saved as {reportFileName}");
            }
            else
            {
                MessageBox.Show("Please load an image and write a report.");
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(loadedImagePath) && !string.IsNullOrEmpty(reportTextBox.Text))
            {
                string baseFileName = Path.Combine(Path.GetDirectoryName(loadedImagePath), Path.GetFileNameWithoutExtension(loadedImagePath));

                // Save as TXT
                string txtFileName = baseFileName + ".txt";
                File.WriteAllText(txtFileName, reportTextBox.Text);

                // Save as PDF
                string pdfFileName = baseFileName + ".pdf";
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Add image to PDF
                XImage pdfImage = XImage.FromFile(loadedImagePath);
                gfx.DrawImage(pdfImage, 0, 0, page.Width, page.Height / 2);

                // Add report text to PDF
                XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
                gfx.DrawString(reportTextBox.Text, font, XBrushes.Black, new XRect(0, page.Height / 2, page.Width, page.Height / 2), XStringFormats.TopLeft);

                document.Save(pdfFileName);

                MessageBox.Show($"Report exported as {txtFileName} and {pdfFileName}");
            }
            else
            {
                MessageBox.Show("Please load an image and write a report.");
            }
        }
    }
}

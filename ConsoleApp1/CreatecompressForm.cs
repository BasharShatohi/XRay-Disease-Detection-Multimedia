using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace ImageApp
{
    public class CreatecompressForm : Form
    {
        private RadioButton deleteAfterCompressRadioButton;
        private RadioButton keepAfterCompressRadioButton;
        private ComboBox compressionTypeComboBox;

        public CreatecompressForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Compress Files";
            this.Size = new Size(450, 300);

            Font controlFont = new Font("Arial", 12, FontStyle.Regular);

            Button compressButton = new Button();
            compressButton.Text = "Compress";
            compressButton.Size = new Size(150, 60);
            compressButton.Location = new Point(125, 200);
            compressButton.Font = controlFont;
            compressButton.Click += CompressButton_Click;
            this.Controls.Add(compressButton);

            deleteAfterCompressRadioButton = new RadioButton();
            deleteAfterCompressRadioButton.Text = "Compress and Delete";
            deleteAfterCompressRadioButton.Size = new Size(200, 40);
            deleteAfterCompressRadioButton.Location = new Point(50, 30);
            deleteAfterCompressRadioButton.Font = controlFont;
            this.Controls.Add(deleteAfterCompressRadioButton);

            keepAfterCompressRadioButton = new RadioButton();
            keepAfterCompressRadioButton.Text = "Compress Only";
            keepAfterCompressRadioButton.Size = new Size(200, 40);
            keepAfterCompressRadioButton.Location = new Point(50, 80);
            keepAfterCompressRadioButton.Font = controlFont;
            this.Controls.Add(keepAfterCompressRadioButton);

            keepAfterCompressRadioButton.Checked = true;

            Label compressionTypeLabel = new Label();
            compressionTypeLabel.Text = "Compression Type:";
            compressionTypeLabel.Size = new Size(200, 40);
            compressionTypeLabel.Location = new Point(50, 130);
            compressionTypeLabel.Font = controlFont;
            this.Controls.Add(compressionTypeLabel);

            compressionTypeComboBox = new ComboBox();
            compressionTypeComboBox.Items.AddRange(new string[] { "ZIP", "RAR", "RAR4" });
            compressionTypeComboBox.Size = new Size(150, 40);
            compressionTypeComboBox.Location = new Point(250, 130);
            compressionTypeComboBox.Font = controlFont;
            compressionTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(compressionTypeComboBox);
            compressionTypeComboBox.SelectedIndex = 0;
        }

        private void CompressButton_Click(object sender, EventArgs e)
        {
            CompressFiles(deleteAfterCompressRadioButton.Checked, compressionTypeComboBox.SelectedItem.ToString());
        }

        public void CompressFiles(bool deleteAfterCompress, string selectedCompressionType)
        {
            string imagePath = OpenImage();
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("No image selected.");
                return;
            }

            string directory = Path.GetDirectoryName(imagePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);

            string[] extensions = new[] { ".txt", ".pdf", ".mp3", ".wav", ".ogg", ".png", ".jpg" };
            var relatedFiles = new List<string>();

            foreach (var ext in extensions)
            {
                string[] relatedFilePatterns = new[]
                {
                    Path.Combine(directory, fileNameWithoutExtension + ext),
                    Path.Combine(directory, fileNameWithoutExtension + "_*" + ext)
                };

                foreach (var pattern in relatedFilePatterns)
                {
                    foreach (var file in Directory.GetFiles(directory, Path.GetFileName(pattern)))
                    {
                        if (File.Exists(file))
                        {
                            relatedFiles.Add(file);
                        }
                    }
                }
            }
            relatedFiles.Add(imagePath);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"{selectedCompressionType} files (*.{selectedCompressionType.ToLower()})|*.{selectedCompressionType.ToLower()}";
            saveFileDialog.Title = "Save Compressed File";
            saveFileDialog.FileName = fileNameWithoutExtension + "." + selectedCompressionType.ToLower();

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (selectedCompressionType == "ZIP")
                    {
                        // Compress as ZIP
                        using (FileStream zipToOpen = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                            {
                                foreach (var file in relatedFiles)
                                {
                                    archive.CreateEntryFromFile(file, Path.GetFileName(file));
                                }
                            }
                        }
                    }
                    else if (selectedCompressionType == "RAR" || selectedCompressionType == "RAR4")
                    {
                        // Compress as RAR or RAR4
                        string winRarPath = @"C:\Program Files\WinRAR\WinRAR.exe";
                        if (!File.Exists(winRarPath))
                        {
                            MessageBox.Show("WinRAR is not installed or the path is incorrect.");
                            return;
                        }

                        string rarOptions = selectedCompressionType == "RAR" ? "-r" : "-r -ma4";
                        string arguments = $"a {rarOptions} \"{saveFileDialog.FileName}\"";

                        foreach (var file in relatedFiles)
                        {
                            arguments += $" \"{file}\"";
                        }

                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = winRarPath,
                            Arguments = arguments,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            CreateNoWindow = true,
                        };

                        using (Process process = Process.Start(startInfo))
                        {
                            process.WaitForExit();
                        }
                    }
                    MessageBox.Show("Files compressed successfully.");

                    if (deleteAfterCompress)
                    {
                        foreach (var file in relatedFiles)
                        {
                            File.Delete(file);
                        }
                        MessageBox.Show("Original files deleted successfully.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error compressing files: " + ex.Message);
                }
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

       

    } 
}

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageApp
{
    internal class TelegramService
    {
        private readonly string _token;
        private readonly string _chatId;

        public TelegramService(string token, string chatId)
        {
            _token = token;
            _chatId = chatId;
        }

        public async Task ShareFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please select a file first.");
                return;
            }

            string telegramApiUrl = $"https://api.telegram.org/bot{_token}/sendDocument";

            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            using (var fileStream = File.OpenRead(filePath))
            {
                form.Add(new StringContent(_chatId), "chat_id");
                form.Add(new StreamContent(fileStream), "document", Path.GetFileName(filePath));

                var response = await client.PostAsync(telegramApiUrl, form);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("File shared on Telegram successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to share file on Telegram.");
                }
            }
        }
    }
}

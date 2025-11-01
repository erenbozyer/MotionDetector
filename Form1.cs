using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OpenCvSharp;

namespace MotionDetector
{
    public partial class Form1 : Form
    {
        private VideoCapture capture;
        private System.Windows.Forms.Timer frameTimer;
        private bool isRunning = false;
        private BackgroundSubtractorMOG2 bgSubtractor;
        private DateTime lastSentAt = DateTime.MinValue;

        public Form1()
        {
            InitializeComponent();
            bgSubtractor = BackgroundSubtractorMOG2.Create(history: 500, varThreshold: 16, detectShadows: false);
            frameTimer = new System.Windows.Forms.Timer { Interval = 33 };
            frameTimer.Tick += OnFrame;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (!isRunning) StartCamera(); else StopCamera();
        }

        private void StartCamera()
        {
            try
            {
                capture = new VideoCapture(0);
                if (!capture.IsOpened())
                {
                    lblStatus.Text = "❌ Kamera açılamadı";
                    return;
                }
                isRunning = true;
                btnToggle.Text = "DURDUR";
                lblStatus.Text = "🎥 Çalışıyor";
                frameTimer.Start();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Hata: {ex.Message}";
            }
        }

        private void StopCamera()
        {
            frameTimer.Stop();
            isRunning = false;
            btnToggle.Text = "BAŞLA";
            lblStatus.Text = "⏹️ Durduruldu";
            capture?.Release();
            capture?.Dispose();
        }

        private void OnFrame(object sender, EventArgs e)
        {
            if (capture == null || !capture.IsOpened()) return;

            using var frame = new Mat();
            if (!capture.Read(frame) || frame.Empty()) return;

            using var fgMask = new Mat();
            bgSubtractor.Apply(frame, fgMask);
            Cv2.Threshold(fgMask, fgMask, 200, 255, ThresholdTypes.Binary);
            using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
            Cv2.MorphologyEx(fgMask, fgMask, MorphTypes.Open, kernel);
            Cv2.Dilate(fgMask, fgMask, kernel, iterations: 2);

            var contours = Cv2.FindContoursAsArray(fgMask, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            bool motionDetected = false;
            foreach (var contour in contours)
            {
                if (Cv2.ContourArea(contour) < 500) continue;
                var rect = Cv2.BoundingRect(contour);
                Cv2.Rectangle(frame, rect, new Scalar(0, 0, 255), 2);
                motionDetected = true;
            }

            picturePreview.Image?.Dispose();
            picturePreview.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame);

            if (motionDetected)
            {
                if ((DateTime.UtcNow - lastSentAt).TotalSeconds >= 10) // 10 saniye cooldown
                {
                    lastSentAt = DateTime.UtcNow;
                    _ = SendEmailAsync(frame);
                    lblStatus.Text = "📧 Mail gönderiliyor...";
                }
            }
        }

        private async Task SendEmailAsync(Mat frame)
        {
            try
            {
                var tmpDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "MotionShots");
                Directory.CreateDirectory(tmpDir);
                var filePath = Path.Combine(tmpDir, $"motion_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
                Cv2.ImWrite(filePath, frame);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Motion Detector", "from@example.com"));
                message.To.Add(new MailboxAddress("User", "to@example.com"));
                message.Subject = "Hareket Algılandı";
                var builder = new BodyBuilder { TextBody = "Hareket algılandı!" };
                builder.Attachments.Add(filePath);
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.example.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("from@example.com", "password");
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                lblStatus.Text = "✅ Mail gönderildi";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Mail hatası: {ex.Message}";
            }
        }
    }
}

using System;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace RSA_sign
{
    public partial class MainForm : Form
    {
        private TextBox txtMessage;
        private TextBox txtPublicKey;
        private TextBox txtSignature;
        private TextBox txtResult;
        private Button btnCreateSign;
        private Button btnVerifySign;
        private Label lblMessage;
        private Label lblPublicKey;
        private Label lblSignature;
        private Label lblResult;
        private RSACryptoServiceProvider rsa;
        private string currentPublicKey;
        private byte[] currentSignature;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Електронно-цифровий підпис RSA";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Label для повідомлення
            lblMessage = new Label();
            lblMessage.Text = "Повідомлення:";
            lblMessage.Location = new System.Drawing.Point(20, 20);
            lblMessage.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(lblMessage);

            // TextBox для повідомлення
            txtMessage = new TextBox();
            txtMessage.Location = new System.Drawing.Point(20, 45);
            txtMessage.Size = new System.Drawing.Size(740, 23);
            txtMessage.Text = "Hello world!";
            this.Controls.Add(txtMessage);

            // Кнопка створення підпису
            btnCreateSign = new Button();
            btnCreateSign.Text = "Створити підпис";
            btnCreateSign.Location = new System.Drawing.Point(20, 80);
            btnCreateSign.Size = new System.Drawing.Size(150, 30);
            btnCreateSign.Click += BtnCreateSign_Click;
            this.Controls.Add(btnCreateSign);

            // Label для публічного ключа
            lblPublicKey = new Label();
            lblPublicKey.Text = "Публічний ключ:";
            lblPublicKey.Location = new System.Drawing.Point(20, 120);
            lblPublicKey.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(lblPublicKey);

            // TextBox для публічного ключа
            txtPublicKey = new TextBox();
            txtPublicKey.Location = new System.Drawing.Point(20, 145);
            txtPublicKey.Size = new System.Drawing.Size(740, 23);
            txtPublicKey.Multiline = true;
            txtPublicKey.ScrollBars = ScrollBars.Vertical;
            txtPublicKey.Height = 80;
            txtPublicKey.ReadOnly = true;
            this.Controls.Add(txtPublicKey);

            // Label для підпису
            lblSignature = new Label();
            lblSignature.Text = "Цифровий підпис:";
            lblSignature.Location = new System.Drawing.Point(20, 235);
            lblSignature.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(lblSignature);

            // TextBox для підпису
            txtSignature = new TextBox();
            txtSignature.Location = new System.Drawing.Point(20, 260);
            txtSignature.Size = new System.Drawing.Size(740, 23);
            txtSignature.Multiline = true;
            txtSignature.ScrollBars = ScrollBars.Vertical;
            txtSignature.Height = 80;
            txtSignature.ReadOnly = true;
            this.Controls.Add(txtSignature);

            // Кнопка перевірки підпису
            btnVerifySign = new Button();
            btnVerifySign.Text = "Перевірити підпис";
            btnVerifySign.Location = new System.Drawing.Point(190, 80);
            btnVerifySign.Size = new System.Drawing.Size(150, 30);
            btnVerifySign.Click += BtnVerifySign_Click;
            this.Controls.Add(btnVerifySign);

            // Label для результату
            lblResult = new Label();
            lblResult.Text = "Результат:";
            lblResult.Location = new System.Drawing.Point(20, 350);
            lblResult.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(lblResult);

            // TextBox для результату
            txtResult = new TextBox();
            txtResult.Location = new System.Drawing.Point(20, 375);
            txtResult.Size = new System.Drawing.Size(740, 100);
            txtResult.Multiline = true;
            txtResult.ScrollBars = ScrollBars.Vertical;
            txtResult.ReadOnly = true;
            this.Controls.Add(txtResult);
        }

        private void BtnCreateSign_Click(object sender, EventArgs e)
        {
            try
            {
                txtResult.Clear();
                txtResult.AppendText("<<< Створення підпису >>>\r\n\r\n");

                // Створюється контейнер з ключами за замовчуванням
                CspParameters signParam = new CspParameters();
                signParam.KeyContainerName = "Bob";
                rsa = new RSACryptoServiceProvider(signParam);

                // Публічний ключ експортується для передачі іншій стороні в XML-форматі
                currentPublicKey = rsa.ToXmlString(false);
                txtPublicKey.Text = currentPublicKey;
                txtResult.AppendText("Публічний ключ створено.\r\n");

                // Вхідне повідомлення представляється у вигляді байтової послідовності
                byte[] message = Encoding.UTF8.GetBytes(txtMessage.Text);
                txtResult.AppendText($"Повідомлення (HEX): {BitConverter.ToString(message)}\r\n");

                // Створюється дайджест повідомлення за алгоритмом SHA-1
                SHA1 sha1 = SHA1.Create();
                byte[] hmessage = sha1.ComputeHash(message);
                txtResult.AppendText($"Геш (SHA-1): {BitConverter.ToString(hmessage)}\r\n");

                // Створюється підпис для дайджесту
                currentSignature = rsa.SignHash(hmessage, "sha1");
                txtSignature.Text = BitConverter.ToString(currentSignature);
                txtResult.AppendText($"\r\nПідпис створено успішно!\r\n");
                txtResult.AppendText($"Підпис (HEX): {BitConverter.ToString(currentSignature)}\r\n");
                txtResult.AppendText($"Підпис (Base64): {Convert.ToBase64String(currentSignature)}\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при створенні підпису: {ex.Message}", "Помилка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVerifySign_Click(object sender, EventArgs e)
        {
            try
            {
                txtResult.Clear();
                txtResult.AppendText("<<< Перевірка підпису >>>\r\n\r\n");

                if (string.IsNullOrEmpty(txtPublicKey.Text))
                {
                    MessageBox.Show("Спочатку створіть підпис!", "Помилка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtSignature.Text))
                {
                    MessageBox.Show("Підпис відсутній!", "Помилка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Вхідне повідомлення представляється у вигляді байтової послідовності
                byte[] message = Encoding.UTF8.GetBytes(txtMessage.Text);
                txtResult.AppendText($"Повідомлення (HEX): {BitConverter.ToString(message)}\r\n");

                // Створюється дайджест повідомлення за алгоритмом SHA-1
                SHA1 sha1 = SHA1.Create();
                byte[] hmessage = sha1.ComputeHash(message);
                txtResult.AppendText($"Геш (SHA-1): {BitConverter.ToString(hmessage)}\r\n");

                // Створюється об'єкт RSA, до якого експортується публічний ключ
                RSACryptoServiceProvider rsaVerify = new RSACryptoServiceProvider();
                rsaVerify.FromXmlString(txtPublicKey.Text);

                // Конвертуємо підпис з HEX або Base64
                byte[] signature;
                if (txtSignature.Text.Contains("-"))
                {
                    // HEX формат
                    string[] hexValues = txtSignature.Text.Split('-');
                    signature = new byte[hexValues.Length];
                    for (int i = 0; i < hexValues.Length; i++)
                    {
                        signature[i] = Convert.ToByte(hexValues[i], 16);
                    }
                }
                else
                {
                    // Base64 формат
                    signature = Convert.FromBase64String(txtSignature.Text);
                }

                txtResult.AppendText($"Підпис для перевірки: {Convert.ToBase64String(signature)}\r\n");

                // Перевіряється підпис для дайджесту
                bool match = rsaVerify.VerifyHash(hmessage, "sha1", signature);
                
                txtResult.AppendText($"\r\nРезультат перевірки: {(match ? "ПІДПИС ВАЛІДНИЙ ✓" : "ПІДПИС НЕВАЛІДНИЙ ✗")}\r\n");
                
                if (match)
                {
                    MessageBox.Show("Підпис валідний! Повідомлення не змінено.", "Успіх", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Підпис невалідний! Повідомлення могло бути змінено.", "Помилка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при перевірці підпису: {ex.Message}", "Помилка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


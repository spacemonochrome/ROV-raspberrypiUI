using Renci.SshNet;
using System;
using System.IO;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace rov_arayuz
{
    public partial class Form1 : Form
    {
        public string host;
        public string username;
        public string password;
        public string command;
        public SshClient client;
        public SftpClient ftpClient;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string yazi_String = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "rov_log/motor_isim.txt");
                string[] _Split = yazi_String.Split('&');
                textBox17.Text = _Split[1];
                textBox16.Text = _Split[2];
                textBox15.Text = _Split[3];
                textBox14.Text = _Split[4];
                textBox13.Text = _Split[5];
                textBox12.Text = _Split[6];

                string login_String = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "rov_log/login.txt");
                string[] log_Split = login_String.Split('&');
                textBox2.Text = log_Split[1];
                textBox3.Text = log_Split[2];
                textBox4.Text = log_Split[3];
            }
            catch (Exception)
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "rov_log");
                string dosya_yolu = AppDomain.CurrentDomain.BaseDirectory + "rov_log/motor_isim.txt";
                var ab = File.Create(dosya_yolu);
                ab.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                host = textBox2.Text;
                username = textBox3.Text;
                password = textBox4.Text;
                client = new SshClient(host, username, password);
                ftpClient = new SftpClient(host, username, password);
                ftpClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(1);
                client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(1);
                ftpClient.Connect();                
                client.Connect();
                if (client.IsConnected)
                {
                    pictureBox4.Image = global::rov_arayuz.Properties.Resources.greentick;
                    label7.Text = "Bağlandı";
                    button2.Enabled = true;
                    button1.Enabled = false;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    button5.Enabled = true;
                    button6.Enabled = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Bağlanamadı! \n");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                command = textBox1.Text;
                var commandResult = client.RunCommand(command);
                textBox18.Text += Convert.ToString(commandResult.Result) + Environment.NewLine;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + "");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                client.Disconnect();
                ftpClient.Dispose();
                pictureBox4.Image = global::rov_arayuz.Properties.Resources.rederror;
                label7.Text = "Bağlantı yok";
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + "");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string deger = "yol_deger = " + textBox5.Text + "," + textBox6.Text + "," + textBox7.Text + "," + textBox8.Text + "," + textBox9.Text + "," + textBox10.Text + Environment.NewLine + "saniye = " + textBox11.Text;
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "rov_log");    
            string dosya_yolu = AppDomain.CurrentDomain.BaseDirectory + "rov_log/deger.py";
            var ab = File.Create(dosya_yolu);
            ab.Close();
            File.WriteAllText(dosya_yolu, deger);
            using (FileStream fs = new FileStream(dosya_yolu, FileMode.Open))
            {
                ftpClient.ChangeDirectory("/home/hasan/Desktop");
                ftpClient.UploadFile(fs, Path.GetFileName(dosya_yolu));
                textBox18.Text += dosya_yolu + " yüklendi." + Environment.NewLine; ;
            }
            var commandResult = client.RunCommand("python Desktop/dev-haberlesme.py");
            textBox18.Text += Convert.ToString(commandResult.Result) + Environment.NewLine;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var commandResult= client.RunCommand("ls -l Desktop");
            textBox18.Text += Convert.ToString(commandResult.Result) + Environment.NewLine;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.RestoreDirectory = true;
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in dialog.FileNames)
                    {
                        if (file.EndsWith("dev-haberlesme.py") == false)
                        {
                            using (FileStream fs = new FileStream(file, FileMode.Open))
                            {
                                ftpClient.ChangeDirectory("/home/hasan/Desktop");
                                ftpClient.UploadFile(fs, Path.GetFileName(file));
                                textBox18.Text += file + " yüklendi." + Environment.NewLine;
                            }
                        }                   
                        else { MessageBox.Show("Bu dosya gönderilemez."); }
                    }                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void log_kayit()
        {
            string deger = "&" +textBox2.Text + "&" + textBox3.Text + "&" + textBox4.Text;
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "rov_log");
            string dosya_yolu = AppDomain.CurrentDomain.BaseDirectory + "rov_log/login.txt";
            var ab = File.Create(dosya_yolu);
            ab.Close();
            File.WriteAllText(dosya_yolu, deger);
        }

        public void motor_kayit()
        {
            string deger = "&" + textBox17.Text + "&" + textBox16.Text + "&" + textBox15.Text + "&" + textBox14.Text + "&" + textBox13.Text + "&" + textBox12.Text;
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "rov_log");
            string dosya_yolu = AppDomain.CurrentDomain.BaseDirectory + "rov_log/motor_isim.txt";
            var ab = File.Create(dosya_yolu);
            ab.Close();
            File.WriteAllText(dosya_yolu, deger);
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            motor_kayit();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            motor_kayit();
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            motor_kayit();
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            motor_kayit();
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            motor_kayit();
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            motor_kayit();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            log_kayit();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            log_kayit();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            log_kayit();
        }
    }
}

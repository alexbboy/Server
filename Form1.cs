using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MulticastServer
{
    public partial class Form1 : Form
    {
        Socket socket;
        IPAddress ip;
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            this.Text = "Сервер (выключен)";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try { ip = IPAddress.Parse(textBox1.Text); }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Пожалуйста, введите IP", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (FormatException)
            {
                MessageBox.Show("Пожалуйста, введите корректный IP", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 5);
            }
            catch (SocketException)
            {
                MessageBox.Show("Пожалуйста, введите корректный IP (для многоадресной передачи используйте IP в диапозоне от 224.0.0.0 до 239.255.255.255)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IPEndPoint endPoint;
            try
            {
                endPoint = new IPEndPoint(ip, int.Parse(textBox2.Text));
            }
            catch(ArgumentOutOfRangeException)
            {
                MessageBox.Show("Пожалуйста, введите корректный порт", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch(System.FormatException)
            {
                MessageBox.Show("Пожалуйста, введите порт", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            socket.Connect(endPoint);
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            this.Text = "Сервер (включен)";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileInfo file = new FileInfo("C:\\Users\\Ксения Лучкова\\OneDrive\\Документы\\6 семестр\\Сети ЭВМ\\Курсовая\\test.txt");
            long size = file.Length;
            byte[] data = new byte[size];
            byte[] name = new byte[255];
            string path = textBox3.Text;
            if (path.Substring(path.Length-4)!=".txt")
            {
                MessageBox.Show("Программа предназначена для передачи текстовых файлов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FileStream fstream;
            try
            {
                fstream = new FileStream(path, FileMode.Open);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Пожалуйста, введите адрес передаваемого файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Указан несуществующий путь к файлу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Указанного файла не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show("Данный файл не доступен по соображениям безопасности", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string filename = Path.GetFileName(path);
            fstream.Read(data, 0, data.Length);
            fstream.Close();
            name = Encoding.Default.GetBytes(filename);
            socket.Send(name, name.Length, SocketFlags.None);
            socket.Send(data, data.Length, SocketFlags.None);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            socket.Close();
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            this.Text = "Сервер (выключен)";

        }
    }
}

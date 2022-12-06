using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetDz1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool logIn = false;
        TcpClient tcpClient;
        public MainWindow()
        {
            InitializeComponent();
        }

        async public void Button_Click(object sender, RoutedEventArgs e)
        {
            // получаем поток для взаимодействия с сервером          
            var tcpListener = new TcpListener(IPAddress.Any, 8888);
            try
            {
                tcpListener.Start();    // запускаем сервер
                MessageBox.Show("Сервер запущен. Ожидание подключений... ");
                while (true)
                {
                    // получаем подключение в виде TcpClient
                    tcpClient = await tcpListener.AcceptTcpClientAsync();
                    MessageBox.Show($"Входящее подключение: {tcpClient.Client.RemoteEndPoint}");
                    NetworkStream stream = tcpClient.GetStream();
                    var responseData = new byte[512];
                    // StringBuilder для склеивания полученных данных в одну строку
                    var response = new StringBuilder();
                    int bytes;  // количество полученных байтов
                    do
                    {
                        // получаем данные
                        bytes = await stream.ReadAsync(responseData);
                        // преобразуем в строку и добавляем ее в StringBuilder
                        response.Append(Encoding.UTF8.GetString(responseData, 0, bytes));
                        ListBoxServer.Items.Add(response);
                        if(Convert.ToString(response) == "Bye")
                        {
                            tcpClient.Close();
                            MessageBox.Show("Подключение закрыто");
                            logIn = false;
                        }
                    }
                    while (bytes > 0); // пока данные есть в потоке 
                    {
                        // выводим данные
                        ListBoxServer.Items.Add(response);
                    }
                }                
            }
            finally
            {
                tcpListener.Stop(); // останавливаем сервер
            }
        }

        async private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (logIn == true)
            {
                using TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", 8888);
                Console.WriteLine("Подключение установлено");
                NetworkStream stream = tcpClient.GetStream();
                //// отправляем сообщение для отправки
                var message = TextClient.Text;
                ListBoxClient.Items.Add(TextClient.Text);
                // кодируем его в массив байт
                var data = Encoding.UTF8.GetBytes(message);
                // отправляем массив байт на сервер 
                await stream.WriteAsync(data);
                MessageBox.Show($"Данные отправлены на сервер");

                var responseData = new byte[512];
                // StringBuilder для склеивания полученных данных в одну строку
                var response = new StringBuilder();
                int bytes;  // количество полученных байтов
                do
                {
                    // получаем данные
                    bytes = await stream.ReadAsync(responseData);
                    // преобразуем в строку и добавляем ее в StringBuilder
                    response.Append(Encoding.UTF8.GetString(responseData, 0, bytes));
                    ListBoxClient.Items.Add(response);
                }
                while (bytes > 0); // пока данные есть в потоке 
                    // выводим данные
                    ListBoxClient.Items.Add(response);
            }
            else
            {
                MessageBox.Show("Подключитесь к серверу");
            }
        }

        async private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ip.Text == "127.0.0.1" & port.Text == "8888")
            {
                logIn = true;
                MessageBox.Show("Вы подключенны к серверу");
            }
            else
            {
                MessageBox.Show("Неправельно введены данные");
            }
        }


        async private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NetworkStream stream = tcpClient.GetStream();
            string reply = TextServer.Text;
            ListBoxServer.Items.Add(reply);
            byte[] msg = Encoding.UTF8.GetBytes(reply);
            await stream.WriteAsync(msg);          
        }
    }
}

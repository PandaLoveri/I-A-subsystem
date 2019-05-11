using System.Windows;
using System.Data;
using System.Text;
using System;

namespace IA_subsystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!SQLCon.ConnectToDatabase())
            {
                Close();
            }
            
            //Если в таблице аккаунтов нет записей (т.е. самый первый вход), то переход на окно регистрации администратора
            //"SELECT COUNT(*) FROM tblAccounts"-запрос на количество всех записей в таблице
            if (SQLCon.ExecuteScalar("SELECT COUNT(*) FROM tblAccounts", out object o) && (int)o == 0)
            {
                Registration startWindow = new Registration(true,false);
                startWindow.ShowDialog();
                if (!startWindow.Success)
                {
                    Close();
                }
            }            
        }
                    
        ///Обработчик события нажатия на вкладку окна "О программе"        
        private void MAbout_Click(object sender, RoutedEventArgs e)
        {
            Tabs.AboutProgram();            
        }

        //Обработчик события нажатия на вкладку окна "Помощь"
        private void MHelp_Click(object sender, RoutedEventArgs e)
        {
            Tabs.Help();
        }

        /// <summary>
        ///Событие нажатия на кнопку окна "Вход"
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            //Проверка корректности введеного логина
            if (!User.IsCorrectName(tbxUserName.Text, out string message))
            {
                MessageBox.Show(message);
                tbxUserName.Focus();
            }
            //Проверка введен ли пароль
            else if (string.IsNullOrEmpty(pbxPassword.Password))
            {
                MessageBox.Show("Введите пароль!");
                pbxPassword.Focus();
            }           
            
            else
            {
                //Проверка подключения к БД
                if (!SQLCon.ConnectToDatabase())
                {
                    Close();
                }

                //Получим хэш-пароль, применив алгоритм ГОСТ 34.11-2012
                byte[] password = Encoding.GetEncoding(1251).GetBytes(pbxPassword.Password);
                GOST o = new GOST(512/*256*/);
                byte[] hash_code = o.H(password);
                string hashpassword = BitConverter.ToString(hash_code);
                
                //Получаем таблицу с учетными данными пользователя из БД, где найдутся такой логин и пароль в одной строке
                DataTable dateTable = SQLCon.GetDateTable($"SELECT * FROM tblAccounts WHERE UserName='{tbxUserName.Text}' AND Password='{hashpassword}';");
                if (dateTable != null && dateTable.Rows.Count != 0)
                {
                    User.Name = dateTable.Rows[0]["UserName"] as string;
                    User.Password = dateTable.Rows[0]["Password"] as string;
                    User.IsAdmin = (bool)dateTable.Rows[0]["IsAdmin"];
                    AccessWindow accessWindow = new AccessWindow();
                    Close();
                    accessWindow.ShowDialog();
                    //MessageBox.Show("Доступ разрешен.");
                }
                else
                {
                    MessageBox.Show("Доступ запрещен. Введен невенрный логин или пароль.");
                }                
            }            
        }
    }
}

using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Text;
using System;
using System.Data;
using System.Windows.Data;

namespace IA_subsystem
{
    /// <summary>
    /// Окно регистрации администратора при первичном обращении к подсистеме
    /// </summary>
    public partial class Registration : Window
    {
        public bool AsAdmin;
        public bool IsChangePassword;
        public bool Success;

        public Registration(bool asAdmin, bool isChangePassword)
        {
            InitializeComponent();
            AsAdmin = asAdmin;
            IsChangePassword = isChangePassword;
            btnLogin.Content = "Зарегистрировать";
            if (!IsChangePassword)
                lblRegistration.Content = AsAdmin ? "Регистарция администратора" : "Регистарция пользователя";
            else
            {
                btnLogin.Content = "Изменить пароль";
                lblRegistration.Content = "Изменение пароля";
                spName.Visibility = Visibility.Collapsed;
            }
        }

        //Обработчик события нажатия на вкладку окна "О программе"
        private void MAbout_Click(object sender, RoutedEventArgs e)
        {
            Tabs.AboutProgram();
        }

        //Обработчик события нажатия на вкладку окна "Помощь"
        private void MHelp_Click(object sender, RoutedEventArgs e)
        {
            Tabs.Help();
        }

        //Обработчик события нажатия на кнопку "Зарегистрировать"/"Изменить пароль"
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            object o;

            //Проверка на заполненость всех полей ввода 
            if (!IsChangePassword && !User.IsCorrectName(tbxName.Text, out string message))
                {
                    MessageBox.Show(message);                    
                    tbxName.Focus();
                }
            else if (string.IsNullOrEmpty(pbxPassword1.Password))
                {
                    MessageBox.Show("Введите пароль!");
                    pbxPassword1.Focus();
                }
            else if (string.IsNullOrEmpty(pbxPassword2.Password))
                {
                    MessageBox.Show("Повторите пароль!");
                    pbxPassword1.Focus();
                }
            //Проверка на совпадение введенных паролей
            else if (pbxPassword1.Password!= pbxPassword2.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                pbxPassword1.Focus();
            }

            //Если пройдены все предварительные проверки, выполнение регистрации
            else if (SQLCon.ExecuteScalar($"SELECT COUNT(*) FROM tblAccounts WHERE UserName='{tbxName.Text}'", out o))
            {                
                //Проверка на наличие в БД пользователя с таким же логином(по sql-запросу выше)
                if (!IsChangePassword && (int)o != 0)
                {
                    MessageBox.Show("Пользователь с таким именем уже существует.");
                }
                else 
                {
                    //Получим хэш-пароль, применив алгоритм ГОСТ 34.11-2012
                    byte[] password = Encoding.GetEncoding(1251).GetBytes(pbxPassword1.Password);
                    GOST g = new GOST(512/*256*/);
                    byte[] hash_code = g.H(password);
                    string hashpassword = BitConverter.ToString(hash_code);

                    //Запись учет.данных администратора/пользователя в таблицу БД
                    if (!IsChangePassword && (int)o == 0)
                    {
                        if (SQLCon.ExecuteScalar($"INSERT INTO tblAccounts (UserName, Password, IsAdmin) VALUES ('{tbxName.Text}', '{hashpassword}', '{AsAdmin}')"))
                        {
                            MessageBox.Show("Регистрация успешно завершена.");
                            Success = true;
                            Close();
                        }
                    }
                    //Изменение хэш-кода пароля в БД на новый
                    else if(IsChangePassword)
                    {                        
                        if (hashpassword == User.Password)
                            MessageBox.Show("Введенный пароль совпадает с текущим. Необходимо ввести новый пароль");
                        else
                        {
                            SQLCon.ExecuteScalar(string.Format("UPDATE tblAccounts SET Password='{0}' WHERE [UserName]='{1}'", hashpassword, User.Name));
                            User.Password = hashpassword;
                            MessageBox.Show("Пароль изменен.");
                            Success = true;
                            MainWindow mainWindow = new MainWindow();
                            Close();
                            mainWindow.Show();
                        }
                    }                    
                }
            }           
        }
    }
}

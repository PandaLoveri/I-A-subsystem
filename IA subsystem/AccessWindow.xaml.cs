using System.Windows;

namespace IA_subsystem
{
    /// <summary>
    /// Окно, появляющееся в случае если доступ к КС разрешен
    /// </summary>
    public partial class AccessWindow : Window
    {
        public AccessWindow()
        {
            InitializeComponent();

            //Присваиваем логин пользователя для приветствия
            tbUserName.Text =User.Name;

            //Если вход произведен под администратором, то, кроме кнопок "Изменение пароля" и "Выход",
            // д.б. кнопка "Управление учетными записями"           
            btnManage.Visibility = (!User.IsAdmin) ? Visibility.Collapsed : Visibility.Visible;
            btnChangePassword.Focus();            
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

        //Обработчик события нажатия на кнопку "Изменение пароля"
        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            Registration startWindow = new Registration(User.IsAdmin, true);
            startWindow.ShowDialog();
            if (startWindow.Success)
             Close();
        }        

        //Обработчик события нажатия на кнопку окна "Управление учетными записями"
        private void BtnManage_Click(object sender, RoutedEventArgs e)
        {
            AccountManagement accountManagement = new AccountManagement();
            accountManagement.ShowDialog();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

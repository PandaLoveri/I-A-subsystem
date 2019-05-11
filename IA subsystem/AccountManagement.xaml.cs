using System.Windows;
using System.Data;
using System.Collections.ObjectModel;

namespace IA_subsystem
{
    /// <summary>
    /// Класс для управления учетными записями пользователей
    /// </summary>
    public partial class AccountManagement : Window
    {
        private ObservableCollection<Account> lstaccounts;

        public AccountManagement()
        {
            InitializeComponent();
            lstaccounts = new ObservableCollection<Account>();
            AddAccounts();           
        }
               
        //Метод для добавления всех найденных пользователей из БД в DataGrid
        private void AddAccounts()
        {
            dgUsersAccounts.ItemsSource = lstaccounts;
            //Получение значений из таблицы БД
            foreach (DataRow row in SQLCon.GetDateTable("SELECT UserName, IsAdmin FROM tblAccounts").Rows)
            {
                if (!(bool)row["IsAdmin"])
                {
                    lstaccounts.Add(new Account(row["UserName"] as string, "Пользователь"));
                }
                else
                {
                    lstaccounts.Add(new Account(row["UserName"] as string, "Администратор"));
                }
            }            
        }

        //Обработчик события нажатия на кнопку "Добавить учетную запись"
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Registration startWindow = new Registration(false, false);
            startWindow.ShowDialog();
            lstaccounts.Clear();
            AddAccounts();
            dgUsersAccounts.Items.Refresh();
        }

        //Обработчик события нажатия на кнопку "Удалить учетную запись"
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            object o;
            //Проверка не выполнен ли выбор пользователь для удаления
            if (dgUsersAccounts.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пользователя для удаления");
            }
            //Если пользователь выбран
            else if (SQLCon.ExecuteScalar("SELECT COUNT(*) FROM tblAccounts WHERE IsAdmin='True'", out o))
            {
                //Если выбран-пользователь администратор для удаления и он один
                if (lstaccounts[dgUsersAccounts.SelectedIndex].Type == "Администратор" && (int)o == 1)
                {
                    MessageBox.Show("Вы не можете удалить эту учетную запись, так как это единственная учетная запись администратора.");
                }
                //Если выбран пользователь или существует несколько администраторов
                else if (MessageBox.Show("Вы действительно хотите удалить эту учетную запись?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //Удаление записи о пользователе в БД
                    string userName = lstaccounts[dgUsersAccounts.SelectedIndex].Name;
                    SQLCon.ExecuteScalar($"DELETE FROM tblAccounts WHERE UserName='{userName}'");
                    //Удаление из списка lstaccounts пользователя
                    lstaccounts.RemoveAt(dgUsersAccounts.SelectedIndex);
                    dgUsersAccounts.Items.Refresh();
                }
            }
        }

        //Обработчик события нажатия на кнопку "Назад"
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

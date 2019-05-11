using System;
using System.IO;
using System.Windows;
using System.Data;
using System.Data.SqlServerCe;

namespace IA_subsystem
{
    class SQLCon
    {
        public static SqlCeConnection sqlCon = new SqlCeConnection("Data Source = dbUserAccount.sdf; Password=dbPass; Encrypt = TRUE;");

        //Метод, показывающий есть ли подключение к БД или нет
        public static bool ConnectToDatabase()
        {
            string connStr = "Data Source = dbUserAccount.sdf; Password=dbPass; Encrypt = TRUE;";

            //если БД не существует, создаем таблицу для логинов и паролей
            if (!File.Exists("dbUserAccount.sdf")) 
            {
                try
                {
                    SqlCeEngine engine = new SqlCeEngine(connStr);
                    engine.CreateDatabase();

                    object o;
                    // Создаем таблицу аккаунтов со столбцами "имя пользователя", "пароль", "яв-ся админом"
                    ExecuteScalar("CREATE TABLE tblAccounts (UserName nvarchar(150) NOT NULL PRIMARY KEY, Password nvarchar(4000), IsAdmin bit)", out o, true, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создания базы данных: " + ex.Message);
                    File.Delete("dbUserAccount.sdf");
                    return false;
                }
            }
            return true;
        }

        // Метод, к-ый выполняет sql-выражение и возвращает одно скалярное значение
        public static bool ExecuteScalar(string query, out object o, bool ignore = false, bool throwEx = false)
        {
            o = new object();
            try
            {
               sqlCon.Open();
                using (SqlCeCommand com = new SqlCeCommand(query, sqlCon))
                {
                    o = com.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                if (!ignore)
                    MessageBox.Show(ex.Message, "Ошибка");
                if (throwEx)
                    throw ex;
                return false;
            }
            finally
            {
                sqlCon.Close();
            }
            return true;
        }

        public static bool ExecuteScalar(string query, bool ignore = false, bool throwEx = false)
        {
            object o = new object();
            try
            {
                sqlCon.Open();
                using (SqlCeCommand com = new SqlCeCommand(query, sqlCon))
                {
                    o = com.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                if (!ignore)
                    MessageBox.Show(ex.Message, "Ошибка");
                if (throwEx)
                    throw ex;
                return false;
            }
            finally
            {
                sqlCon.Close();
            }
            return true;
        }

        //Метод получения таблицы из БД
        public static DataTable GetDateTable(string query, bool ignore = false)
        {
            DataTable dt = new DataTable();
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();

                // SqlCeDataAdapter Представляет набор команд данных и подключения к БД, которые используются для заполнения DataSet и обновления источника данных.
                using (SqlCeDataAdapter apr = new SqlCeDataAdapter(query, sqlCon)) //
                {
                    apr.Fill(dt);//Добавляет или обновляет строки в указанном диапазоне в DataSet для соответствия строкам в источнике данных с помощью DataTable имя
                }
            }
            catch (Exception ex)
            {
                if (!ignore)
                    MessageBox.Show(ex.Message, "Ошибка");
                dt = null;
            }
            finally
            {
                sqlCon.Close();
            }
            return dt;
        }
    }
}

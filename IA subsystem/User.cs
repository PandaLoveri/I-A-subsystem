

namespace IA_subsystem
{
    public static class User
    {
        public static string Name { get; set; }
        public static string Password { get; set; }
        public static bool IsAdmin { get; set; }

        //Метод для проверки коррректности введенной строки логина
        public static bool IsCorrectName(string userName, out string error)
        {
            if (string.IsNullOrEmpty(userName))
            {
                error = "Введите логин!";
                return false;
            }            
            foreach (char symb in userName)
                if (!(char.IsLetter(symb) || ('0'<=symb) && (symb<='9')))
                {
                    error = "Логин может содержать только цифры и буквы";
                    return false;
                }
            error = "";
            return true;
        }
    }
}

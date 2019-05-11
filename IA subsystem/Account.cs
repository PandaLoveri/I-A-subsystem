//Класс аккаунта, содержащий логин и тип каждого пользователя КС
namespace IA_subsystem
{
    public class Account
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Account(string name,string type)
        {
            Name = name;
            Type = type;
        }
    }
}

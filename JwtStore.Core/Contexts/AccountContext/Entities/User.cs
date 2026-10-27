using JwtStore.Core.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.SharedContext.Entities;

namespace JwtStore.Core.Contexts.AccountContext.Entities
{
    public class User : Entity
    {
        protected User() { }
        public User(string address, string? password = null)
        {
            Email = address;
            Password = new Password(password);
        }
        public string Name { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public Password Password { get; private set; } = null!;
        public string Image { get; private set; } = string.Empty;

        public void UpdatePassword(string plainTextPassword, string code)
        {
            if (!string.Equals(code.Trim(), Password.ResetCode.Trim(), StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("Código inválido");

            Password = new Password(plainTextPassword);
        }
        public void UpdateEmail(string address) => Email = address;        
        public void ChangePassword(string plainTextPassword) => Password = new Password(plainTextPassword);
    }
}

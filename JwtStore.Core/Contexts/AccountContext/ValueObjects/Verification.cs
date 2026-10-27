using JwtStore.Core.Contexts.SharedContext.ValueObjects;

namespace JwtStore.Core.Contexts.AccountContext.ValueObjects
{
    public class Verification : ValueObject
    {
        public Verification() { }
        public string Code { get; } = Guid.NewGuid().ToString("N")[0..6].ToUpper();
        public DateTime? ExpiresAt { get; private set; } = DateTime.UtcNow.AddMinutes(5);
        public DateTime? VerifiedAt { get; private set; } = null;
        public bool IsActive => VerifiedAt != null && ExpiresAt == null;

        public void Verify(string code)
        {
            if (IsActive)
                throw new Exception("Código já verificado");

            if (ExpiresAt < DateTime.UtcNow)
                throw new Exception("Código expirado");

            if (!string.Equals(code.Trim(), Code.Trim(), StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("Código inválido");

            ExpiresAt = null;
            VerifiedAt = DateTime.UtcNow;
        }
    }
}

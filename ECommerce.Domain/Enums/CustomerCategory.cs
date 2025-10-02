using System.ComponentModel;

namespace ECommerce.Domain.Enums
{
    public enum CustomerCategory : byte
    {
        [Description("REGULAR")]
        REGULAR,
        [Description("PREMIUM")]
        PREMIUM,
        [Description("VIP")]
        VIP
    }
}

using System.ComponentModel.DataAnnotations;

namespace doanweb.Models
{
    /// <summary>
    /// Custom validation attribute ?? ki?m tra checkbox ph?i ???c ch?n (true)
    /// </summary>
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public MustBeTrueAttribute()
        {
            ErrorMessage = "B?n ph?i ??ng ý v?i ?i?u kho?n d?ch v? ?? ??ng ký";
        }

        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }
    }
}

using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PSSC_Proiect.Models
{
    public class InputOrder
    {
        [Required]
        [RegularExpression(OrderRegistrationCode.Pattern)]
        public string RegistrationCode { get; set; }

        [Required]
        [RegularExpression(OrderDescription.Pattern)]
        public string Description { get; set; }

        [Required]
        [Range(1, 300)]
        public float Amount { get; set; }
        [Required]
        [RegularExpression(OrderAddress.Pattern)]
        public string Address { get; set; }

        [Required]
        [Range(1, 999999)]
        public float Price { get; set; }
        [Range(1, 999999)]
        public float FinalPrice { get; set; }
    }
}

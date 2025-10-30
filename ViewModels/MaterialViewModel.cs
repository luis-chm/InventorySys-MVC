using System.ComponentModel.DataAnnotations;

namespace InventorySys.ViewModels
{
    public class MaterialViewModel
    {
        public int MaterialId { get; set; }

        [Required]
        [Display(Name = "Código de Material")]
        public string MaterialCode { get; set; } = null!;

        [Required]
        [Display(Name = "Descripción")]
        public string MaterialDescription { get; set; } = null!;

        [Display(Name = "Colección")]
        public int? CollectionId { get; set; }

        [Display(Name = "Acabado")]
        public int? FinitureId { get; set; }

        [Display(Name = "Formato")]
        public int? FormatId { get; set; }

        [Display(Name = "Sitio")]
        public int? SiteId { get; set; }

        [Display(Name = "Imagen")]
        public string? MaterialImg { get; set; }

        [Display(Name = "Subir Imagen")]
        public IFormFile? MaterialFile { get; set; }

        [Display(Name = "Fecha de Arribo")]
        [DataType(DataType.Date)]
        public DateOnly MaterialReceivedDate { get; set; }

        [Display(Name = "Stock")]
        [DataType(DataType.Currency)]
        public decimal MaterialStock { get; set; }

        [Display(Name = "Usuario")]
        public int? UserId { get; set; }

        public DateTime RecordInsertDateTime { get; set; }
        // Propiedades para mostrar nombres
        // Propiedades nullables para mostrar nombres
        public string? CollectionName { get; set; }
        public string? FinitureName { get; set; }
        public string? FormatName { get; set; }
        public string? SiteName { get; set; }
    }
}

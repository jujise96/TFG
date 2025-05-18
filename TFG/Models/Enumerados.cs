using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFG.Models
{
    using AspNetCoreGeneratedDocument;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TFG.Repositories;

    public enum TipoQuest
    {
        Principal,
        Secundaria,
        [Display(Name = "Sin marcar")]
        SinMarcar
    }

    public enum TipoItem
    {
        Arma,
        Armadura,
        Ayuda,
        Otro
    }

    public enum TipoEntidad
    {
        Juego,
        Mision,
        Item
    }

}


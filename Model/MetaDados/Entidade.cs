using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MetaDados
{
    public class Entidade
    {
        [Key]
        [Required]
        public int Id { get; set; }
    }
}

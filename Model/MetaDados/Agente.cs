using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MetaDados
{
    public class Agente : Entidade
    {
        public string Nome { get; set; }
        public string Status { get; set; }
        public virtual Fila? Fila { get; set; }
        //public virtual ObservacoesAgente? Observacoes { get; set; }
        //public Fila Fila = new();
        //public ObservacoesAgente Observacoes = new();

        public Agente()
        {
            Fila = new();
        }
    }
}

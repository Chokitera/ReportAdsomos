using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MetaDados
{
    public class ObservacoesAgente : Entidade
    {
        public DateTime Data { get; set; }
        public DateTime Duracao { get; set; } //Tempo em pausa
        public DateTime HoraInicial { get; set; }
        public DateTime HoraFinal { get; set; }
        public virtual Agente? Agente { get; set; }

        public ObservacoesAgente()
        {
            Agente = new();
        }
    }
}

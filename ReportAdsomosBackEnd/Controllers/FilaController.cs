using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReportAdsomosBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilaController : ControllerBase
    {
        /* 
          REQUISITO: COLETAR OS DADOS DA CENTRAL E TRANSFORMAR EM JSON, SOMENTE AS INFORMAÇÕES ABAIXO:
            - AGENTE
            - FILA
            - STATUS
            - TEMPO EM LIGAÇÃO
         
          DEPOIS COM ESSAS INFORMAÇÕES MONTAR UMA ROTA QUE DEVOLVE ESSES DADOS PARA O FRONTEND.
          NECESSÁRIO CRIAR TAMBÉM UMA OUTRA ROTA PARA RETORNAR OS VALORES EM FORMA DE RELATÓRIO, INICIALMENTE SEM PAGINAÇÃO.
          SALVAR OS DADOS DO RELATÓRIO NO BANCO, NO CASO O RELATÓRIO VAI SE BASEAR NOS DADOS DO BANCO.
         
         */
        
    }
}

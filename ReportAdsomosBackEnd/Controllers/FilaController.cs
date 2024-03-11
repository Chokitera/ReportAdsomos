using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace ReportAdsomosBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilaController : ControllerBase
    {
        private readonly string urlCentral;
        private readonly string urlRelatorio;
        private readonly string token;
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

        public FilaController()
        {
            urlCentral = "http://192.168.0.6/integrador/public/index";
            urlRelatorio = "";
            token = "";
        }

        [HttpGet("fila")]
        public string GetFila()
        {
            try
            {
                var Client = this.ServiceHTTPClient;
                //Consulta
                var consult = Client.GetAsync("").Result;

                if (consult == null)
                {
                    return "Não foi possível consultar a fila!";
                }
                return consult.Headers.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        ////public void GeraToken()
        ////{
        ////    try
        ////    {

        ////    }
        ////    catch (Exception)
        ////    {

        ////    }
        ////}

        public HttpClient ServiceHTTPClient
        {
            get
            {
                var objHTTPClient = new HttpClient();
                objHTTPClient.Timeout = new TimeSpan(0, 5, 0);
                objHTTPClient.BaseAddress = new Uri(urlCentral);

                return objHTTPClient;
            }

        }
    }
}

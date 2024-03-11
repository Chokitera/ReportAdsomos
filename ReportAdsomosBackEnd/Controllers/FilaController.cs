using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;

namespace ReportAdsomosBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilaController : ControllerBase
    {
        private readonly string urlCentral;
        private readonly string urlRelatorio;
        private string token;
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
            urlCentral = "/public/index";
            urlRelatorio = "/library/fila/realtime_ajax.php";
            token = "";
        }

        [HttpGet("fila")]
        public string GetFila()
        {
            try
            {
                //Gerar o Token
                var client = this.ServiceHTTPClient;
                using MultipartFormDataContent content = new()
                {
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "login" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "senha" }
                };

                //Consulta
                var consult = client.PostAsync(urlCentral, content).Result;

                if (consult.Headers.ToString().Contains("PHPSESSID"))
                {
                    int startPosition = consult.Headers.ToString().IndexOf("=") + 1; //Busca o PHPSESSID=
                    int endPosition = consult.Headers.ToString().LastIndexOf(";") - startPosition; //Busca o final do PHPSESSID
                    token = consult.Headers.ToString() + "\n" + consult.Headers.ToString().Substring(startPosition, endPosition); //Retorna o Token
                }
                else
                    token = "";

                //Relatorio
                var content2 = new StringContent("");
                var consult1 = client.PostAsync(urlRelatorio, content2).Result;

                token = consult.Content.Headers.ToString();
                if (token == string.Empty)
                    token = "Token não gerado!";
                return token;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //public void GeraToken()
        //{
        //    try
        //    {
        //        var client = this.ServiceHTTPClient;
        //        //using MultipartFormDataContent content = new()
        //        //{
        //        //    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "login" },
        //        //    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "senha" }
        //        //};
        //        var content = new StringContent("");
        //        //Consulta
        //        var consult = client.PostAsync("", content).Result;

        //        if (consult.Headers.ToString().Contains("PHPSESSID"))
        //        {
        //            int startPosition = consult.Headers.ToString().IndexOf("=") + 1; //Busca o PHPSESSID=
        //            int endPosition = consult.Headers.ToString().LastIndexOf(";") - startPosition; //Busca o final do PHPSESSID
        //            token = consult.Headers.ToString() + "\n" + consult.Headers.ToString().Substring(startPosition, endPosition); //Retorna o Token
        //        }
        //        else
        //            token = "";
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public void ValidaToken()
        //{

        //}

        public HttpClient ServiceHTTPClient
        {
            get
            {
                var SerHTTPClient = new HttpClient();
                SerHTTPClient.Timeout = new TimeSpan(0, 5, 0);
                SerHTTPClient.BaseAddress = new Uri("http://192.168.0.6/integrador");

                return SerHTTPClient;
            }

        }
    }
}

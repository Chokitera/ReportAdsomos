using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Net.Http.Headers;
using HtmlAgilityPack;
using Model.MetaDados;

namespace ReportAdsomosBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilaController : ControllerBase
    {
        private Agente agente;
        private readonly string urlGeral;
        private readonly string urlRelatorio;
        private readonly string urlFila;
        private string token;
        private string fila;
        private int sequencia;
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
            urlGeral = "http://192.168.0.6/integrador/public/index";
            urlRelatorio = "http://192.168.0.6/integrador/library/fila/answered.php";
            urlFila = "http://192.168.0.6/integrador/library/fila/realtime_ajax.php";
            token = "";
            fila = "";
            agente = new Agente();
        }

        [HttpGet("fila")]
        public string GetFila()
        {
            try
            {
                //Inicialização
                var client = this.ServiceHTTPClient;

                //Gerar o Token
                var consultaToken = client.GetAsync(urlGeral).Result;

                //Coletando o Token dos Cookies recebidos
                if (consultaToken.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        client.DefaultRequestHeaders.Add("Cookie", cookie);
                        token = cookie.ToString().Replace("PHPSESSID=", "").Replace(";", "").Replace("path=/", "").Replace(" ", "");
                    }
                }

                //Define o cookie
                var cookieHttp = new Cookie("PHPSESSID", token);

                //Novo HttpClient para Cookie (Handler)
                var handler = new HttpClientHandler();
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(client.BaseAddress, cookieHttp);

                //Cria um novo HttpClient com o HttpClientHandler personalizado (Cookie)
                var httpClientComCookies = new HttpClient(handler);
                httpClientComCookies.BaseAddress = new Uri(urlGeral);

                //Definição dos dados para Login
                using MultipartFormDataContent contentLogin = new()
                {
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "login" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "senha" }
                };

                //Login
                var login = httpClientComCookies.PostAsync("", contentLogin).Result;

                //Definição dos filtros para o relatório
                using MultipartFormDataContent filtroRelatorio = new()
                {
                    { new StringContent("13/03/2024", Encoding.UTF8, MediaTypeNames.Text.Plain), "data_ini" },
                    { new StringContent("00:00", Encoding.UTF8, MediaTypeNames.Text.Plain), "hora_inicio" },
                    { new StringContent("13/03/2024", Encoding.UTF8, MediaTypeNames.Text.Plain), "data_final" },
                    { new StringContent("23:59", Encoding.UTF8, MediaTypeNames.Text.Plain), "hora_fim" },
                    { new StringContent("'atitude'", Encoding.UTF8, MediaTypeNames.Text.Plain), "List_Queue[]" }, //Gestor
                    { new StringContent("'Agent/9234'", Encoding.UTF8, MediaTypeNames.Text.Plain), "List_Agent[]" },
                    { new StringContent("124", Encoding.UTF8, MediaTypeNames.Text.Plain), "totagentes" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "start" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "end" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "secondsstart" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "secondsend" }
                };

                //Filtrar relatório
                var relatorio = httpClientComCookies.PostAsync(urlRelatorio, filtroRelatorio).Result;

                //Fila com os agentes
                var filaAgentes = httpClientComCookies.GetAsync(urlFila).Result;

                //Coleta o retorno e define a fila
                fila = filaAgentes.Content.ReadAsStringAsync().Result;
                Console.WriteLine(fila); //Debug

                /* FAZER MÉTODO PARA PEGAR A FILA E VERIFICAR QUEM ESTÁ EM PAUSA (UTILIZAR MODELS), AO SAIR DA PAUSA ENVIAR AO BANCO A INFORMAÇÃO: 
                 * FILA
                 * AGENTE
                 * STATUS (PAUSA)
                 * DATA
                 * TEMPO (TEMPO DE PAUSA)
                 * HORARIO INICIAL
                 * HORARIO FINAL
                */

                //Trabalhando com o HTML recebido
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(fila);

                html.DocumentNode.SelectNodes("//tr").First().Remove(); //Remover código indevido (Cabeçalho Status)
                var valor = html.DocumentNode.SelectNodes("//tr").Last(); //Seleciona o último tr do html
                int resultado; //Validação numerica

                //Varre os dados presentes na fila
                sequencia = 1;
                foreach (HtmlNode item in valor.SelectNodes("//td"))
                {
                    if (!string.IsNullOrWhiteSpace(item.InnerHtml) && 
                        !item.InnerHtml.Trim().Contains("span") &&
                        !item.InnerHtml.Trim().Contains("Status") &&
                        !int.TryParse(item.InnerHtml.Trim(), out resultado))
                    {
                        string result = item.InnerHtml.Trim();
                        Console.WriteLine(result);

                        switch (sequencia)
                        {
                            case 1:
                                agente.Fila.Nome = result ?? "";
                                break;
                            case 2:
                                agente.Nome = result; //Nome
                                break;
                            case 3:
                                agente.Status = result; //Status

                                //Verificar antes se o agente está em pausa...
                                if (agente.Status == "Pausa")
                                {
                                    Console.WriteLine($"Agente em pausa: {agente.Nome}");

                                    //Gravar essa informação no banco
                                    agente.Observacoes.Data = DateTime.Today;
                                    agente.Observacoes.HoraInicial = DateTime.Now;
                                }
                                else
                                {
                                    //Verifica agente no banco
                                    Agente agenteAux = new();
                                    //agenteAux = //Trabalhar aqui com a consulta no banco... (Pesquisar aqui pelo nome do agente)

                                    if(agenteAux.Observacoes.Data == DateTime.Today && agenteAux.Observacoes.HoraFinal == null)
                                    {
                                        //Fazer aqui a inserção no banco da hora final e tempo
                                        agente.Observacoes.HoraFinal = DateTime.Now;
                                        //agente.Observacoes.Duracao = agenteAux.Observacoes.HoraInicial - DateTime.Now;
                                    }
                                }


                                sequencia = 0;
                                break;
                            default:
                                sequencia = 0;
                                break;
                        }

                        sequencia++;
                    } 
                }

                ////Varre os dados presentes na fila
                //foreach (HtmlNode div in html.DocumentNode.SelectNodes("//div")) //Div principal
                //{
                //    foreach(HtmlNode table in html.DocumentNode.SelectNodes("//table")) //Tabela principal
                //    {
                //        foreach(HtmlNode tr in html.DocumentNode.SelectNodes("//tr")) //Inicio da fila
                //        {
                //            foreach (HtmlNode td in html.DocumentNode.SelectNodes("//td")) //Dados da fila
                //            {
                //                string resultado = td.InnerHtml;
                //            }
                //        }
                //    }
                //}

                //TRANSFORMAR FILA EM JSON
                //Fazer...

                //Retorna o Token + a Fila (Utilizado como teste atualmente)
                if (token == string.Empty)
                    token = "Token não gerado!";
                return "Token: " + token + "\n\nFila: " + fila;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public HttpClient ServiceHTTPClient
        {
            get
            {
                //HttpClient
                var SerHTTPClient = new HttpClient();
                SerHTTPClient.Timeout = new TimeSpan(0, 5, 0);
                SerHTTPClient.BaseAddress = new Uri(urlGeral);

                return SerHTTPClient;
            }

        }
    }
}
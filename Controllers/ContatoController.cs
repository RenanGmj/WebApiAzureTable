using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using WebApiAzureTable.Models;

namespace WebApiAzureTable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContatoController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public ContatoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("SAConnectionString");
            _tableName = configuration.GetValue<string>("AzureTableName");
        }

        private TableClient GetTableClient()
        {
            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient(_tableName);

            tableClient.CreateIfNotExists();
            return tableClient;
        }

        [HttpPost]
        public IActionResult Criar(Contato contato)
        {
            var tableClient = GetTableClient();
            contato.RowKey = Guid.NewGuid().ToString();
            contato.PartitionKey = contato.RowKey;

            tableClient.UpsertEntity(contato);
            return Ok(contato);
            
        }


        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, Contato contato)
        {
            var tableClient = GetTableClient();
            var contatoTable = tableClient.GetEntity<Contato>(id, id).Value;

            contatoTable.Name = contato.Name;
            contatoTable.Email = contato.Email;
            contatoTable.Telefone = contato.Telefone;

            tableClient.UpsertEntity(contatoTable);
            return Ok();

        }


        [HttpGet("Listar")]
        public IActionResult ObterTodos()
        {
            var tableClient = GetTableClient();
            var contato = tableClient.Query<Contato>().ToList();
            return Ok(contato);


        }

        [HttpGet("ObterPorNome/{name}")]
        public IActionResult ObterPorNome(string name)
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contato>(x => x.Name == name).ToList();
            return Ok(contatos);
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Slapper;

namespace APIRegioes.Data
{
    public class RegioesRepository
    {
        private readonly IConfiguration _configuration;

        public RegioesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Regiao> Get(string codRegiao = null)
        {
            var conexao = new SqlConnection(
                _configuration.GetConnectionString("BaseDadosGeograficos"));

            bool queryWithParameter = !String.IsNullOrWhiteSpace(codRegiao);
            var sqlCmd =
                "SELECT R.IdRegiao, " +
                        "R.CodRegiao, " +
                        "R.NomeRegiao, " +
                        "E.SiglaEstado AS Estados_SiglaEstado, " +
                        "E.NomeEstado AS Estados_NomeEstado, " +
                        "E.NomeCapital AS Estados_NomeCapital " +
                "FROM dbo.Regioes R " +
                "INNER JOIN dbo.Estados E " +
                    "ON E.IdRegiao = R.IdRegiao " +
                (queryWithParameter ? $"WHERE (R.CodRegiao = @CodigoRegiao) " : String.Empty) +
                "ORDER BY R.NomeRegiao, E.NomeEstado";
            
            object paramQuery = null;
            if (queryWithParameter)
                paramQuery = new { CodigoRegiao = codRegiao };
            var dados = conexao.Query<dynamic>(sqlCmd, paramQuery);

            AutoMapper.Configuration.AddIdentifier(
                typeof(Regiao), "IdRegiao");
            AutoMapper.Configuration.AddIdentifier(
                typeof(Estado), "SiglaEstado");

            return (AutoMapper.MapDynamic<Regiao>(dados)
                as IEnumerable<Regiao>).ToArray();
        }        
    }
}
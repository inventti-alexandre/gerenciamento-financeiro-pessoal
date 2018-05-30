﻿using Competencias.Domain.Aggregates;
using Competencias.Domain.Exceptions;
using Competencias.Domain.Repositories;
using SharedKernel.Common.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Competencias.Domain.Services
{
	public class CompetenciaService : ICompetenciaService
	{
		private readonly ICompetenciaRepository _competenciaRepository;

		public CompetenciaService(ICompetenciaRepository competenciaRepository)
		{
			_competenciaRepository = competenciaRepository;
		}

		public async Task<Competencia> CriarAsync(Guid id, int ano, int mes)
		{
			var competenciaPorAnoeMes = await _competenciaRepository.ObterPorAnoEMesAsync(ano, mes);
			if (competenciaPorAnoeMes != null) throw new CompetenciaJaExistenteParaAnoEMesException(mes, ano);

			var competencia = new Competencia(id, DateTime.Now, new Ano(ano), (Mes)mes);

			await _competenciaRepository.AddAsync(competencia);

			return competencia;
		}

		public async Task<Lancamento> AdicionarReceitaAsync(Guid competenciaId, Guid id, int categoriaId, DateTime data, string descricao, bool isLancamentoPago,
															decimal valor, FormaDePagamento formaDePagto, string anotacao)
		{
			var competencia = await _competenciaRepository.GetByEntityIdAsync(competenciaId);
			if (competencia == null) throw new CompetenciaNaoEncontradaException();

			var receita = Receita.Create(Guid.NewGuid(), categoriaId, data, descricao, isLancamentoPago, valor, 
										formaDePagto, anotacao);

			competencia.AdicionarReceita(receita);

			return receita;
		}

		public async Task<Lancamento> AdicionarDespesaAsync(Guid competenciaId, Guid id, int categoriaId, DateTime data, string descricao, bool isLancamentoPago,
													decimal valor, FormaDePagamento formaDePagto, string anotacao)
		{
			var competencia = await _competenciaRepository.GetByEntityIdAsync(competenciaId);
			if (competencia == null) throw new CompetenciaNaoEncontradaException();

			var despesa = Despesa.Create(Guid.NewGuid(), categoriaId, data, descricao, isLancamentoPago, valor,
										formaDePagto, anotacao);

			competencia.AdicionarDespesa(despesa);

			return despesa;
		}
	}
}
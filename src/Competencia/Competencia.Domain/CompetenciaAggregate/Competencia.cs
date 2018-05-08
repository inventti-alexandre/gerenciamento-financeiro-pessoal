﻿using SharedKernel.Common;
using SharedKernel.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Competencia.Domain.CompetenciaAggregate
{
	public class CompetenciaAggregateRoot : Entity<Guid>
	{
		public Ano Ano { get; private set; }
		public Mes Mes { get; private set; }

		public Decimal TotalContasAPagar { get; private set; }
		public Decimal TotalContasAReceber { get; private set; }
		public Decimal Saldo { get; private set; }

		private List<Lancamento> _lancamentos = new List<Lancamento>();
		public IReadOnlyList<Lancamento> Lancamentos => _lancamentos.AsReadOnly();

		private DomainEvents _domainEvents;

		public CompetenciaAggregateRoot(DomainEvents domainEvents)
		{
			_domainEvents = domainEvents;
		}

		private void Register()
		{
			_domainEvents.Register<ReceitaAdicionada>(e =>
			{
				_lancamentos.Add(e.Receita);

				TotalContasAReceber += e.Receita;
				Saldo += e.Receita;

			});

			_domainEvents.Register<DespesaAdicionada>(e =>
			{
				_lancamentos.Add(e.Despesa);

				TotalContasAPagar += e.Despesa;
				Saldo += e.Despesa;

			});

			_domainEvents.Register<ReceitaAlterada>(e =>
			{
				var receitaAlterar = _lancamentos.SingleOrDefault(x => x.Id == e.Receita.Id) as Receita;
				if (receitaAlterar == null) return;

				TotalContasAReceber -= receitaAlterar;
				TotalContasAReceber += e.Receita;

				Saldo -= receitaAlterar;
				Saldo += e.Receita;

				receitaAlterar = e.Receita;

			});

			_domainEvents.Register<DespesaAlterada>(e =>
			{
				var despesaAlterar = _lancamentos.SingleOrDefault(x => x.Id == e.Despesa.Id) as Despesa;
				if (despesaAlterar == null) return;

				TotalContasAPagar -= despesaAlterar;
				TotalContasAPagar += e.Despesa;

				Saldo -= despesaAlterar;
				Saldo += e.Despesa;

				despesaAlterar = e.Despesa;

			});

			_domainEvents.Register<ReceitaRemovida>(e =>
			{
				var receitaRemover = _lancamentos.SingleOrDefault(x => x.Id == e.Receita.Id) as Receita;
				if (receitaRemover == null) return;

				TotalContasAReceber -= receitaRemover;
				Saldo -= receitaRemover;

				_lancamentos.Remove(receitaRemover);

			});

			_domainEvents.Register<DespesaRemovida>(e =>
			{
				var despesaRemover = _lancamentos.SingleOrDefault(x => x.Id == e.Despesa.Id) as Despesa;
				if (despesaRemover == null) return;

				TotalContasAPagar -= despesaRemover;
				Saldo -= despesaRemover;

				_lancamentos.Remove(despesaRemover);

			});
		}

		public CompetenciaAggregateRoot(DomainEvents domainEvents, Guid id, Ano ano, Mes mes) : base(id)
		{
			_domainEvents = domainEvents;

			Register();

			Ano = ano;
			Mes = mes;

			_domainEvents.Raise(new CompetenciaCriada(this));

		}

		public void AdicionarReceita(Receita receita)
		{
			var existeReceita = _lancamentos.OfType<Receita>().Any(x => x.Id == receita.Id);
			if (existeReceita) throw new Exception("Receita já adicionada!");

			_domainEvents.Raise(new ReceitaAdicionada(receita));
		}

		public void AdicionarDespesa(Despesa despesa)
		{
			var existeDespesa = _lancamentos.OfType<Despesa>().Any(x => x.Id == despesa.Id);
			if (existeDespesa) throw new Exception("Despesa já adicionada!");

			_domainEvents.Raise(new DespesaAdicionada(despesa));
		}

		public void AlterarReceita(Receita receita)
		{
			_domainEvents.Raise(new ReceitaAlterada(receita));
		}

		public void AlterarDespesa(Despesa despesa)
		{
			_domainEvents.Raise(new DespesaAlterada(despesa));
		}

		public void RemoverReceita(Receita receita)
		{
			_domainEvents.Raise(new ReceitaRemovida(receita));
		}

		public void RemoverDespesa(Despesa despesa)
		{
			_domainEvents.Raise(new DespesaRemovida(despesa));
		}

	}
}
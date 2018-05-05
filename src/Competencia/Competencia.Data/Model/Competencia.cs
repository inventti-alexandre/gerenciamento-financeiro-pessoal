﻿using System;
using System.Collections.Generic;

namespace Competencia.Data.Model
{
	public class Competencia
    {
		public int Id { get; set; }
		public DateTime DataCriacao { get; set; }
		public int Mes { get; set; }
		public int Ano { get; set; }

		public HashSet<Lancamento> Lancamentos { get; set; } = new HashSet<Lancamento>();

	}
}

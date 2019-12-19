﻿using AnalysisServicesRefresh.BLL.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface ITokenProvider
    {
        Task<Token> CreateAsync(CancellationToken cancellationToken = default);
    }
}
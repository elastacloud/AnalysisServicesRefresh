using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Interfaces;
using System;

namespace AnalysisServicesRefresh.BLL.Factories
{
    public class ModelProcessorFactory
    {
        public IModelProcessor Create(ModelProcessorType modelProcessorType)
        {
            switch (modelProcessorType)
            {
                case ModelProcessorType.NonTransactional:
                    return new NonTransactionalModelProcessor();
                case ModelProcessorType.Transactional:
                    return new TransactionalModelProcessor();
                default:
                    throw new InvalidOperationException("ModelProcessorType is unknown.");
            }
        }
    }
}

using System;

namespace AnalysisServicesRefresh.BLL.ModelProcessors
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
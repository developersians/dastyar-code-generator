namespace Dastyar.CodeGenerator;

public enum TemplateKind
{
    GetAggregateByIdQuery = 1,
    GetAggregateByIdQueryHandler,

    GetAggregateListQuery,
    GetAggregateListQueryHandler,

    CreateAggregateCommand,
    CreateAggregateCommandHandler,

    UpdateAggregateCommand,
    UpdateAggregateCommandHandler,

    DeleteAggregateCommand,
    DeleteAggregateCommandHandler,

    AggregateResponse,

    AggregateMapper,

    AggregateController,

    CreateAggregateRequest,

    UpdateAggregateRequest,

    IAggregateRepository,

    AggregateRepository,

    AggregateErrors,

    AggregateDomainMethods,
}

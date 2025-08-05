using Dastyar.CodeGenerator;

var kernelDomainDll = @"D:\Projects\dastyar\src\Dastyar.SharedKernel.Domain\bin\Debug\net9.0\Dastyar.SharedKernel.Domain.dll";
var domainDll = @"D:\Projects\dastyar\src\Dastyar.Domain\bin\Debug\net9.0\Dastyar.Domain.dll";

var engine = GeneratorEngine.CreateEngine(
    sharedDomainDllPath: kernelDomainDll,
    domainDllPath: domainDll,
    iAggregateRootNamespace: "Dastyar.SharedKernel.Domain.IAggregateRoot");

engine.GenerateTemplates();

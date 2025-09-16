using Dastyar.CodeGenerator;

var kernelDomainDll = @"D:\Projects\dastyar\src\Kernel\Ddd.Kernel.Domain\bin\Debug\net9.0\Ddd.Kernel.Domain.dll";
var domainDll = @"D:\Projects\dastyar\src\Dastyar.Domain\bin\Debug\net9.0\Dastyar.Domain.dll";

var engine = GeneratorEngine.CreateEngine(
    sharedDomainDllPath: kernelDomainDll,
    domainDllPath: domainDll,
    iAggregateRootNamespace: "Ddd.Kernel.Domain.IAggregateRoot");

engine.GenerateTemplates();

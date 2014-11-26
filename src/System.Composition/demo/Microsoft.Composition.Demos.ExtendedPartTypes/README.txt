Extended Part Type Demo
-----------------------

Shows how pre-existing instances, or factory methods,
can be registered with the container to take part in
composition.

Elements:

 * Extension/ContainerConfigurationExtensions.cs -
   adds extension methods WithExport and
   WithFactoryMethod to the ContainerConfiguration
   class.
 * Extension/InstanceExportDescriptorProvider.cs -
   provider that returns a pre-existing instance
   as an export value.
 * Extension/DelegateExportDescriptorProvider.cs -
   provider that supplies the result of calling
   a factory method as an export.

Points:

 * Shows mechanics only; because this is one-
   instance-per-provider, the time for the first
   resolve operation to complete will be linear
   with respect to the number of such providers.
   To support larger numbers of instances/factories
   some work is required to extend the providers.
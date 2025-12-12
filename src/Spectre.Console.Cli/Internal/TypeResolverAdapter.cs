using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal sealed class TypeResolverAdapter : ITypeResolver, IDisposable
{
    private readonly ITypeResolver? _resolver;
    private readonly ICommandMetadataContext _metadataContext;

    public TypeResolverAdapter(ITypeResolver? resolver, ICommandMetadataContext metadataContext)
    {
        _resolver = resolver;
        _metadataContext = metadataContext;
    }

    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            throw new CommandRuntimeException("Cannot resolve null type.");
        }

        try
        {
            var obj = _resolver?.Resolve(type);
            if (obj != null)
            {
                return obj;
            }

            // Fall back to use the metadata context for activation
            return _metadataContext.CreateInstance(type);
        }
        catch (CommandAppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CommandRuntimeException.CouldNotResolveType(type, ex);
        }
    }

    public void Dispose()
    {
        if (_resolver is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
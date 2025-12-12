# AOT Support Architecture

This document provides an overview of the Native AOT compilation changes in this branch.

## What Changed

This branch introduces a **metadata abstraction layer** that allows Spectre.Console.Cli to operate without runtime reflection. A Roslyn **source generator** produces compile-time metadata, enabling full Native AOT support while maintaining backward compatibility with reflection-based execution.

## The Core Pattern

All reflection operations are abstracted behind `ICommandMetadataContext`. The source generator produces a **thin wrapper** around `ReflectionMetadataContext` that adds `DynamicDependency` attributes to preserve types for the IL trimmer.

| Implementation | When Used | How It Works |
|---------------|-----------|--------------|
| `ReflectionMetadataContext` | Default (non-AOT) | Uses runtime reflection, same as before |
| Source-generated wrapper | AOT scenarios | Wraps reflection with `DynamicDependency` to preserve types |

**Why wrap reflection instead of replacing it?** Simplicity. The wrapper approach reuses existing reflection code while adding the trimmer hints needed for AOT. Most operations delegate to `ReflectionMetadataContext`, with a few exceptions where the generator emits direct instantiation code for AOT safety (see Generic Types below).

Existing applications continue to work unchanged. AOT support is opt-in.

## Key Components

### Public Interfaces (`Metadata/`)
- `ICommandMetadataContext` - Core abstraction for all metadata operations
- `ISettingsMetadata` - Describes a settings type's properties and constructors
- `IPropertyAccessor` - AOT-safe property get/set with cached attributes
- `IConstructorMetadata` - Constructor invocation without reflection
- `ICommandTypeMetadata` - Provides metadata about a command type (description attributes)

**Generic types:** Types like `MultiMap<TKey, TValue>` and `FlagValue<T>` require special handling for AOT. The generator emits **direct instantiation code** (e.g., `new MultiMap<string, int>()`) rather than using `MakeGenericType()` at runtime. This ensures the AOT compiler generates native code for each specific generic instantiation.

### Reflection Implementations (`Internal/Metadata/`)
- `ReflectionMetadataContext` - Default implementation using reflection
- `ReflectionSettingsMetadata`, `ReflectionPropertyAccessor`, `ReflectionCommandTypeMetadata`, etc.
- `BuiltInCommandMetadata` - Hand-coded metadata for built-in commands

### Source Generator (`Spectre.Console.Cli.SourceGenerator/`)
- Scans for types inheriting `CommandSettings` and implementing `ICommand`
- Generates a partial class that wraps `ReflectionMetadataContext`
- Emits `DynamicDependency` attributes for discovered types:
  - Command and settings types
  - Custom TypeConverters, PairDeconstructors, and ValueProviders
- Emits **direct instantiation code** for generic types:
  - `CreateMultiMap()` - returns `new MultiMap<TKey, TValue>()` for each discovered key/value type pair
  - `CreateFlagValue()` - returns `new FlagValue<T>()` for each discovered flag type

### Type Conversion (`Internal/TypeConverterHelper.cs`)
- Provides intrinsic converters for common types (int, string, DateTime, etc.)
- Supports custom TypeConverters via `[TypeConverter]` attribute (preserved by DynamicDependency)
- Falls back to `TypeDescriptor.GetConverter()` when dynamic code is supported
- Controlled by `Spectre.Console.TypeConverterHelper.IsGetConverterSupported` feature switch

**Why intrinsic converters?** `TypeDescriptor.GetConverter()` uses reflection to discover type converters at runtime. The intrinsic converters provide AOT-safe conversion for common types, while custom converters work via the source generator's `DynamicDependency` attributes.

## How to Use AOT Support

```csharp
// 1. Create a partial class with the [SpectreMetadata] attribute
[SpectreMetadata]
public partial class AppMetadata { }

// 2. Pass it to CommandApp
var app = new CommandApp(new AppMetadata());
app.Configure(config => config.AddCommand<MyCommand>("cmd"));
return app.Run(args);
```

The source generator fills in the partial class with all discovered metadata.

## Trade-offs

### What You Gain
- **Native AOT support** - Applications can be published as self-contained, trimmed binaries
- **Smaller binaries** - Trimming works correctly because `DynamicDependency` tells the trimmer what to keep
- **Backward compatible** - Existing code works unchanged, reflection still used at runtime
- **Simple implementation** - Wrapper pattern keeps code maintainable

### What Changes
- **AOT requires opt-in** - Must create a `[SpectreMetadata]` partial class
- **Open generic commands not supported** - Generator can't discover closed generic instantiations
- **Build-time generation** - Source generator runs during compilation
- **DynamicDependency attributes** - Generated code includes attributes to preserve types for trimming

## Edge Cases for Release Notes

### Supported
- **Abstract settings base classes** - Inheritance hierarchies work correctly
- **Init-only properties** - `init` setters are supported
- **Custom TypeConverters** - Work via `[TypeConverter]` attribute on properties
- **Custom PairDeconstructors** - Supported for dictionary-style arguments
- **Validation attributes** - All validation attributes are preserved

### Limitations
- **Generic commands must be closed** - Open generic commands won't be discovered.
  ```csharp
  // Won't work - generator can't emit metadata for unknown T
  public class MyCommand<T> : Command<MySettings<T>> { }

  // Works - generator sees the concrete type and emits metadata for it
  public class StringCommand : Command<MySettings<string>> { }
  ```

  **Why?** The generator scans for type *declarations* that inherit from `CommandSettings`/`ICommand`. When it finds `class MyCommand<T>`, it can't emit property accessors because `T` is unknown.

  **Could this be supported?** Yes - the generator could scan *invocations* instead of declarations:
  ```csharp
  // Generator would find these call sites and extract the closed type
  config.AddCommand<MyCommand<string>>("cmd");
  ```
  This would require handling all config methods (AddCommand, AddBranch, SetDefaultCommand, etc.), fluent chains, and configuration spread across helper methods. Achievable but adds complexity to the generator.
- **Types must be accessible** - Settings and commands must be `public` or `internal`. Generated code references types directly with `typeof()`, so they must be visible to the generated metadata class.
- **Partial class required** - The `[SpectreMetadata]` target must be a partial class so the generator can augment it with the implementation.

### Built-in Commands
Built-in commands (Version, Explain, XmlDoc, OpenCliGenerator) are preserved by `BuiltInCommandMetadata` within the library itself. Since these types are internal to the library and directly referenced in code, they are automatically preserved by the trimmer without requiring `DynamicDependency` attributes from user code.

## Testing the Changes

### Source Generator Tests
The `Spectre.Console.Cli.SourceGenerator.Tests` project contains 20+ commands covering diverse scenarios:
- Async execution, deep inheritance, edge cases
- Custom converters, enum handling, validation
- Dependency injection, flag values, arrays, subcommands

```bash
dotnet test src/Spectre.Console.Cli.SourceGenerator.Tests
```

Each test verifies that the source generator produces correct metadata and that commands execute properly using the AOT-safe path.

### Metadata Comparison Tests
`Spectre.Console.Cli.Tests/Metadata/MetadataComparisonTests.cs` verifies that source-generated metadata produces identical results to reflection-based metadata across all test settings types.

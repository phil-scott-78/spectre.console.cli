namespace Spectre.Console.Cli;

/// <summary>
/// Helper class for type conversion operations with AOT-safe intrinsic converters.
/// </summary>
public static class TypeConverterHelper
{
    internal const DynamicallyAccessedMemberTypes ConverterAnnotation = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields;

    internal static bool IsGetConverterSupported =>
        !AppContext.TryGetSwitch("Spectre.Console.TypeConverterHelper.IsGetConverterSupported", out var enabled) || enabled;

    /// <summary>
    /// Retrieves the appropriate <see cref="System.ComponentModel.TypeConverter"/> for the specified type.
    /// </summary>
    /// <param name="type">
    /// The type for which a <see cref="System.ComponentModel.TypeConverter"/> is to be retrieved.
    /// The type must have either an intrinsic converter, a <see cref="System.ComponentModel.TypeConverterAttribute"/>,
    /// or support dynamic code for creation of a converter.
    /// </param>
    /// <returns>
    /// A <see cref="System.ComponentModel.TypeConverter"/> instance that is capable of handling conversions
    /// for the specified type.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when no suitable <see cref="System.ComponentModel.TypeConverter"/> can be found for the specified type.
    /// </exception>
    public static TypeConverter GetTypeConverter([DynamicallyAccessedMembers(ConverterAnnotation)] Type type)
    {
        var converter = GetConverter(type);
        if (converter != null)
        {
            return converter;
        }

        throw new InvalidOperationException($"Could not find type converter for '{type.FullName}'");

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2087",
            Justification = "Types are preserved via [DynamicDependency] in the generated metadata context.")]
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026",
            Justification = "Types are preserved via [DynamicDependency] in the generated metadata context.")]
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067",
            Justification = "Types are preserved via [DynamicDependency] in the generated metadata context.")]
        static TypeConverter? GetConverter([DynamicallyAccessedMembers(ConverterAnnotation)] Type type)
        {
            // Try intrinsic converters first (AOT-safe)
            var intrinsicConverter = GetIntrinsicConverter(type);
            if (intrinsicConverter != null)
            {
                return intrinsicConverter;
            }

            // Check for TypeConverterAttribute
            // Note: This works in AOT when the converter type is preserved via DynamicDependency
            // (the source generator emits DynamicDependency for all discovered TypeConverterAttribute types)
            var attribute = type.GetCustomAttribute<TypeConverterAttribute>();
            if (attribute != null)
            {
                var converterType = Type.GetType(attribute.ConverterTypeName, false, false);
                if (converterType != null)
                {
                    var converter = Activator.CreateInstance(converterType) as TypeConverter;
                    if (converter != null)
                    {
                        return converter;
                    }
                }
            }

            // Fallback to TypeDescriptor when feature switch allows it
            if (IsGetConverterSupported)
            {
                return TypeDescriptor.GetConverter(type);
            }

            return null;
        }
    }

    /// <summary>
    /// Wrapper for type converter factories to avoid multicast delegate thunk annotation issues.
    /// </summary>
    private readonly struct ConverterFactory
    {
        private readonly Func<TypeConverter>? _simpleFactory;
        private readonly bool _isEnumFactory;

        private ConverterFactory(Func<TypeConverter> factory)
        {
            _simpleFactory = factory;
            _isEnumFactory = false;
        }

        private ConverterFactory(bool isEnumFactory)
        {
            _simpleFactory = null;
            _isEnumFactory = isEnumFactory;
        }

        public static ConverterFactory Simple(Func<TypeConverter> factory) => new(factory);
        public static ConverterFactory ForEnum() => new(isEnumFactory: true);

        public TypeConverter Create([DynamicallyAccessedMembers(ConverterAnnotation)] Type type)
        {
            if (_isEnumFactory)
            {
                return new EnumConverter(type);
            }

            return _simpleFactory!();
        }
    }

    private static readonly Dictionary<Type, ConverterFactory> _intrinsicConverters;

    static TypeConverterHelper()
    {
        _intrinsicConverters = new Dictionary<Type, ConverterFactory>
        {
            [typeof(bool)] = ConverterFactory.Simple(() => new BooleanConverter()),
            [typeof(byte)] = ConverterFactory.Simple(() => new ByteConverter()),
            [typeof(sbyte)] = ConverterFactory.Simple(() => new SByteConverter()),
            [typeof(char)] = ConverterFactory.Simple(() => new CharConverter()),
            [typeof(double)] = ConverterFactory.Simple(() => new DoubleConverter()),
            [typeof(string)] = ConverterFactory.Simple(() => new StringConverter()),
            [typeof(int)] = ConverterFactory.Simple(() => new Int32Converter()),
            [typeof(short)] = ConverterFactory.Simple(() => new Int16Converter()),
            [typeof(long)] = ConverterFactory.Simple(() => new Int64Converter()),
            [typeof(float)] = ConverterFactory.Simple(() => new SingleConverter()),
            [typeof(ushort)] = ConverterFactory.Simple(() => new UInt16Converter()),
            [typeof(uint)] = ConverterFactory.Simple(() => new UInt32Converter()),
            [typeof(ulong)] = ConverterFactory.Simple(() => new UInt64Converter()),
            [typeof(object)] = ConverterFactory.Simple(() => new TypeConverter()),
            [typeof(CultureInfo)] = ConverterFactory.Simple(() => new CultureInfoConverter()),
            [typeof(DateTime)] = ConverterFactory.Simple(() => new DateTimeConverter()),
            [typeof(DateTimeOffset)] = ConverterFactory.Simple(() => new DateTimeOffsetConverter()),
            [typeof(decimal)] = ConverterFactory.Simple(() => new DecimalConverter()),
            [typeof(TimeSpan)] = ConverterFactory.Simple(() => new TimeSpanConverter()),
            [typeof(Guid)] = ConverterFactory.Simple(() => new GuidConverter()),
            [typeof(Uri)] = ConverterFactory.Simple(() => new UriTypeConverter()),
            [typeof(Array)] = ConverterFactory.Simple(() => new ArrayConverter()),
            [typeof(ICollection)] = ConverterFactory.Simple(() => new CollectionConverter()),
            [typeof(Enum)] = ConverterFactory.ForEnum(),
            [typeof(FileInfo)] = ConverterFactory.Simple(() => new FileInfoTypeConverter()),
            [typeof(DirectoryInfo)] = ConverterFactory.Simple(() => new DirectoryInfoTypeConverter()),
#if !NETSTANDARD2_0
            [typeof(Int128)] = ConverterFactory.Simple(() => new Int128Converter()),
            [typeof(Half)] = ConverterFactory.Simple(() => new HalfConverter()),
            [typeof(UInt128)] = ConverterFactory.Simple(() => new UInt128Converter()),
            [typeof(DateOnly)] = ConverterFactory.Simple(() => new DateOnlyConverter()),
            [typeof(TimeOnly)] = ConverterFactory.Simple(() => new TimeOnlyConverter()),
            [typeof(Version)] = ConverterFactory.Simple(() => new VersionConverter()),
#endif
        };
    }

    /// <summary>
    /// A highly-constrained version of <see cref="TypeDescriptor.GetConverter(Type)" /> that only returns intrinsic converters.
    /// </summary>
    private static TypeConverter? GetIntrinsicConverter([DynamicallyAccessedMembers(ConverterAnnotation)] Type type)
    {
        var originalType = type;
        var lookupType = type;

        // Handle Nullable<T> - unwrap to underlying type for converter lookup
        var underlyingType = Nullable.GetUnderlyingType(lookupType);
        if (underlyingType != null)
        {
            lookupType = underlyingType;
            originalType = underlyingType;
        }

        if (lookupType.IsArray)
        {
            lookupType = typeof(Array);
        }

        if (typeof(ICollection).IsAssignableFrom(lookupType))
        {
            lookupType = typeof(ICollection);
        }

        if (lookupType.IsEnum)
        {
            lookupType = typeof(Enum);
        }

        if (_intrinsicConverters.TryGetValue(lookupType, out var factory))
        {
            // Pass the original type to the factory (important for EnumConverter)
            return factory.Create(originalType);
        }

        return null;
    }

    /// <summary>
    /// Converts strings to <see cref="FileInfo"/> instances.
    /// </summary>
    private sealed class FileInfoTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string path)
            {
                return new FileInfo(path);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Converts strings to <see cref="DirectoryInfo"/> instances.
    /// </summary>
    private sealed class DirectoryInfoTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string path)
            {
                return new DirectoryInfo(path);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
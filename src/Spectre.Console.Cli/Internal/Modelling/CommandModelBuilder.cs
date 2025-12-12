using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal static class CommandModelBuilder
{
    // Consider removing this in favor for value tuples at some point.
    private sealed class OrderedProperties
    {
        public int Level { get; }
        public int SortOrder { get; }
        public IPropertyAccessor[] Properties { get; }

        public OrderedProperties(int level, int sortOrder, IPropertyAccessor[] properties)
        {
            Level = level;
            SortOrder = sortOrder;
            Properties = properties;
        }
    }

    public static CommandModel Build(IConfiguration configuration, ICommandMetadataContext metadataContext)
    {
        var result = new List<CommandInfo>();
        foreach (var command in configuration.Commands)
        {
            result.Add(Build(null, command, metadataContext));
        }

        if (configuration.DefaultCommand != null)
        {
            // Add the examples from the configuration to the default command.
            configuration.DefaultCommand.Examples.AddRange(configuration.Examples);

            // Build the default command.
            var defaultCommand = Build(null, configuration.DefaultCommand, metadataContext);

            result.Add(defaultCommand);
        }

        // Create the command model and validate it.
        var model = new CommandModel(configuration.Settings, result, configuration.Examples);
        CommandModelValidator.Validate(model, configuration.Settings);

        return model;
    }

    private static CommandInfo Build(CommandInfo? parent, ConfiguredCommand command, ICommandMetadataContext metadataContext)
    {
        var info = new CommandInfo(parent, command, metadataContext);

        foreach (var parameter in GetParameters(info, metadataContext))
        {
            info.Parameters.Add(parameter);
        }

        foreach (var childCommand in command.Children)
        {
            var child = Build(info, childCommand, metadataContext);
            info.Children.Add(child);
        }

        // Normalize argument positions.
        var index = 0;
        foreach (var argument in info.Parameters.OfType<CommandArgument>()
            .OrderBy(argument => argument.Position))
        {
            argument.Position = index;
            index++;
        }

        return info;
    }

    private static IEnumerable<CommandParameter> GetParameters(CommandInfo command, ICommandMetadataContext metadataContext)
    {
        var result = new List<CommandParameter>();
        var argumentPosition = 0;

        // We need to get parameters in order of the class where they were defined.
        // We assign each inheritance level a value that is used to properly sort the
        // arguments when iterating over them.
        IEnumerable<OrderedProperties> GetPropertiesInOrder()
        {
            var current = command.SettingsType;
            var level = 0;
            var sortOrder = 0;
            while (current.BaseType != null)
            {
                var metadata = metadataContext.GetSettingsMetadata(current);
                var declaredProperties = metadata.Properties
                    .Where(p => p.DeclaringType == current)
                    .ToArray();
                yield return new OrderedProperties(level, sortOrder, declaredProperties);
                current = current.BaseType;

                // Things get a little bit complicated now.
                // Only consider a setting's base type part of the
                // setting, if there isn't a parent command that implements
                // the setting's base type. This might come back to bite us :)
                var currentCommand = command.Parent;
                while (currentCommand != null)
                {
                    if (currentCommand.SettingsType == current)
                    {
                        level--;
                        break;
                    }

                    currentCommand = currentCommand.Parent;
                }

                sortOrder--;
            }
        }

        var groups = GetPropertiesInOrder();
        foreach (var group in groups.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
        {
            var parameters = new List<CommandParameter>();

            foreach (var accessor in group.Properties)
            {
                if (accessor.OptionAttribute != null)
                {
                    var option = BuildOptionParameter(accessor);

                    // Any previous command has this option defined?
                    if (command.HaveParentWithOption(option))
                    {
                        // Do we allow it to exist on this command as well?
                        if (command.AllowParentOption(option))
                        {
                            option.IsShadowed = true;
                            parameters.Add(option);
                        }
                    }
                    else
                    {
                        // No parent have this option.
                        parameters.Add(option);
                    }
                }
                else if (accessor.ArgumentAttribute != null)
                {
                    var argument = BuildArgumentParameter(accessor);

                    // Any previous command has this argument defined?
                    // In that case, we should not assign the parameter to this command.
                    if (!command.HaveParentWithArgument(argument))
                    {
                        parameters.Add(argument);
                    }
                }
            }

            // Update the position for the parameters.
            foreach (var argument in parameters.OfType<CommandArgument>().OrderBy(x => x.Position))
            {
                argument.Position = argumentPosition++;
            }

            // Add all parameters to the result.
            foreach (var groupResult in parameters)
            {
                result.Add(groupResult);
            }
        }

        return result;
    }

    private static CommandOption BuildOptionParameter(IPropertyAccessor accessor)
    {
        var attribute = accessor.OptionAttribute!;
        var description = accessor.DescriptionAttribute;
        var converter = accessor.ConverterAttribute;
        var deconstructor = accessor.PairDeconstructorAttribute;
        var valueProvider = accessor.ValueProviderAttribute;
        var validators = accessor.ValidationAttributes;
        var defaultValue = accessor.DefaultValueAttribute;

        var kind = GetOptionKind(accessor.PropertyType, attribute, deconstructor, converter);

        if (defaultValue == null && accessor.PropertyType == typeof(bool))
        {
            defaultValue = new DefaultValueAttribute(false);
        }

        return new CommandOption(accessor.PropertyType, kind,
            accessor, description?.Description, converter, deconstructor,
            attribute, valueProvider, validators, defaultValue,
            attribute.ValueIsOptional);
    }

    private static CommandArgument BuildArgumentParameter(IPropertyAccessor accessor)
    {
        var attribute = accessor.ArgumentAttribute!;
        var description = accessor.DescriptionAttribute;
        var converter = accessor.ConverterAttribute;
        var defaultValue = accessor.DefaultValueAttribute;
        var valueProvider = accessor.ValueProviderAttribute;
        var validators = accessor.ValidationAttributes;

        var kind = GetParameterKind(accessor.PropertyType);

        return new CommandArgument(
            accessor.PropertyType, kind, accessor,
            description?.Description, converter,
            defaultValue, attribute, valueProvider,
            validators);
    }

    private static ParameterKind GetOptionKind(
        Type type,
        CommandOptionAttribute attribute,
        PairDeconstructorAttribute? deconstructor,
        TypeConverterAttribute? converter)
    {
        if (attribute.ValueIsOptional)
        {
            return ParameterKind.FlagWithValue;
        }

        if (type.IsPairDeconstructable() && (deconstructor != null || converter == null))
        {
            return ParameterKind.Pair;
        }

        return GetParameterKind(type);
    }

    private static ParameterKind GetParameterKind(Type type)
    {
        if (type == typeof(bool) || type == typeof(bool?))
        {
            return ParameterKind.Flag;
        }

        if (type.IsArray)
        {
            return ParameterKind.Vector;
        }

        return ParameterKind.Scalar;
    }
}
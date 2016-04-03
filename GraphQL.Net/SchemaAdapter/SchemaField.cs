using System.Collections.Generic;
using System.Linq;
using GraphQL.Parser.SchemaAST;
using Microsoft.FSharp.Core;

namespace GraphQL.Net.SchemaAdapter
{
    class SchemaField : ISchemaField<SchemaInfo>
    {
        private readonly SchemaType _declaringType;
        private readonly GraphQLField _field;

        public SchemaField(SchemaType declaringType, GraphQLField field)
        {
            _declaringType = declaringType;
            _field = field;
        }

        public ISchemaType<SchemaInfo> DeclaringType => _declaringType;
        public ISchemaType<SchemaInfo> FieldType => new SchemaType(_field.Type);
        public string FieldName => _field.Name;
        public FSharpOption<string> Description => FSharpOption<string>.None;
        public SchemaInfo Info => new SchemaFieldInfo(_field);

        private class SchemaArgumentValue : ISchemaArgumentValue<SchemaInfo>
        {
            public SchemaArgumentValue(ISchemaArgument<SchemaInfo> argument, Value<SchemaInfo> value)
            {
                Argument = argument;
                Value = value;
            }

            public ISchemaArgument<SchemaInfo> Argument { get; }
            public SchemaInfo Info { get; } = new SchemaInfo();
            public Value<SchemaInfo> Value { get; }
        }

        private class SchemaArgumentWithoutValidation : ISchemaArgument<SchemaInfo>
        {
            public SchemaArgumentWithoutValidation(string argumentName)
            {
                ArgumentName = argumentName;
            }

            public ValidationResult<ISchemaArgumentValue<SchemaInfo>> ValidateValue(Value<SchemaInfo> value)
                => ValidationResult<ISchemaArgumentValue<SchemaInfo>>.NewValid
                    (new SchemaArgumentValue(this, value));

            public string ArgumentName { get; }
            public FSharpOption<string> Description => FSharpOption<string>.None;
            public SchemaInfo Info { get; } = new SchemaInfo();
        }

        private IReadOnlyDictionary<string, ISchemaArgument<SchemaInfo>> _arguments;
        public IReadOnlyDictionary<string, ISchemaArgument<SchemaInfo>> Arguments
        {
            get
            {
                if (_arguments != null) return _arguments;
                _arguments = _field.InputArgumentNames
                    .Select(arg => new SchemaArgumentWithoutValidation(arg) as ISchemaArgument<SchemaInfo>)
                    .ToDictionary(a => a.ArgumentName);
                return _arguments;
            }
        }
    }
}
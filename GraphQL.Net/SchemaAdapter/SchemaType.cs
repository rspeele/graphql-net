using System.Collections.Generic;
using GraphQL.Parser.SchemaAST;
using Microsoft.FSharp.Core;

namespace GraphQL.Net.SchemaAdapter
{
    class SchemaType : ISchemaType<SchemaInfo>
    {
        private readonly GraphQLType _type;

        public SchemaType(GraphQLType type)
        {
            _type = type;
        }

        public string TypeName => _type.Name;
        public FSharpOption<string> Description => FSharpOption<string>.None;
        public SchemaInfo Info => new SchemaTypeInfo(_type);
        public IReadOnlyDictionary<string, ISchemaField<SchemaInfo>> Fields { get; }

        public IReadOnlyDictionary<string, ISchemaEnumValue<SchemaInfo>> EnumValues
            => new Dictionary<string, ISchemaEnumValue<SchemaInfo>>();
    }
}

namespace GraphQL.Net.SchemaAdapter
{
    class SchemaInfo
    {
        // nothing here, just a base class for more specific types
    }

    class SchemaTypeInfo : SchemaInfo
    {
        public SchemaTypeInfo(GraphQLType type)
        {
            Type = type;
        }

        public GraphQLType Type { get; }
    }

    class SchemaFieldInfo : SchemaInfo
    {
        public SchemaFieldInfo(GraphQLField field)
        {
            Field = field;
        }

        public GraphQLField Field { get; }
    }
}
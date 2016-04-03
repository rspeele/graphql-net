using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.Net
{
    // TODO: k, this whole thing is starting to suck. We don't need anything more
    // TODO: than the GraphQLField class. The inheritance isn't buying us anything.

    internal abstract class GraphQLField
    {
        public string Name { get; protected set; }
        public string Description { get; set; }
        public virtual bool IsList => false;
        public virtual bool IsPost => false;

        public abstract GraphQLType Type { get; }
        public abstract IEnumerable<string> InputArgumentNames { get; }
        public abstract LambdaExpression GetExpression(List<Input> inputs);
        public virtual object ResolvePostField() => null;
    }

    internal class GraphQLField<TContext, TArgs, TEntity, TField> : GraphQLField
    {
        protected readonly GraphQLSchema<TContext> Schema;
        protected readonly Func<TArgs, Expression<Func<TContext, TEntity, TField>>> ExprFunc;

        public GraphQLField(GraphQLSchema<TContext> schema, string name, Func<TArgs, Expression<Func<TContext, TEntity, TField>>> exprFunc)
        {
            Schema = schema;
            ExprFunc = exprFunc;
            Name = name;
        }

        public override IEnumerable<string> InputArgumentNames => TypeHelpers.GetArgNames<TArgs>();

        public override LambdaExpression GetExpression(List<Input> inputs)
        {
            var args = TypeHelpers.GetArgs<TArgs>(inputs);
            return ExprFunc(args);
        }

        // lazily initialize type, fields may be defined before all types are loaded
        private GraphQLType _type;
        public override GraphQLType Type => _type ?? (_type = Schema.GetGQLType(typeof(TField)));
    }

    internal class GraphQLListField<TContext, TArgs, TEntity, TField> : GraphQLField<TContext, TArgs, TEntity, List<TField>>
    {
        public GraphQLListField(GraphQLSchema<TContext> schema, string name, Func<TArgs, Expression<Func<TContext, TEntity, List<TField>>>> exprFunc)
            : base(schema, name, exprFunc) { }

        public override bool IsList => true;

        // this has to be copied since we're using the type TField and not List<TField> from the base class
        private GraphQLType _type;
        public override GraphQLType Type => _type ?? (_type = Schema.GetGQLType(typeof(TField)));
    }

    internal class GraphQLPostField<TContext, TField> : GraphQLField
    {
        private readonly GraphQLSchema<TContext> _schema;
        private readonly Func<TField> _fieldFunc;

        public GraphQLPostField(GraphQLSchema<TContext> schema, string name, Func<TField> fieldFunc)
        {
            _schema = schema;
            _fieldFunc = fieldFunc;

            Name = name;
        }

        public override object ResolvePostField() => _fieldFunc();
        public override bool IsPost => true;

        private GraphQLType _type;
        public override GraphQLType Type => _type ?? (_type = _schema.GetGQLType(typeof(TField)));
        public override IEnumerable<string> InputArgumentNames => Enumerable.Empty<string>();

        public override LambdaExpression GetExpression(List<Input> inputs)
        {
            throw new NotImplementedException();
        }
    }
}
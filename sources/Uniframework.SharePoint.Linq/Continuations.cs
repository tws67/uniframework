/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

/*
 * Version history:
 *
 * 0.2.3 - Continuation support experiments.
 */

/*
 * WARNING!
 * 
 * This file is part of a feasibility check for query projection continuation support.
 * There's no guarantuee this feature will make it to the final product.
 * 
 * This file shouldn't be included in any compilation whatsoever but acts as a scratchpad.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Uniframework.SharePoint.Linq
{
    internal class ContinuationTable : Dictionary<PropertyInfo, ContinuationTableEntry>
    {
    }

    internal class ContinuationTableEntry
    {
        /// <summary>
        /// Field attribute of referenced field on the root entity. Used in conjunction with Property or Lookup.
        /// </summary>
        public FieldAttribute FieldAttribute { get; private set; }

        /// <summary>
        /// Used for entity references.
        /// </summary>
        /// <example>select new { Product = p }</example>
        public Type Entity { get; private set; }

        /// <summary>
        /// Property on the root entity.
        /// </summary>
        /// <example>select new { Name = p.ProductName }</example>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Lookup reference for lookup entity properties.
        /// </summary>
        /// <example>select new { Supplier = p.Supplier.CompanyName }</example>
        public ContinuationTableLookupEntry Lookup { get; private set; }

        /// <summary>
        /// Type of the continuation.
        /// </summary>
        public ContinuationType Type { get; private set; }

        #region Factory pattern

        private ContinuationTableEntry()
        {
        }

        public static ContinuationTableEntry FromEntity(Type entity)
        {
            ContinuationTableEntry entry = new ContinuationTableEntry();
            entry.Type = ContinuationType.Entity;
            entry.Entity = entity;
            return entry;
        }

        public static ContinuationTableEntry FromProperty(PropertyInfo property, FieldAttribute field)
        {
            ContinuationTableEntry entry = new ContinuationTableEntry();
            entry.Type = ContinuationType.Property;
            entry.Property = property;
            entry.FieldAttribute = field;
            return entry;
        }

        public static ContinuationTableEntry FromLookup(PropertyInfo property, FieldAttribute field, PropertyInfo childProperty, FieldAttribute childField)
        {
            ContinuationTableEntry entry = new ContinuationTableEntry();
            entry.Type = ContinuationType.Lookup;
            entry.Property = property;
            entry.FieldAttribute = field;
            entry.Lookup = new ContinuationTableLookupEntry() { FieldAttribute = childField, Property = childProperty };
            return entry;
        }

        #endregion
    }

    internal enum ContinuationType
    {
        Entity,
        Property,
        Lookup
    }

    internal class ContinuationTableLookupEntry
    {
        public Type Entity { get; set; }
        public FieldAttribute FieldAttribute { get; set; }
        public PropertyInfo Property { get; set; }
    }

    class QueryParser
    {
        /// <summary>
        /// Continuation table.
        /// </summary>
        private ContinuationTable _continuationTable;

        /// <summary>
        /// Dictionary of continuation tables. Used to resolve cross-references in tables.
        /// </summary>
        private Dictionary<Type, ContinuationTable> _continuationTables;

        /// <summary>
        /// List of projections. Will be used to compile the final projection.
        /// </summary>
        private List<LambdaExpression> _projections;

        private void ParseProjection(LambdaExpression projection, int ppS, int ppE)
        {
            //
            // Equiprojections (u => u) can be ignored.
            // Will take care of continuations in groupby constructs too (group #1 by <grouping> into #2).
            //                                                                                   -------
            //
            if (projection.Parameters[0] == projection.Body)
                return;

            //
            // Any continuation yet?

            //
            bool hasContinuation = _continuationTable != null;

            //
            // There's a projection already, but there's no continuation: we've lost track of meaning, so bail out here.
            //
            if (_results.Projection != null && !hasContinuation)
            {
                this.SecondProjectionExpression(ppS, ppE); /* PARSE ERROR */
                return;
            }

            //
            // Always construct the ViewFields CAML element; the last projection will be the one that mandates the required fields.
            //
            _results.Projection = _factory.ViewFields();

            //
            // Store the projection for compilation later on.
            //
            //_projections.Add(projection);
            if (_results.Project2 == null)
                _results.Project2 = new List<Delegate>();
            _results.Project2.Add(projection.Compile());

            //
            // Support for selection of grouping key inside a continuation (group #1 by <grouping> into #2 select #2.Key)
            //                                                                                                    ------
            //
            if (_results.Grouping != null)
            {
                //
                // Look for IGrouping<,>.Key property.
                //
                MemberExpression me = projection.Body as MemberExpression;
                if (me != null && me.Member is PropertyInfo && me.Member.DeclaringType.IsGenericType && me.Member.DeclaringType.GetGenericTypeDefinition() == typeof(IGrouping<,>) && me.Member.Name == "Key")
                {
                    //
                    // The only view field required is the one that the grouping key is referring to.
                    //
                    _results.ProjectionProperties = new HashSet<PropertyInfo>();
                    _results.ProjectionProperties.Add(_results.GroupField);
                }
                else
                {
                    //
                    // Acts as barrier for groupings that are not of type .Key
                    //
                    _results.Projection.AppendChild(this.AfterGrouping(ppS, ppE)); /* PARSE ERROR */
                    return;
                }
            }
            //
            // Regular projections.
            //
            else
            {
                //
                // Detect continuations using anonymous types.
                //
                BuildContinuationTable(projection.Parameters[0], projection.Body);

                //
                // New set of projection properties.
                //
                _results.ProjectionProperties = new HashSet<PropertyInfo>();

                //
                // Create the set with entity properties used in projection and populate it.
                //
                if (!hasContinuation)
                {
                    //
                    // There wasn't a continuation yet, so we have to scan the whole projection expression.
                    //
                    FindEntityProperties(projection.Body, projection.Parameters[0], _results.ProjectionProperties);
                }
                else
                {
                    //
                    // Check the continuation table for all properties that are referred to.
                    //
                    foreach (ContinuationTableEntry cte in _continuationTable.Values)
                    {
                        //
                        // Reference to the entity: all fields are in scope.
                        //
                        if (cte.Type == ContinuationType.Entity)
                        {
                            _results.ProjectionProperties.UnionWith(Helpers.GetEntityProperties(cte.Entity));
                            break;
                        }
                        //
                        // Reference to a specific property. Takes care of Lookups too.
                        //
                        else
                            _results.ProjectionProperties.Add(cte.Property);
                    }
                }
            }

            //
            // Populate the ViewFields element with FieldRef elements pointing to the properties used in the projection.
            //
            HashSet<string> fields = new HashSet<string>();
            foreach (PropertyInfo property in _results.ProjectionProperties)
            {
                //
                // Get the field and field name corresponding to the current entity property.
                //
                XmlElement field = GetFieldRef(property);
                string fieldName = field.Attributes["Name"].Value;

                //
                // Filter for duplicates that can occur because of multi-choice values with fill-in choices.
                //
                if (field != null && !fields.Contains(fieldName))
                {
                    fields.Add(fieldName);
                    _results.Projection.AppendChild(field);
                }
            }
        }

        private void BuildContinuationTable(ParameterExpression p, Expression ex)
        {
            var c = new ContinuationTable();
            Type t;
            if ((t = BuildContinuationTable(p, ex, c)) != null)
            {
                _continuationTable = c;
                _continuationTables.Add(t, c);
            }
        }

        private Type BuildContinuationTable(ParameterExpression p, Expression ex, ContinuationTable table)
        {
            //
            // Only anonymous type creations can cause valid continuations.
            //
            NewExpression ne = ex as NewExpression;
            if (ne != null && IsAnonymousType(ne.Constructor.DeclaringType))
            {
                Type t = ne.Constructor.DeclaringType;

                //
                // Check for arguments that are just aliases for the entity or an entity property.
                //
                for (int i = 0; i < ne.Arguments.Count; i++)
                {
                    MemberExpression me;

                    //
                    // Get the rhs and lhs of new { <name := lhs> = <arg := rhs> }.
                    //
                    Expression arg = ne.Arguments[i];
                    string name = ne.Members[i].Name.Substring("get_".Length);
                    PropertyInfo prop = t.GetProperty(name);

                    //
                    // Keep reference to the entity itself, or deal with continuation.
                    //
                    if (arg == p)
                    {
                        if (_continuationTable == null)
                            table.Add(prop, ContinuationTableEntry.FromEntity(_results.EntityType));
                        else
                        {
                            //
                            // Merge tables because there's a reference to the previous continuation type, so all original mappings are still available.
                            //
                            foreach (PropertyInfo key in _continuationTable.Keys)
                                if (!table.ContainsKey(key))
                                    table.Add(key, _continuationTable[key]);
                        }
                    }
                    //
                    // Find recursive case.
                    //
                    else if (arg is NewExpression)
                    {
                        BuildContinuationTable(p, arg, table);
                    }
                    //
                    // Find reference to entity property.
                    //
                    else if ((me = arg as MemberExpression) != null)
                    {
                        //
                        // For continuations, check whether the target is already in the table. If so, copy to the new table.
                        // This will also take care of deep copies of nested continuations if one is referred to.
                        //
                        if (_continuationTable != null)
                        {
                            //
                            // Check for raw reference.
                            //
                            PropertyInfo tgt = me.Member as PropertyInfo;
                            if (tgt != null && _continuationTable.ContainsKey(tgt))
                            {
                                table.Add(prop, _continuationTable[tgt]);
                                continue;
                            }

                            //
                            // Check for reference to anonymous type.
                            //
                            if (IsAnonymousType(tgt.PropertyType))
                            {
                                //
                                // Check in dictionary of continuations.
                                //
                                if (_continuationTables.ContainsKey(tgt.PropertyType))
                                {
                                    //
                                    // Merge tables.
                                    //
                                    foreach (PropertyInfo key in _continuationTables[tgt.PropertyType].Keys)
                                        if (!table.ContainsKey(key))
                                            table.Add(key, _continuationTables[tgt.PropertyType][key]);
                                }
                                //
                                // New nested anonymous type.
                                //
                                else
                                {
                                    Queue<Type> q = new Queue<Type>();
                                    q.Enqueue(tgt.PropertyType);
                                    while (q.Count > 0)
                                    {
                                        foreach (PropertyInfo pi in q.Dequeue().GetProperties())
                                        {
                                            //
                                            // Recursive case.
                                            //
                                            if (IsAnonymousType(pi.PropertyType))
                                                q.Enqueue(pi.PropertyType);
                                            //
                                            // Copy in existing references; the same key could already be present.
                                            //
                                            else if (_continuationTable.ContainsKey(pi) && !table.ContainsKey(pi))
                                                table.Add(pi, _continuationTable[pi]);
                                        }
                                    }
                                }
                            }
                        }

                        FieldAttribute fa;
                        PropertyInfo target = FindEntityProperty(me, out fa);
                        if (target != null)
                        {
                            //
                            // Reference to entity property on the entity itself.
                            //
                            if (target.DeclaringType == _results.EntityType)
                            {
                                table.Add(prop, ContinuationTableEntry.FromProperty(target, fa));
                            }
                            //
                            // Lookup(Multi) fields can refer to another entity's property.
                            //
                            else
                            {
                                //
                                // Variables me2 and fa2 will refer to the parent of the current expression, which should be a lookup field on the root entity.
                                //
                                MemberExpression me2 = me.Expression as MemberExpression;
                                if (me2 != null)
                                {
                                    //
                                    // The lookup field could be referenced through a field on the previous continuation. Check for this situation.
                                    //
                                    if (_continuationTable != null)
                                    {
                                        PropertyInfo tgt2 = me2.Member as PropertyInfo;
                                        ContinuationTableEntry el;
                                        if (tgt2 != null && _continuationTable.TryGetValue(tgt2, out el) && el.Type == ContinuationType.Property && el.Property.DeclaringType == _results.EntityType)
                                        {
                                            table.Add(prop, ContinuationTableEntry.FromLookup(el.Property, el.FieldAttribute, target, fa));
                                            continue;
                                        }
                                    }

                                    FieldAttribute fa2;
                                    PropertyInfo target2 = FindEntityProperty(me2, out fa2);

                                    if (target2 != null && target2.DeclaringType == _results.EntityType)
                                        table.Add(prop, ContinuationTableEntry.FromLookup(target2, fa2, target, fa));
                                }
                            }
                        }
                    }
                }
                return ne.Constructor.DeclaringType;
            }
            else
                return null;
        }

        private static bool IsAnonymousType(Type t)
        {
            return t.Name.Contains("AnonymousType") && t.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0;
        }

        private PropertyInfo FindEntityProperty(MemberExpression member, out FieldAttribute field)
        {
            field = null;

            PropertyInfo prop = member.Member as PropertyInfo;
            if (prop != null)
            {
                field = Helpers.GetFieldAttribute(prop);
                if (field != null)
                    return prop;
            }

            return null;
        }

        private void CompileProjection()
        {
            //
            // No projection.
            //
            if (_projections.Count == 0)
                return;
            //
            // One projection. Non-continuation case. The input will be an entity object, so we can compile straight away.
            //
            else if (_projections.Count == 1)
                _results.Projection = _projections[0].Compile();
            //
            // More than one projection. This will be caused by a continuation.
            // We need to bypass all intermediate projections and wrap it in one single projection that takes an entity object as its input.
            //
            else
            {
                //
                // Parameter of the resulting projection.
                //
                ParameterExpression parameter = Expression.Parameter(_results.EntityType, "p");

                //
                // Last projection.
                //
                LambdaExpression input = _projections[_projections.Count - 1];
                Expression body = CompileProjection(parameter, input.Body);

                //
                // Get resulting projection.
                //
                LambdaExpression proj = Expression.Lambda(body, parameter);
            }
        }

        private Expression CompileProjection(ParameterExpression p, Expression body)
        {
            if (body == null)
                return null;

            BinaryExpression bin = body as BinaryExpression;
            MethodCallExpression mc = body as MethodCallExpression;
            UnaryExpression u = body as BinaryExpression;
            ConditionalExpression cond = body as ConditionalExpression;
            ConstantExpression cst = body as ConstantExpression;
            InvocationExpression inv = body as InvocationExpression;
            ListInitExpression li = body as ListInitExpression;
            MemberInitExpression mi = body as MemberInitExpression;
            NewExpression ne = body as NewExpression;
            NewArrayExpression nae = body as NewArrayExpression;
            ParameterExpression pe = body as ParameterExpression;
            MemberExpression me = body as MemberExpression;

            switch (body.NodeType)
            {
                case ExpressionType.Add:
                    return Expression.Add(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.AddChecked:
                    return Expression.AddChecked(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.And:
                    return Expression.And(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.AndAlso:
                    return Expression.AndAlso(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.ArrayIndex:
                    return Expression.ArrayIndex(CompileProjection(p, mc.Object), mc.Arguments.Select(arg => CompileProjection(p, arg)));
                case ExpressionType.ArrayLength:
                    return Expression.ArrayLength(CompileProjection(p, u.Operand));
                case ExpressionType.Call:
                    return Expression.Call(CompileProjection(p, mc.Object), mc.Method, mc.Arguments.Select(arg => CompileProjection(p, arg)));
                case ExpressionType.Coalesce:
                    return Expression.Coalesce(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), CompileProjection(p, bin.Conversion));
                case ExpressionType.Conditional:
                    return Expression.Condition(CompileProjection(p, cond.Test), CompileProjection(p, cond.IfTrue), CompileProjection(p, cond.IfFalse));
                case ExpressionType.Constant:
                    return Expression.Constant(cst.Value);
                case ExpressionType.Convert:
                    return Expression.Convert(CompileProjection(p, u.Operand), u.Type, u.Method);
                case ExpressionType.ConvertChecked:
                    return Expression.ConvertChecked(CompileProjection(p, u.Operand), u.Type, u.Method);
                case ExpressionType.Divide:
                    return Expression.Divide(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.Equal:
                    return Expression.Equal(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.IsLiftedToNull, bin.Method);
                case ExpressionType.ExclusiveOr:
                    return Expression.ExclusiveOr(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.GreaterThan:
                    return Expression.GreaterThan(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.IsLiftedToNull, bin.Method);
                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.IsLiftedToNull, bin.Method);
                case ExpressionType.Invoke:
                    return Expression.Invoke(CompileProjection(p, inv.Expression), inv.Arguments.Select(arg => CompileProjection(p, arg)));
                case ExpressionType.Lambda:
                    //TODO
                case ExpressionType.LeftShift:
                    return Expression.LeftShift(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.LessThan:
                    return Expression.LessThan(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.IsLiftedToNull, bin.Method);
                case ExpressionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.IsLiftedToNull, bin.Method);
                case ExpressionType.ListInit:
                    return Expression.ListInit(CompileProjection(p, li.NewExpression), li.Initializers.Select(init => Expression.ElementInit(init.AddMethod, init.Arguments.Select(arg => CompileProjection(p, arg)))));
                case ExpressionType.MemberAccess:
                    if (me.Member is FieldInfo)
                        return Expression.Field(CompileProjection(p, me.Expression), (FieldInfo)me.Member);
                    else
                    {
                        PropertyInfo prop = (PropertyInfo)me.Member;
                        ContinuationTableEntry cte;
                        if (_continuationTable.TryGetValue(prop, out cte))
                        {
                            switch (cte.Type)
                            {
                                case ContinuationType.Entity:
                                    return p;
                                case ContinuationType.Property:
                                    return Expression.Property(p, cte.Property);
                                case ContinuationType.Lookup:
                                    return Expression.Property(Expression.Property(p, cte.Property), cte.Lookup.Property);
                                default:
                                    throw new Exception("Bang!"); //TODO
                            }
                        }
                        else
                            return Expression.Property(CompileProjection(p, me.Expression), (PropertyInfo)me.Member);
                    }
                case ExpressionType.MemberInit:
                    return Expression.MemberInit(
                        CompileProjection(p, mi.NewExpression), 
                        mi.Bindings.Select(bind =>
                        {
                            switch (bind.BindingType)
                            {
                                case MemberBindingType.Assignment:
                                    MemberAssignment ma = (MemberAssignment)bind;
                                    return Expression.Bind(ma.Member, CompileProjection(p, ma.Expression));
                                    break;
                                case MemberBindingType.ListBinding:
                                    MemberListBinding ml = (MemberListBinding)bind;
                                    return Expression.ListBind(ml.Member, ml.Initializers.Select(init => Expression.ElementInit(init.AddMethod, init.Arguments.Select(arg => CompileProjection(p, arg)))));
                                    break;
                                case MemberBindingType.MemberBinding:
                                    MemberMemberBinding mm = (MemberMemberBinding)bind;
                                    return Expression.MemberBind(mm.Member, mm.Bindings.Select(b => ));
                                    break;
                            }
                        })
                    );
                case ExpressionType.Modulo:
                    return Expression.Modulo(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.Multiply:
                    return Expression.Multiply(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.MultiplyChecked:
                    return Expression.MultiplyChecked(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.Negate:
                    return Expression.Negate(CompileProjection(p, u.Operand), u.Method);
                case ExpressionType.NegateChecked:
                    return Expression.NegateChecked(CompileProjection(p, u.Operand), u.Method);
                case ExpressionType.New:
                    return Expression.New(ne.Constructor, ne.Arguments.Select(arg => CompileProjection(p, arg)), ne.Members);
                case ExpressionType.NewArrayBounds:
                    return Expression.NewArrayBounds(nae.Type, nae.Expressions.Select(ex => CompileProjection(p, ex)));
                case ExpressionType.NewArrayInit:
                    return Expression.NewArrayInit(nae.Type, nae.Expressions.Select(ex => CompileProjection(p, ex)));
                case ExpressionType.Not:
                    return Expression.Not(CompileProjection(p, u.Operand), u.Method);
                case ExpressionType.NotEqual:
                    return Expression.NotEqual(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.IsLiftedToNull, bin.Method);
                case ExpressionType.Or:
                    return Expression.Or(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.OrElse:
                    return Expression.OrElse(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.Parameter:
                    if (pe == p)
                    return Expression.Parameter(pe.Type, pe.Name); //CHECK
                case ExpressionType.Power:
                    return Expression.Power(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.Quote:
                    return Expression.Quote(CompileProjection(p, u.Operand));
                case ExpressionType.RightShift:
                    return Expression.RightShift(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.Subtract:
                    return Expression.Subtract(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.SubtractChecked:
                    return Expression.SubtractChecked(CompileProjection(p, bin.Left), CompileProjection(p, bin.Right), bin.Method);
                case ExpressionType.TypeAs:
                    return Expression.TypeAs(CompileProjection(p, u.Operand), u.Type);
                case ExpressionType.TypeIs:
                    return Expression.TypeIs(CompileProjection(p, u.Operand), u.Type);
                case ExpressionType.UnaryPlus:
                    return Expression.UnaryPlus(CompileProjection(p, u.Operand), u.Method);
                default:
                    throw new Exception("Bang!"); //TODO
            }
        }
    }
}

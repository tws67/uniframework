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
 * 0.2.1 - Introduction of QueryParser.
 */

#region Namespace imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

#endregion

namespace Uniframework.SharePoint.Linq
{
    internal class QueryParser
    {
        #region Private members

        /// <summary>
        /// Factory for CAML fragments.
        /// </summary>
        internal CamlFactory _factory;

        /// <summary>
        /// Parse error collection, used when running the parser in validation mode.
        /// </summary>
        internal ParseErrorCollection _errors = null;

        /// <summary>
        /// Expression being parsed.
        /// </summary>
        private Expression _expression;

        //
        // Results of the parser.
        //
        private QueryInfo _results = new QueryInfo();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the query parser for the given expression.
        /// </summary>
        /// <param name="expression">Expression to be parsed.</param>
        /// <param name="validate">Indicates the mode the parser is run in; if true, the parser will validate the specified expression and build a collection with parse errors.</param>
        public QueryParser(Expression expression, bool validate)
        {
            _expression = expression;

            if (validate)
            {
                _errors = new ParseErrorCollection();
                _errors.Expression = expression.ToString();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the parse operation.
        /// </summary>
        public QueryInfo Parse()
        {
            //
            // Do the recursive parsing.
            //
            Parse(_expression);

            //
            // Return parser results.
            //
            return _results;
        }

        private void Parse(Expression expression)
        {
            MethodCallExpression mce = expression as MethodCallExpression;
            ConstantExpression ce = expression as ConstantExpression;

            //
            // Initialize position tracking.
            //
            int ppS;
            int ppE = expression.ToString().Length - 1;

            //
            // Method call expression represents a query operator from the System.Linq.Queryable type.
            //
            if (mce != null && mce.Method.DeclaringType == typeof(Queryable))
            {
                //
                // Depth-first parsing of the expression tree.
                //
                Parse(mce.Arguments[0]);

                //
                // Check the extension method called during query creation.
                //
                bool error = false;
                ppS = mce.Arguments[0].ToString().Length + 1;
                switch (mce.Method.Name)
                {
                    //
                    // Query expression for filtering.
                    //
                    case "Where":
                        {
                            //
                            // Original call = Queryable::Where(source, predicate)
                            //                 where predicate is of type Expression<Func<TSource, bool>>
                            // Parse the query based on the Func<TSource, bool> predicate expression tree.
                            //
                            Type expressionFunc = mce.Method.GetParameters()[1].ParameterType.GetGenericArguments()[0];
                            if (expressionFunc.GetGenericArguments().Length == 2)
                            {
                                if (GuardProjection(mce.Method.Name, ppS, ppE) || // Where after project -> lost meaning (TODO: continuations)
                                    GuardGrouping(mce.Method.Name, ppS, ppE) ||   // Where after group   -> IGrouping<K,T> objects not known by SharePoint
                                    GuardTake(mce.Method.Name, ppS, ppE))         // Where after take    -> can't put a predicate on a reduced row set server-side
                                    return;

                                ParsePredicate((LambdaExpression)((UnaryExpression)mce.Arguments[1]).Operand, ppS, ppE);
                            }
                            else
                                error = true;
                            break;
                        }
                    //
                    // Query expression for sorting. Multiple possibilities exist and can act cumulatively.
                    //
                    case "OrderBy":
                    case "OrderByDescending":
                    case "ThenBy":
                    case "ThenByDescending":
                        {
                            //
                            // Original call = Queryable::{OrderBy|ThenBy}[Descending](source, keySelector)
                            //                 where keySelector is of type Expression<Func<TSource, TKey>>
                            // Parse the query based on the sort Expression<Func<TSource, TKey>> key selector expression tree; keep track of descending sorts.
                            //
                            bool orderBy = mce.Method.Name.StartsWith("O", StringComparison.Ordinal);
                            if (mce.Method.GetParameters().Length == 2)
                            {
                                if (GuardProjection(mce.Method.Name, ppS, ppE) || // Order after project -> lost meaning (TODO: continuations)
                                    GuardGrouping(mce.Method.Name, ppS, ppE) ||   // Order after group   -> IGrouping<K,T> objects not known by SharePoint
                                    GuardTake(mce.Method.Name, ppS, ppE))         // Order after take    -> can't sort a reduced row set server-side
                                    return;

                                ParseOrdering((LambdaExpression)((UnaryExpression)mce.Arguments[1]).Operand, orderBy, mce.Method.Name.EndsWith("g", StringComparison.Ordinal), ppS + mce.Method.Name.Length + 1, ppE - 1);
                            }
                            else
                                error = true;
                            break;
                        }
                    //
                    // Query expression for projection.
                    //
                    case "Select":
                        {
                            //
                            // Original call = Queryable::Select(source, selector)
                            //                 where selector is of type Expression<Func<TSource, TResult>>
                            // Parse the query based on the Expression<Func<TSource, TResult>> selector.
                            //
                            Type expressionFunc = mce.Method.GetParameters()[1].ParameterType.GetGenericArguments()[0];
                            if (expressionFunc.GetGenericArguments().Length == 2)
                                ParseProjection((LambdaExpression)((UnaryExpression)mce.Arguments[1]).Operand, ppS, ppE);
                            else
                                error = true;
                            break;
                        }
                    //
                    // Query expression for result restriction ("TOP").
                    //
                    case "Take":
                        {
                            if (GuardGrouping(mce.Method.Name, ppS, ppE)) // Take after group -> IGrouping<K,T> objects not known by SharePoint
                                return;

                            //
                            // Original call = Queryable::Take(source, count)
                            // Parse the query based on the count value obtained by compilation and dynamic invocation of the count argument to the call.
                            //
                            SetResultRestriction((int)Expression.Lambda<Func<int>>(mce.Arguments[1]).Compile().DynamicInvoke());
                            break;
                        }
                    //
                    // First and FirstOrDefault are based on the Take(1) operation (row number restriction).
                    //
                    case "First":
                    case "FirstOrDefault":
                        {
                            if (GuardGrouping(mce.Method.Name, ppS, ppE)) // First after group -> IGrouping<K,T> objects not known by SharePoint
                                return;

                            //
                            // Original call = Queryable::First(source[, predicate])
                            //                 Queryable::FirstOrDefault(source[, predicate])
                            //
                            // Check for predicate (2nd parameter)
                            //
                            if (mce.Method.GetParameters().Length == 2)
                            {
                                //
                                // Parameter signature = Expression<Func<TSource, bool>> predicate
                                //                                  +-----------------+
                                //                                   => expressionFunc
                                //
                                Type expressionFunc = mce.Method.GetParameters()[1].ParameterType.GetGenericArguments()[0];
                                if (expressionFunc.GetGenericArguments().Length == 2)
                                {
                                    if (GuardProjection(mce.Method.Name, ppS, ppE) || // Where after project -> lost meaning (TODO: continuations)
                                        GuardGrouping(mce.Method.Name, ppS, ppE) ||   // Where after group   -> IGrouping<K,T> objects not known by SharePoint
                                        GuardTake(mce.Method.Name, ppS, ppE))         // Where after take    -> can't put a predicate on a reduced row set server-side
                                        return;

                                    ParsePredicate((LambdaExpression)((UnaryExpression)mce.Arguments[1]).Operand, ppS, ppE);
                                }
                            }

                            //
                            // Set row restriction (first = 1 row only).
                            //
                            SetResultRestriction(1);
                            break;
                        }
                    //
                    // Grouping support.
                    //
                    case "GroupBy":
                        {
                            //
                            // Original call = Queryable::GroupBy(source, keySelector) -> only overload with two parameters
                            //
                            if (mce.Method.GetParameters().Length == 2)
                            {
                                if (GuardProjection(mce.Method.Name, ppS, ppE) || // Group after project -> lost meaning (TODO: continuations)
                                    GuardTake(mce.Method.Name, ppS, ppE))         // Group after take    -> can't group a reduced row set server-side
                                    return;

                                ParseGrouping((LambdaExpression)((UnaryExpression)mce.Arguments[1]).Operand, ppS, ppE);
                            }
                            else
                                error = true;
                            break;
                        }
                    //
                    // Currently we don't support additional query operators in LINQ to SharePoint.
                    //
                    default:
                        error = true;
                        break;
                }
                if (error)
                    this.UnsupportedQueryOperator(mce.Method.Name, ppS, ppE); /* PARSE ERROR */
            }
            //
            // Constant expression represents the source of the query.
            //
            else if (ce != null)
            {
                Type t = ce.Value.GetType();
                if (t.GetGenericTypeDefinition() == typeof(SharePointList<>))
                {
                    //
                    // Store the entity type.
                    //
                    _results.Context = (SharePointDataContext)t.GetProperty("Context").GetValue(ce.Value, null);
                    _results.EntityType = t.GetGenericArguments()[0];
                }
            }
            else
                this.UnsupportedQueryExpression(0, ppE); /* PARSE ERROR */
        }

        #endregion

        #region Query predicate expression parsing (Where)

        /// <summary>
        /// Parses a query filter expression, resulting in a CAML Where element.
        /// </summary>
        /// <param name="predicate">Lambda expression of the query predicate to parse.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        private void ParsePredicate(LambdaExpression predicate, int ppS, int ppE)
        {
            //
            // Calculcate expression body parser positions.
            // E.g. Where(t => ((t.Age >= 24) && t.LastName.StartsWith("Smet")))
            //      ++++++0xxxx                                                -
            // ppS <- ppS + 6(+) + predicate.Parameters[0].Name.Length + 4(x)
            // ppE <- ppE - 1(-)
            //
            ppS += 10 + predicate.Parameters[0].Name.Length;
            ppE -= 1;

            //
            // Parse the predicate recursively, starting without negation (last parameter "positive" set to true).
            //
            LookupInfo lookup;
            XmlElement pred = ParsePredicate(predicate.Body, predicate.Parameters[0], true, out lookup, ppS, ppE);

            //
            // Predicate can be null because of optimizations.
            //
            if (pred != null)
            {
                //
                // Patches required for lookup fields?
                //
                if (lookup != null)
                    PatchQueryExpressionNode(lookup, ref pred);

                //
                // If this is the first predicate, create a new <Where> element.
                //
                if (_results.Where == null)
                {
                    _results.Where = _factory.Where(pred);
                }
                //
                // Otherwise, add the new predicate to the existing one using an <And> element.
                //
                else
                {
                    _results.Where = _factory.Where(
                        _factory.And(pred, _results.Where.FirstChild)
                    );
                }
            }
        }

        /// <summary>
        /// Parses the given predicate recursively, building up the given query predicate element.
        /// </summary>
        /// <param name="predicate">Predicate expression to be parsed.</param>
        /// <param name="predicateParameter">Parameter of the predicate lambda expression. Used to detect references to the entity type itself.</param>
        /// <param name="isPositive">Indicates whether the predicate should be evaluated as a positive condition or not; serves boolean negation using De Morgan's law.</param>
        /// <param name="lookup">Output parameter for Lookup fields, used to build the query expression for a lookup field.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Output XML element representing the parsed predicate in CAML syntax.</returns>
        private XmlElement ParsePredicate(Expression predicate, ParameterExpression predicateParameter, bool isPositive, out LookupInfo lookup, int ppS, int ppE)
        {
            BinaryExpression be;
            UnaryExpression ue;
            MethodCallExpression mce;
            MemberExpression me;
            ConstantExpression ce;

            //
            // By default, no lookup.
            //
            lookup = null;

            //
            // The given predicate can be a binary expression containing either boolean expressions or conditions that require further parsing.
            //
            if ((be = predicate as BinaryExpression) != null)
            {
                return ParsePredicateBinary(predicate, predicateParameter, isPositive, be, ref lookup, ppS, ppE);
            }
            //
            // A unary expression will occur for boolean negation.
            //
            else if ((ue = predicate as UnaryExpression) != null)
            {
                return ParsePredicateUnary(predicate, predicateParameter, isPositive, ue, ref lookup, ppS, ppE);
            }
            //
            // Converts the unary boolean evaluation like "where u.Member.Value select" or "where u.Age.HasValue select".
            // 
            else if ((me = predicate as MemberExpression) != null)
            {
                return ParsePredicateMember(isPositive, me, ref lookup, ppS, ppE);
            }
            //
            // Method calls are supported for a limited set of string operations and for LookupMulti fields.
            //
            else if ((mce = predicate as MethodCallExpression) != null)
            {
                return ParsePredicateMethodCall(predicate, predicateParameter, isPositive, mce, ref lookup, ppS, ppE);
            }
            //
            // Constant values are possible if the user writes clauses like "1 == 1" in the query's where predicate.
            //
            else if ((ce = predicate as ConstantExpression) != null)
            {
                //
                // The value should be boolean-valued.
                //
                if (ce.Value is bool)
                    return _factory.BooleanPatch((bool)ce.Value);
                else
                    return /* PARSE ERROR */ this.NonBoolConstantValueInPredicate(ppS, ppE);
            }

            //
            // Fall-through case (shouldn't occur under normal circumstances).
            //
            ParseErrors.FatalError(); /* PARSE ERROR */
            return null;
        }

        /// <summary>
        /// Helper method to parse member expressions in a query predicate.
        /// </summary>
        /// <param name="isPositive">Indicates whether the predicate should be evaluated as a positive condition or not; serves boolean negation using De Morgan's law.</param>
        /// <param name="me">Member expression to parse.</param>
        /// <param name="lookup">Output parameter for Lookup fields, used to build the query expression for a lookup field.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Output XML element representing the parsed predicate member expression in CAML syntax.</returns>
        private XmlElement ParsePredicateMember(bool isPositive, MemberExpression me, ref LookupInfo lookup, int ppS, int ppE)
        {
            //
            // Check for (and trim) Nullable wrapper.
            //
            bool? isNullableHasValue;
            Expression res = CheckForNullableType(me, out isNullableHasValue);

            //
            // Did we find an entity reference?
            //
            if (IsEntityPropertyReference(res))
            {
                MemberExpression mRes = res as MemberExpression;

                //
                // Check for lookup field to propagate lookup query expressions to parent.
                //
                if (!FindLookups(mRes, ref lookup))
                    return /* PARSE ERROR */ this.InvalidEntityReference(me.Member.Name, ppS, ppE);

                me = mRes;

                XmlElement c;

                //
                // Call to .HasValue? If so, convert to IsNull or IsNotNull.
                //
                if (isNullableHasValue.HasValue && isNullableHasValue.Value)
                    c = isPositive ? _factory.IsNotNull() : _factory.IsNull();
                //
                // No .HasValue should be either .Value or a non-Nullable boolean. Convert to <Eq> with the member's value.
                //
                else
                {
                    c = _factory.Eq();
                    bool isLookup;
                    c.AppendChild(GetValue(isPositive, Helpers.GetFieldAttribute((PropertyInfo)me.Member), out isLookup));
                }

                //
                // Append field reference.
                //
                c.AppendChild(GetFieldRef((PropertyInfo)me.Member));
                return c;
            }
            else
            {
                bool b = (bool)Expression.Lambda<Func<bool>>(me).Compile().DynamicInvoke();
                return _factory.BooleanPatch(b);
            }
        }

        /// <summary>
        /// Helper method to parse method calls in a query predicate.
        /// </summary>
        /// <param name="predicate">Predicate expression to be parsed.</param>
        /// <param name="predicateParameter">Parameter of the predicate lambda expression. Used to detect references to the entity type itself.</param>
        /// <param name="isPositive">Indicates whether the predicate should be evaluated as a positive condition or not; serves boolean negation using De Morgan's law.</param>
        /// <param name="mce">Method call expression to parse.</param>
        /// <param name="lookup">Output parameter for Lookup fields, used to build the query expression for a lookup field.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Output XML element representing the parsed predicate method call expression in CAML syntax.</returns>
        private XmlElement ParsePredicateMethodCall(Expression predicate, ParameterExpression predicateParameter, bool isPositive, MethodCallExpression mce, ref LookupInfo lookup, int ppS, int ppE)
        {
            //
            // Check for CamlElements methods.
            //
            if (mce.Method.DeclaringType == typeof(CamlMethods))
            {
                //
                // DateRangesOverlap support.
                //
                if (mce.Method.Name == "DateRangesOverlap")
                {
                    return ParseDateRangesOverlap(predicate, isPositive, mce, ref lookup, ppS, ppE);
                }
            }
            //
            // Check for Any call on LookupMulti field.
            //
            else if (mce.Method.DeclaringType == typeof(Enumerable))
            {
                ParameterInfo[] parameters = mce.Method.GetParameters();
                if (mce.Method.Name == "Any" && parameters.Length == 2)
                {
                    //
                    // First argument is the extension 'this' parameter. It should be an entity property reference.
                    //
                    MemberExpression lookupMultiField = (MemberExpression)mce.Arguments[0];
                    if (!IsEntityPropertyReference(lookupMultiField))
                        return /* PARSE ERROR */ this.PredicateContainsNonEntityMethodCall(mce.Method.Name, ppS, ppE);

                    //
                    // Check the entity property reference is of type LookupMulti.
                    //
                    FieldAttribute lookupMultiFieldAttribute = Helpers.GetFieldAttribute((PropertyInfo)lookupMultiField.Member);
                    if (lookupMultiFieldAttribute.FieldType != FieldType.LookupMulti)
                        return /* PARSE ERROR */ this.AnyOnNonLookupMultiField(ppS, ppE);

                    //
                    // Only supported in positive contexts.
                    //
                    if (!isPositive)
                        return /* PARSE ERROR */ this.LookupMultiAnyOnNonPositiveContext(ppS, ppE);

                    //
                    // Parse the entity property reference.
                    //
                    LookupInfo lookupMulti = new LookupInfo();
                    if (!FindLookups(lookupMultiField, ref lookupMulti))
                        return /* PARSE ERROR */ this.InvalidEntityReference(lookupMultiField.Member.Name, ppS, ppE);

                    //
                    // Add patch reference to self (= LookupMulti field).
                    //
                    lookupMulti.Add((PropertyInfo)lookupMultiField.Member);

                    //
                    // Calculcate expression body parser positions.
                    // E.g. ___.Any(t => ((t.Age >= 24) && t.LastName.StartsWith("Smet")))
                    //         +++++0xxxx                                                -
                    // ppSL <- ppS + 5(+) + predicate.Parameters[0].Name.Length + 4(x) + lookupMultiField.ToString().Length(_)
                    // ppEL <- ppE - 1(-)
                    //
                    ppS = ppS + 10 + lookupMultiField.ToString().Length;
                    ppE = ppE - 1;

                    //
                    // Second argument is the Expression<Func<T,bool>> parameter.
                    //
                    LambdaExpression lookupMultiSubquery = (LambdaExpression)mce.Arguments[1];
                    LookupInfo lookupMultiSub;

                    //
                    // Parse the Any predicate.
                    //
                    XmlElement lookupMultiSubqueryResult = ParsePredicate(lookupMultiSubquery.Body, lookupMultiSubquery.Parameters[0], true, out lookupMultiSub, ppS, ppE);

                    //
                    // Patch for the inner query and for the outer field reference.
                    // E.g.: p.Bar.Foo.Any(l => l.Sub.Test == "xyz") gets two patches
                    //       <Patch Field="Bar">
                    //         <Patch Field="Sub">
                    //           <Eq ... />
                    //         </Patch>
                    //       </Patch>
                    //
                    PatchQueryExpressionNode(lookupMultiSub, ref lookupMultiSubqueryResult);
                    PatchQueryExpressionNode(lookupMulti, ref lookupMultiSubqueryResult);

                    return lookupMultiSubqueryResult;
                }
                else
                    return /* PARSE ERROR */ this.UnsupportedExtensionMethod(mce.Method.Name, ppS, ppE);
            }

            //
            // Check whether the method call is an instance method call.
            // All code above this barrier processes static method calls.
            //
            if (mce.Object == null)
                return /* PARSE ERROR */ this.PredicateContainsNonEntityMethodCall(mce.Method.Name, ppS, ppE);

            //
            // Get object position.
            //
            int ppSO = ppS;
            int ppEO = ppSO + mce.Object.ToString().Length - 1;

            //
            // Only method calls on entity type properties are supported.
            //
            Expression ex = DropToString(mce.Object, ref ppEO);
            bool? isHasValue;
            MemberExpression o = ex as MemberExpression;
            if (o != null)
                o = CheckForNullableType(o, out isHasValue) as MemberExpression;

            if (o == null || !IsEntityPropertyReference(o))
                return /* PARSE ERROR */ this.PredicateContainsNonEntityMethodCall(mce.Method.Name, ppS, ppE);

            PropertyInfo property = (PropertyInfo)o.Member;

            //
            // Check for UrlValue fields.
            //
            if (property.DeclaringType == typeof(Url))
            {
                if (property.Name != "Address")
                {
                    int ppS1 = ppS + o.Expression.ToString().Length + 1;
                    return /* PARSE ERROR */ this.NonAddressPropertyCallOnUrlValue(ppS1, ppS1 + property.Name.Length - 1);
                }

                o = o.Expression as MemberExpression;
                if (o == null || !IsEntityPropertyReference(o))
                    return /* PARSE ERROR */ this.PredicateContainsNonEntityMethodCall(mce.Method.Name, ppS, ppE);

                property = (PropertyInfo)o.Member;
            }

            //
            // Check for lookup field to propagate lookup query expressions to parent.
            //
            if (!FindLookups(o, ref lookup))
                return /* PARSE ERROR */ this.InvalidEntityReference(mce.Method.Name, ppS, ppE);

            //
            // Only string operations "Contains", "StartsWith" and "Equals" are supported in CAML.
            //
            if (mce.Method.DeclaringType == typeof(string) && mce.Object != null && mce.Arguments.Count > 0)
            {
                int ppSS = ppS + mce.Object.ToString().Length + ".".Length + mce.Method.Name.Length + "(".Length;
                int ppES = ppSS + mce.Arguments[0].ToString().Length - 1;

                //
                // Get the value of the method call argument and ensure it's lambda parameter free.
                //
                Expression arg = mce.Arguments[0];
                EnsureLambdaFree(arg, predicateParameter, ppSS, ppES);

                //
                // Find the value of the method call argument using lamda expression compilation and dynamic invocation.
                //
                object val = null;

                //
                // When debugging, this might fail.
                //
                if (_errors != null)
                {
                    try
                    {
                        val = Expression.Lambda(arg).Compile().DynamicInvoke();
                    }
                    catch (InvalidOperationException) { } //TODO: in this case, val will be null and no ParseError tag will be injected in the CAML
                }
                //
                // Otherwise, it shouldn't fail since EnsureLambdaFree takes care of pathological situations.
                //
                else
                    val = Expression.Lambda(arg).Compile().DynamicInvoke();

                string sval = val as string;

                //
                // Build the condition.
                //
                XmlElement cond;
                switch (mce.Method.Name)
                {
                    case "Contains":
                        //
                        // Contains "" is always true.
                        //
                        if (String.IsNullOrEmpty(sval))
                            return null;

                        if (!isPositive)
                            return /* PARSE ERROR */ this.CantNegate("Contains", ppS, ppE);

                        cond = _factory.Contains();
                        break;
                    case "StartsWith":
                        //
                        // StartsWith "" is always true.
                        //
                        if (String.IsNullOrEmpty(sval))
                            return null;

                        if (!isPositive)
                            return /* PARSE ERROR */ this.CantNegate("BeginsWith", ppS, ppE);

                        cond = _factory.BeginsWith();
                        break;
                    case "Equals":
                        if (val == null)
                        {
                            cond = !isPositive ? _factory.IsNotNull() : _factory.IsNull();
                            cond.AppendChild(GetFieldRef(property));
                            return cond;
                        }
                        else
                        {
                            cond = !isPositive ? _factory.Neq() : _factory.Eq();
                        }
                        break;
                    default:
                        ppSS = ppS + mce.Object.ToString().Length + 1;
                        return /* PARSE ERROR */ this.UnsupportedStringMethodCall(mce.Method.Name, ppSS, ppE);
                }
                cond.AppendChild(GetFieldRef(property));

                //
                // Set the value on the condition element.
                //
                if (val != null)
                {
                    bool isLookup;
                    cond.AppendChild(GetValue(val, Helpers.GetFieldAttribute(property), out isLookup));
                }
                else
                    return null;

                return cond;
            }
            //
            // LookupMulti fields support the Contains method call.
            //
            else if (mce.Method.DeclaringType.IsGenericType
                     && mce.Method.DeclaringType.GetGenericTypeDefinition() == typeof(EntitySet<>)
                     && mce.Object != null
                     && mce.Arguments.Count > 0
                     && mce.Method.Name == "Contains")
            {
                if (!isPositive)
                    return /* PARSE ERROR */ this.CantNegate("Contains", ppS, ppE);

                XmlElement cond = _factory.Contains();

                int ppSL = ppS + mce.Object.ToString().Length + ".".Length + mce.Method.Name.Length + "(".Length;
                int ppEL = ppSL + mce.Arguments[0].ToString().Length - 1;

                //
                // Get the value of the method call argument and ensure it's lambda parameter free.
                //
                Expression arg = mce.Arguments[0];
                if (!EnsureLambdaFree(arg, predicateParameter, ppSL, ppEL))
                    return null;

                //
                // Find the value of the method call argument using lambda expression compilation and dynamic invocation.
                //
                object val = Expression.Lambda(arg).Compile().DynamicInvoke();

                //
                // Contains(null) is considered to be always true.
                //
                if (val == null)
                    return null;

                //
                // Check type of the Contains parameter to match the entity type.
                //
                if (mce.Method.DeclaringType.GetGenericArguments()[0] != val.GetType())
                    return /* PARSE ERROR */ this.InvalidLookupMultiContainsCall(ppS, ppE);

                //
                // Build condition based on the referenced field and the lookup key field.
                //
                bool isLookup; //should always be true
                XmlElement value = GetValue(val, Helpers.GetFieldAttribute(property), out isLookup);
                XmlElement fieldRef = GetFieldRef(property);
                if (isLookup)
                    fieldRef.Attributes.Append(_factory.LookupAttribute());
                cond.AppendChild(fieldRef);
                cond.AppendChild(value);

                return cond;
            }
            else
            {
                int ppSS = ppS;
                if (mce.Object != null)
                    ppSS += mce.Object.ToString().Length + 1;

                return /* PARSE ERROR */ this.UnsupportedMethodCall(mce.Method.Name, ppSS, ppE);
            }
        }

        /// <summary>
        /// Parses a DateRangesOverlap call on CamlMethods.
        /// </summary>
        /// <param name="predicate">Predicate to parse.</param>
        /// <param name="isPositive">Indicates whether the predicate should be evaluated as a positive condition or not; serves boolean negation using De Morgan's law.</param>
        /// <param name="mce">Method call expression to parse.</param>
        /// <param name="lookup">Output parameter for Lookup fields, used to build the query expression for a lookup field.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Output XML element representing the parsed DateRangesOverlap expression in CAML syntax.</returns>
        private XmlElement ParseDateRangesOverlap(Expression predicate, bool isPositive, MethodCallExpression mce, ref LookupInfo lookup, int ppS, int ppE)
        {
            //
            // Negation isn't supported.
            //
            if (!isPositive)
                return /* PARSE ERROR */ this.CantNegate("DateRangesOverlap", ppS, ppE);

            //
            // Get value argument.
            //
            Expression valEx = mce.Arguments[0];

            bool? isNullableHasValue;
            valEx = CheckForNullableType(valEx, out isNullableHasValue);

            //
            // Value argument shouldn't be an entity property reference.
            //
            if (IsEntityPropertyReference(valEx))
            {
                //
                // Find value argument location in parent expression.
                //
                int ppSD1 = ppS + "DateRangesOverlap(".Length;
                int ppED1 = ppSD1 + mce.Arguments[0].ToString().Length - 1;

                return /* PARSE ERROR */ this.DateRangesOverlapInvalidValueArgument(ppSD1, ppED1);
            }

            //
            // Get value element.
            //
            XmlElement value = GetDateValue(valEx);

            //
            // Field references.
            //
            NewArrayExpression fields = mce.Arguments[1] as NewArrayExpression;
            if (fields == null || fields.Expressions.Count == 0)
            {
                //
                // Find fields argument location in parent expression.
                //
                int ppSD2 = ppS + predicate.ToString().IndexOf(fields.ToString(), StringComparison.Ordinal);
                int ppED2 = ppE - 1;

                return /* PARSE ERROR */ this.DateRangesOverlapMissingFieldReferences(ppSD2, ppED2);
            }

            List<XmlElement> fieldRefs = new List<XmlElement>();

            int ppSD = ppS + "DateRangesOverlap(".Length + mce.Arguments[0].ToString().Length + ", new [] {".Length;
            int ppED = ppE - 2;

            //
            // Find all field expressions.
            //
            foreach (Expression fieldEx in fields.Expressions)
            {
                //
                // Clean-up the field expression.
                //
                Expression fEx = fieldEx;
                while (fEx.NodeType == ExpressionType.Convert || fEx.NodeType == ExpressionType.ConvertChecked)
                    fEx = ((UnaryExpression)fEx).Operand;

                fEx = CheckForNullableType(fEx, out isNullableHasValue);

                if (!IsEntityPropertyReference(fEx))
                    return /* PARSE ERROR */ this.DateRangesOverlapInvalidFieldReferences(ppSD, ppED);

                MemberExpression mex = fEx as MemberExpression;
                if (mex == null || !(mex.Member is PropertyInfo))
                    return /* PARSE ERROR */ this.DateRangesOverlapInvalidFieldReferences(ppSD, ppED);

                //
                // Lookup properties are supported only if all property references are of the same lookup type.
                //
                if (!(mex.Expression is ParameterExpression))
                {
                    LookupInfo lookup1 = new LookupInfo();
                    if (!FindLookups(mex, ref lookup1))
                        return /* PARSE ERROR */ this.DateRangesOverlapInvalidFieldReferences(ppSD, ppED);

                    //
                    // We've already found field references; check that all of these refer to the same entity type.
                    //
                    //if (fieldRefs.Count != 0 && (lookup == null || lookup != lookup1))
                    if (fieldRefs.Count != 0 && (lookup == null || lookup != lookup1))
                        return /* PARSE ERROR */ this.DateRangesOverlapInvalidFieldReferences(ppSD, ppED);
                    else
                        lookup = lookup1;
                }

                //
                // Add field reference element.
                //
                fieldRefs.Add(GetFieldRef((PropertyInfo)mex.Member));

                ppSD += fieldEx.ToString().Length + ", ".Length;
            }

            //
            // Construct and return DateRangesOverlap element.
            //
            XmlElement dro = _factory.DateRangesOverlap();
            dro.AppendChild(value);
            foreach (XmlElement fieldRef in fieldRefs)
                dro.AppendChild(fieldRef);
            return dro;
        }

        /// <summary>
        /// Helper method to parse unary expressions in a query predicate.
        /// </summary>
        /// <param name="predicate">Predicate expression to be parsed.</param>
        /// <param name="predicateParameter">Parameter of the predicate lambda expression. Used to detect references to the entity type itself.</param>
        /// <param name="isPositive">Indicates whether the predicate should be evaluated as a positive condition or not; serves boolean negation using De Morgan's law.</param>
        /// <param name="ue">Unary expression to parse.</param>
        /// <param name="lookup">Output parameter for Lookup fields, used to build the query expression for a lookup field.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Output XML element representing the parsed predicate unary expression in CAML syntax.</returns>
        private XmlElement ParsePredicateUnary(Expression predicate, ParameterExpression predicateParameter, bool isPositive, UnaryExpression ue, ref LookupInfo lookup, int ppS, int ppE)
        {
            //
            // CAML doesn't support boolean negation; therefore, we apply De Morgan's law by inverting the isPositive indicator.
            //
            if (predicate.NodeType == ExpressionType.Not)
            {
                //
                // Calculcate expression body parser positions. Trim Not(.) portion.
                // E.g. Not(t.Age >= 24)
                //      ++++           -
                // ppS <- ppS + 4(+)
                // ppE <- ppE - 1(-
                //
                ppS += 4;
                ppE -= 1;

                LookupInfo lookup1;
                XmlElement c = ParsePredicate(ue.Operand, predicateParameter, !isPositive, out lookup1, ppS, ppE);

                //
                // Optimized away?
                //
                if (c != null)
                {
                    //
                    // If a Lookup reference was detected, propagate the lookup query expression to the parent.
                    //
                    if (lookup1 != null)
                        lookup = lookup1;
                }

                return c;
            }
            else
                return /* PARSE ERROR */ this.UnsupportedUnary(predicate.NodeType.ToString(), ppS, ppE);
        }

        /// <summary>
        /// Helper method to parse binary expressions in a query predicate.
        /// </summary>
        /// <param name="predicate">Predicate expression to be parsed.</param>
        /// <param name="predicateParameter">Parameter of the predicate lambda expression. Used to detect references to the entity type itself.</param>
        /// <param name="isPositive">Indicates whether the predicate should be evaluated as a positive condition or not; serves boolean negation using De Morgan's law.</param>
        /// <param name="be">Binary expression to parse.</param>
        /// <param name="lookup">Output parameter for Lookup fields, used to build the query expression for a lookup field.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Output XML element representing the parsed predicate binary expression in CAML syntax.</returns>
        private XmlElement ParsePredicateBinary(Expression predicate, ParameterExpression predicateParameter, bool isPositive, BinaryExpression be, ref LookupInfo lookup, int ppS, int ppE)
        {
            //
            // Calculcate expression body parser positions: trim outer parentheses.
            // E.g. ((t.Age >= 24) && t.LastName.StartsWith("Smet"))
            //      +                                              -
            // ppS <- ppS + 1(+)
            // ppE <- ppE - 1(-)
            //
            ppS += 1;
            ppE -= 1;

            switch (predicate.NodeType)
            {
                //
                // AndAlso boolean expression (&&, AndAlso)
                // And boolean expression     (&,  And)
                // OrElse boolean expression  (||, OrElse)
                // Or boolean expression      (|,  Or)
                //
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    {
                        XmlElement c;

                        if (predicate.NodeType == ExpressionType.And || predicate.NodeType == ExpressionType.AndAlso)
                            //
                            // If not evaluated positively, apply De Morgan's law: !(a && b) == !a || !b
                            //
                            c = (isPositive ? _factory.And() : _factory.Or());
                        else
                            //
                            // If not evaluated positively, apply De Morgan's law: !(a || b) == !a && !b
                            //
                            c = (isPositive ? _factory.Or() : _factory.And()); // De Morgan

                        //
                        // Calculcate expression body parser positions for lhs and rhs.
                        // E.g. (t.Age >= 24) && t.LastName.StartsWith("Smet")
                        //      S           T    D                           E
                        // ppT <- ppS + be.Left.ToString().Length - 1
                        // ppE <- ppE - be.Right.ToString().Length + 1
                        //
                        int ppT = ppS + be.Left.ToString().Length - 1;
                        int ppD = ppE - be.Right.ToString().Length + 1;

                        LookupInfo lookupLeft, lookupRight;
                        XmlElement left = ParsePredicate(be.Left, predicateParameter, isPositive, out lookupLeft, ppS, ppT);
                        XmlElement right = ParsePredicate(be.Right, predicateParameter, isPositive, out lookupRight, ppD, ppE);

                        //
                        // Optimizations could occur. If none, do the following.
                        //
                        if (left != null && right != null)
                        {
                            //
                            // Try to coalesce both lookups to propagate the common lookup chain to the parent.
                            //
                            if (lookupLeft != null && lookupRight != null)
                                lookup = LookupInfo.Coalesce(ref lookupLeft, ref lookupRight);

                            //
                            // Patch the nodes using the remainder of the coalesced lookup chains.
                            //
                            if (lookupLeft != null)
                                PatchQueryExpressionNode(lookupLeft, ref left);
                            if (lookupRight != null)
                                PatchQueryExpressionNode(lookupRight, ref right);

                            //
                            // Continue to compose the query expression tree.
                            //
                            c.AppendChild(left);
                            c.AppendChild(right);

                            return c;
                        }
                        //
                        // In case of optimization, cut pruned condition children.
                        //
                        else
                        {
                            //
                            // Only left hand side remains.
                            //
                            if (left != null)
                            {
                                //
                                // If a lookup is found, it can be propagated now because we end up with a single node.
                                //
                                if (lookupLeft != null)
                                {
                                    lookup = lookupLeft;
                                    PatchQueryExpressionNode(lookupLeft, ref left);
                                }

                                return left;
                            }
                            //
                            // Only right hand side remains.
                            //
                            else if (right != null)
                            {
                                //
                                // If a lookup is found, it can be propagated now because we end up with a single node.
                                //
                                if (lookupRight != null)
                                {
                                    lookup = lookupRight;
                                    PatchQueryExpressionNode(lookupRight, ref right);
                                }

                                return right;
                            }
                            //
                            // Both sides of the expression are optimized away.
                            //
                            else
                                return null;
                        }
                    }
                //
                // Remaining binary operations are parsed as conditions. Examples include ==, !=, >, <, >=, <=.
                //
                default:
                    {
                        LookupInfo lookup1;
                        XmlElement c = GetCondition(be, isPositive, predicateParameter, out lookup1, ppS, ppE);

                        //
                        // If a Lookup reference was detected, propagate the lookup query expression to the parent.
                        //
                        if (lookup1 != null)
                            lookup = lookup1;

                        return c;
                    }
            }
        }

        /// <summary>
        /// Patches a query expression node by surrounding it with a Patch element so that it can be replaced with lookup field references upon execution.
        /// </summary>
        /// <param name="lookups">Chain of lookup entity properties to make a patch for.</param>
        /// <param name="node">Node of the query expression to be patched.</param>
        private void PatchQueryExpressionNode(LookupInfo lookups, ref XmlElement node)
        {
            //
            // Apply the patch.
            //
            if (lookups != null)
                lookups.Patch(_factory, ref node);
        }

        /// <summary>
        /// Helper method to check whether the given expression is Nullable type wrapper and removes it.
        /// </summary>
        /// <param name="e">Expression to check for Nullable occurrence.</param>
        /// <param name="isHasValue">Output parameter that indicates that the Nullable usage on the given expression was a HasValue member access.</param>
        /// <returns>Nullable-free expression.</returns>
        private static Expression CheckForNullableType(Expression e, out bool? isHasValue)
        {
            MemberExpression me = e as MemberExpression;
            if (me != null && me.Member is PropertyInfo)
            {
                Type t = me.Member.DeclaringType;
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    isHasValue = (me.Member.Name == "HasValue");
                    return me.Expression;
                }
            }

            isHasValue = null;
            return e;
        }

        private static bool FindLookups(MemberExpression me, ref LookupInfo lookup)
        {
            //
            // Prepare (optional) result.
            //
            LookupInfo l = new LookupInfo();

            //
            // Find entity property references in chain.
            //
            bool found = false;
            MemberExpression outer;
            while ((outer = me.Expression as MemberExpression) != null)
            {
                found = true;
                me = outer;

                //
                // If the current member doesn't refer to an entity property, this should be considered a problem.
                //
                if (!IsEntityPropertyReference(outer))
                    return false;

                //
                // Register entity property call in chain.
                //
                l.Add((PropertyInfo)outer.Member);
            }

            //
            // An error has occurred if the chain of member expressions doesn't have the parameter as its root.
            //
            if (!(me.Expression is ParameterExpression))
                return false;

            //
            // Only assign to the lookup result variable if something was found: a non-set lookup result variable will indicate there's no lookup.
            //
            if (found)
                lookup = l;

            return true;
        }

        /// <summary>
        /// Detects the use of Microsoft.VisualBasic.CompilerServices.Operators.CompareString and Microsoft.VisualBasic.Strings.StrComp for string equality checks.
        /// </summary>
        /// <param name="condition">Condition to check for CompareString or StrComp presence.</param>
        /// <param name="left">Left operand, will be rewritten by the CompareString or StrComp first parameter if CompareString or StrComp usage was detected.</param>
        /// <param name="right">Right operand, will be rewritten by the CompareString or StrComp second parameter if CompareString or StrComp usage was detected.</param>
        private static void FindVisualBasicCompareStringCondition(BinaryExpression condition, ref Expression left, ref Expression right)
        {
            //
            // Right hand side should be 0 to indicate string equality.
            //
            ConstantExpression ce = condition.Right as ConstantExpression;
            if (ce != null && ce.Value is int && (int)ce.Value == 0)
            {
                //
                // Check for call to static method Microsoft.VisualBasic.CompilerServices.Operators.CompareString or Microsoft.VisualBasic.Strings.StrComp.
                //
                MethodCallExpression mce = condition.Left as MethodCallExpression;
                if (mce != null && mce.Object == null &&
                    (
                         (mce.Method.Name == "CompareString" && mce.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators")
                      || (mce.Method.Name == "StrComp" && mce.Method.DeclaringType.FullName == "Microsoft.VisualBasic.Strings")
                    )
                   )
                {
                    left = mce.Arguments[0];
                    right = mce.Arguments[1];
                }
            }
        }

        /// <summary>
        /// Get the CAML representation for the specified condition.
        /// </summary>
        /// <param name="condition">Condition to translate into CAML.</param>
        /// <param name="isPositive">Indicates whether the condition should be evaluated in a positive context or not; used for inversion of conditions using De Morgan's law.</param>
        /// <param name="predicateParameter">Parameter of the predicate lambda expression. Used to detect references to the entity type itself.</param>
        /// <param name="lookup">Output parameter for Lookup fields, used to build the query expression for a lookup field.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Output XML element representing the parsed condition in CAML syntax.</returns>
        private XmlElement GetCondition(BinaryExpression condition, bool isPositive, ParameterExpression predicateParameter, out LookupInfo lookup, int ppS, int ppE)
        {
            //
            // Normally, we don't face a lookup field so stick with the default of no lookup (=empty list).
            //
            lookup = new LookupInfo();

            Expression left = condition.Left;
            Expression right = condition.Right;

            int ppT = ppS + left.ToString().Length - 1;
            int ppD = ppE - right.ToString().Length + 1;

            //
            // Detect use of Microsoft.VisualBasic.CompilerServices.Operators.CompareString or Microsoft.VisualBasic.Strings.StrComp.
            //
            FindVisualBasicCompareStringCondition(condition, ref left, ref right);

            //
            // Trim Convert nodes on the both operandi before examining the nodes further on and remove excessive ToString method calls at the end.
            //
            left = TrimConvert(left, ref ppS, ref ppT);
            left = DropToString(left, ref ppT);
            right = TrimConvert(right, ref ppD, ref ppE);
            right = DropToString(right, ref ppE);

            //
            // Detect and trim Nullable wrappers on both arguments. Keep track of .HasValue calls.
            //
            bool? leftIsNullableHasValue;
            left = CheckForNullableType(left, out leftIsNullableHasValue);
            bool? rightIsNullableHasValue;
            right = CheckForNullableType(right, out rightIsNullableHasValue);

            //
            // Is entity property reference?
            //
            bool ieprl = IsEntityPropertyReference(left);
            bool ieprr = IsEntityPropertyReference(right);

            //
            // Detect UrlValue member calls.
            //
            if (!ieprl)
            {
                XmlElement res = CheckForUrlValueUrl(ref left, out ieprl, ppS);
                if (res != null) //PARSE ERROR
                    return res;
            }
            if (!ieprr)
            {
                XmlElement res = CheckForUrlValueUrl(ref right, out ieprr, ppS);
                if (res != null) //PARSE ERROR
                    return res;
            }

            //
            // If the left operand is a member expression (pointing to an entity property), we'll assume "normal ordering".
            // CAML queries always check the field 'f' against a value 'v' in the order f op v where 'op' is the operator.
            //
            bool correctOrder;
            if (ieprl)
                correctOrder = true;
            else if (ieprr)
                correctOrder = false;
            else
            {
                //
                // Check for references to entity properties. If none are found, the expression can be evaluated right away.
                //
                HashSet<PropertyInfo> eProps = new HashSet<PropertyInfo>();
                FindEntityProperties(condition, predicateParameter, eProps);
                if (eProps.Count == 0)
                {
                    object o = Expression.Lambda(condition).Compile().DynamicInvoke();
                    if (o is bool)
                        return _factory.BooleanPatch((bool)o);
                    else
                        return /* PARSE ERROR */ this.NonBoolConstantValueInPredicate(ppS, ppE);
                }
                else
                    return /* PARSE ERROR */ this.UnsupportedQueryExpression(ppS, ppE);
            }

            //
            // Find the side of the condition that refers to an entity property (lhs).
            //
            MemberExpression lhs = (MemberExpression)(correctOrder ? left : right);
            PropertyInfo entityProperty = (PropertyInfo)lhs.Member;

            //
            // Lookup field reference?
            //
            if (!FindLookups(lhs, ref lookup))
                return /* PARSE ERROR */ this.InvalidEntityReference(lhs.Member.Name, ppS, ppE);

            //
            // Ensure that the value side (rhs) of the condition is lambda parameter free.
            //
            Expression rhs = (correctOrder ? right : left);
            bool lambdaFree = EnsureLambdaFree(rhs, predicateParameter, (correctOrder ? ppD : ppS), (correctOrder ? ppE : ppT));

            //
            // Find DateTime values, possibly special ones including Today and Now.
            //
            XmlElement dateValue = null;
            if (lhs.Type == typeof(DateTime) || lhs.Type == typeof(DateTime?))
                dateValue = GetDateValue(rhs);

            object value = null;
            XmlElement c;

            if (dateValue == null && lambdaFree)
            {
                //
                // Get the rhs value by dynamic execution.
                //
                value = Expression.Lambda(rhs).Compile().DynamicInvoke();

                //
                // Any Nullable.HasValue calls detected? Convert to IsNull or IsNotNull elements based on the rhs value, the context positivity and the node type.
                //
                if ((leftIsNullableHasValue.HasValue && leftIsNullableHasValue.Value) || (rightIsNullableHasValue.HasValue && rightIsNullableHasValue.Value))
                {
                    bool isEquality = condition.NodeType == ExpressionType.Equal;
                    bool checkValue = (bool)value;
                    bool isNull = !(checkValue ^ isEquality) ^ isPositive;

                    c = isNull ? _factory.IsNull() : _factory.IsNotNull();
                    c.AppendChild(GetFieldRef(entityProperty));

                    return c;
                }
            }

            //
            // Variable that holds the CAML equivalent of the condition.
            //
            string camlQueryElement = GetCamlQueryElementFor(condition.NodeType, isPositive, correctOrder);
            if (camlQueryElement == null)
                return /* PARSE ERROR */ this.UnsupportedBinary(condition.NodeType.ToString(), ppS, ppE);

            //
            // Special treatment for UrlValues.
            //
            Url urlVal = value as Url;
            if (urlVal != null)
                value = urlVal.Address;

            //
            // Special treatment for detected date values.
            //
            if (dateValue != null)
            {
                c = _factory.CreateElement(camlQueryElement);
                c.AppendChild(dateValue);
            }
            //
            // If the calculated value is null, we'll use a IsNull or IsNotNull (if isPositive == false) element for the condition in CAML.
            //
            else if (value == null)
            {
                //
                // lhs == null  <=(!isPositive)=>  lhs != null
                //   IsNull                         IsNotNull
                //
                if (condition.NodeType == ExpressionType.Equal)
                    c = isPositive ? _factory.IsNull() : _factory.IsNotNull();
                //
                // lhs != null  <=(!isPositive)=>  lhs == null
                //  IsNotNull                        IsNull
                //
                else if (condition.NodeType == ExpressionType.NotEqual)
                    c = isPositive ? _factory.IsNotNull() : _factory.IsNull();
                else
                    return /* PARSE ERROR */ this.InvalidNullValuedCondition(ppS, ppE);
            }
            //
            // Type-specific processing required in translating the query to CAML.
            //
            else
            {
                Type enumCheck = entityProperty.PropertyType;
                if (enumCheck.IsGenericType && enumCheck.GetGenericTypeDefinition() == typeof(Nullable<>))
                    enumCheck = Nullable.GetUnderlyingType(enumCheck);

                //
                // Enums represent Choice or MultiChoice fields.
                //
                if (enumCheck != null && enumCheck.IsSubclassOf(typeof(Enum)))
                {
                    //
                    // Enums might be compiled to numeric values; reconstruct the enum back if needed.
                    //
                    if (!(value is Enum))
                    {
                        if (value is uint)
                            value = Enum.ToObject(enumCheck, (uint)value);
                        else if (value is int)
                            value = Enum.ToObject(enumCheck, (int)value);
                    }

                    //
                    // Check whether the type of the value has been marked as [Flags].
                    //
                    FlagsAttribute[] fa = (FlagsAttribute[])value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false);

                    //
                    // Choice type case.
                    //
                    if (fa == null || fa.Length == 0)
                    {
                        c = _factory.CreateElement(camlQueryElement);
                        bool isLookup; //can be ignored
                        c.AppendChild(GetValue(value, Helpers.GetFieldAttribute(entityProperty), out isLookup));
                    }
                    //
                    // MultiChoice type case.
                    //
                    else
                    {
                        //
                        // Check for each value of the MultiChoice enum definition whether or not it is set for the given value.
                        //
                        Queue<XmlElement> values = new Queue<XmlElement>();
                        uint enumValue = (uint)value;
                        Type enumType = value.GetType();
                        FieldAttribute f = Helpers.GetFieldAttribute(entityProperty);

                        bool isLookup; //can be ignored
                        foreach (uint o in Enum.GetValues(enumType))
                            if ((enumValue & o) == o)
                                values.Enqueue(GetValue(Enum.ToObject(enumType, o), f, out isLookup));

                        //
                        // If no flags values have been set, we're faced with an invalid value.
                        //
                        if (values.Count == 0)
                            return /* PARSE ERROR */ this.UnrecognizedEnumValue(ppS, ppE);
                        else
                        {
                            //
                            // For MultiChoice field queries we need to construct a CAML condition tree consisting of conjunctions.
                            //
                            // E.g. translation of e.X == MC.A | MC.B | MC.C (pseudo-CAML):
                            //    <And>
                            //       <Eq><FieldRef Name="X" /><Value>A</Value></Eq>
                            //       <And>
                            //          <Eq><FieldRef Name="X" /><Value>B</Value></Eq>
                            //          <Eq><FieldRef Name="X" /><Value>C</Value></Eq>
                            //       </And>
                            //    </And>
                            //
                            // NOTE: This translation causes a semantic mismatch between SharePoint and C#. A condition like "e.X == MC.A | MC.B | MC.C"
                            //       has an absolute equality characteristic in C# while CAML has a more relaxed evaluation where e.X == MC.A means that
                            //       option A should be set on field X, while it doesn't say anything about possible other values being present.
                            //       In LINQ to SharePoint, e.X == MC.A | MC.B | MC.C means that choices A, B and C should be set, but not necessarily
                            //       exclusively; that is, other choices may be set and the MC.A | MC.B | MC.C represents a subset of the actual value.
                            //
                            c = null;
                            while (values.Count > 0)
                                c = AppendMultiChoiceCondition(camlQueryElement, entityProperty, values.Dequeue(), c);
                        }

                        return c;
                    }
                }
                //
                // Other fields can be processed in a generic fashion.
                //
                else
                {
                    c = _factory.CreateElement(camlQueryElement);
                    bool isLookup;
                    c.AppendChild(GetValue(value, Helpers.GetFieldAttribute(entityProperty), out isLookup));

                    //
                    // Lookup fields should use the LookupId attribute.
                    //
                    XmlElement fieldRef = GetFieldRef(entityProperty);
                    if (isLookup)
                        fieldRef.Attributes.Append(_factory.LookupAttribute());
                    c.AppendChild(fieldRef);

                    return c;
                }
            }

            //
            // Append the FieldRef element to the condition element and return the condition.
            //
            c.AppendChild(GetFieldRef(entityProperty));
            return c;
        }

        private static Expression TrimConvert(Expression expression, ref int ppS, ref int ppE)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;

                ppS += (expression.NodeType == ExpressionType.Convert ? "Convert(".Length : "ConvertChecked(".Length);
                ppE -= 1;
            }
            return expression;
        }

        private static string GetCamlQueryElementFor(ExpressionType nodeType, bool isPositive, bool correctOrder)
        {
            //
            // Check the type of the node for CAML-supported operations.
            //
            switch (nodeType)
            {
                //
                // Equality is commutative, correctOrder doesn't matter.
                //
                case ExpressionType.Equal:
                    return isPositive ? "Eq" : "Neq";
                //
                // Non-equality is commutative, correctOrder doesn't matter.
                //
                case ExpressionType.NotEqual:
                    return isPositive ? "Neq" : "Eq";
                //
                // Less than <=(!correctOrder)=> Greater than
                //
                case ExpressionType.LessThan:
                    if (!correctOrder)
                        //
                        // !(a > b) == a <= b
                        //
                        return isPositive ? "Gt" : "Leq";
                    else
                        //
                        // !(a < b) == a >= b
                        //
                        return isPositive ? "Lt" : "Geq";
                //
                // Less than or equal <=(!correctOrder)=> Greater than or equal
                //
                case ExpressionType.LessThanOrEqual:
                    if (!correctOrder)
                        //
                        // !(a >= b) == a < b
                        //
                        return isPositive ? "Geq" : "Lt";
                    else
                        //
                        // !(a <= b) == a > b
                        //
                        return isPositive ? "Leq" : "Gt";
                //
                // Greater than <=(!correctOrder)=> Less than
                //
                case ExpressionType.GreaterThan:
                    if (!correctOrder)
                        //
                        // !(a < b) == a >= b
                        //
                        return isPositive ? "Lt" : "Geq";
                    else
                        //
                        // !(a > b) == a <= b
                        //
                        return isPositive ? "Gt" : "Leq";
                //
                // Greater than or equal <=(!correctOrder)=> Less than or equal
                //
                case ExpressionType.GreaterThanOrEqual:
                    if (!correctOrder)
                        //
                        // !(a <= b) == a > b
                        //
                        return isPositive ? "Leq" : "Gt";
                    else
                        //
                        // !(a >= b) == a < b
                        //
                        return isPositive ? "Geq" : "Lt";
                //
                // Currently no support for other binary operations (if any).
                //
                default:
                    return null;
            }
        }

        private XmlElement CheckForUrlValueUrl(ref Expression expression, out bool found, int ppS)
        {
            found = false;

            MemberExpression m = expression as MemberExpression;
            if (m != null && m.Member.DeclaringType == typeof(Url) && IsEntityPropertyReference(m.Expression))
            {
                if (m.Member.Name != "Url")
                {
                    int ppS1 = ppS + m.Expression.ToString().Length + 1;
                    return /* PARSE ERROR */ this.NonAddressPropertyCallOnUrlValue(ppS1, ppS1 + m.Member.Name.Length - 1);
                }

                expression = m.Expression;
                found = true;
            }
            return null;
        }

        /// <summary>
        /// Retrieves the XML-representation for the DateTime value represented in the specified expression.
        /// </summary>
        /// <param name="dateValue">Expression containing a DateTime value to be converted to XML.</param>
        /// <returns>XML representation for the specified DateTime value expression.</returns>
        private XmlElement GetDateValue(Expression dateValue)
        {
            /*
             * KNOWN ISSUES: see work item 2032
             */

            bool isNow = false;
            bool isToday = false;

            MemberExpression me = dateValue as MemberExpression;
            if (me != null && me.Type == typeof(DateTime))
            {
                Type dt = me.Member.DeclaringType;
                if (dt == typeof(DateTime) || dt == typeof(CamlMethods))
                {
                    isNow = me.Member.Name == "Now";
                    isToday = me.Member.Name == "Today";
                }
            }

            //
            // Value element.
            //
            XmlElement valueElement = _factory.Value("DateTime");

            //
            // [DateTime|CamlElements].Now and [DateTime|CamlElements].Today calls require special treatment.
            //
            if (isNow || isToday)
                valueElement.AppendChild(isNow ? _factory.Now() : _factory.Today());
            else
            {
                object value = Expression.Lambda(dateValue).Compile().DynamicInvoke();
                valueElement.InnerText = Helpers.CreateISO8601DateTimeFromSystemDateTime((DateTime)value);
            }

            return valueElement;
        }

        /// <summary>
        /// Helper method to support MultiChoice field conditions by building a tree of And CAML elements.
        /// </summary>
        /// <param name="condition">Condition node textual representation, e.g. Eq.</param>
        /// <param name="field">Entity property to construct the MultiChoice condition node for.</param>
        /// <param name="value">Value for the MultiChoice condition.</param>
        /// <param name="parent">Current tree of MultiChoice conditions to add the new condition node to. Should be null to start creating a condition tree.</param>
        /// <returns>Output XML element representing the MultiChoice field condition in CAML syntax.</returns>
        /// <example>
        /// If parent == null:
        /// <![CDATA[
        /// <condition>
        ///    value
        ///    <FieldRef Name="field" />
        /// </condition>
        /// ]]>
        /// 
        /// If parent != null:
        /// <![CDATA[
        /// <And>
        ///    <condition>
        ///       value
        ///       <FieldRef Name="field" />
        ///    </condition>
        ///    parent
        /// </And>
        /// ]]>
        /// </example>
        private XmlElement AppendMultiChoiceCondition(string condition, PropertyInfo field, XmlElement value, XmlElement parent)
        {
            //
            // Create condition element with the child tree and the FieldRef element.
            //
            XmlElement cond = _factory.CreateElement(condition);
            cond.AppendChild(value);
            cond.AppendChild(GetFieldRef(field));

            //
            // If no parent is present yet, we'll just return the condition element.
            //
            if (parent == null)
                return cond;
            //
            // If we're deeper in the tree, we'll take the current parent and lift it to a new And element together with the newly created condition.
            //
            else
                return _factory.And(cond, parent);
        }

        /// <summary>
        /// Get a CAML Value element that represents the given value for the given field.
        /// </summary>
        /// <param name="value">Field value to get a Value element for.</param>
        /// <param name="field">Field to get a Value element for.</param>
        /// <param name="lookup">Indicates that a lookup reference was found, e.g. in a comparison with an entity object.</param>
        /// <returns>CAML Value element representing the given value for the given field.</returns>
        private XmlElement GetValue(object value, FieldAttribute field, out bool lookup)
        {
            //
            // Create Value element and set the Type attribute.
            //
            XmlElement valueElement = _factory.Value(field.FieldType.ToString());

            //
            // Default no lookup.
            //
            lookup = false;

            //
            // Null-check
            //
            if (value == null)
                return valueElement;

            //
            // DateTime fields should be converted to ISO 8601 date/time representation in SharePoint.
            //
            if (value is DateTime)
                valueElement.InnerText = Helpers.CreateISO8601DateTimeFromSystemDateTime((DateTime)value);
            //
            // Boolean fields in SharePoint are represented as 1 or 0.
            //
            else if (value is bool)
                valueElement.InnerText = ((bool)value ? "1" : "0");
            //
            // For enums, we should get the name of the field value which can be mapped using a ChoiceAttribute.
            // Only single-valued enum objects should occur here (i.e. no flag combinations).
            //
            else if (value is Enum)
            {
                string choice = Enum.GetName(value.GetType(), value);
                valueElement.InnerText = GetChoiceName(value.GetType(), choice);
            }
            //
            // Other cases.
            //
            else
            {
                ListAttribute la = Helpers.GetListAttribute(value.GetType(), false);

                //
                // Support for lookup fields (v0.2.0.0).
                //
                if (la != null)
                {
                    lookup = true;

                    //
                    // Find primary key field and property.
                    //
                    FieldAttribute pkField;
                    PropertyInfo pkProp;
                    Helpers.FindPrimaryKey(value.GetType(), out pkField, out pkProp, true);

                    //
                    // Get the value and assign it to the Value element.
                    //
                    object val = pkProp.GetValue(value, null);
                    valueElement.InnerText = val.ToString();
                }
                //
                // Other types will be converted to a string.
                // TODO: I18n issues might occur for numeric values; this should be checked.
                //
                else
                    valueElement.InnerText = value.ToString();
            }

            return valueElement;
        }

        /// <summary>
        /// Gets the SharePoint CHOICE value for a given enum field.
        /// </summary>
        /// <param name="enumType">Enum type to map the specified field for.</param>
        /// <param name="field">Enum field name to map to a SharePoint CHOICE value.</param>
        /// <returns>SharePoint CHOICE value corresponding with the given enum field.</returns>
        private static string GetChoiceName(Type enumType, string field)
        {
            FieldInfo fi = enumType.GetField(field);

            //
            // Check whether a ChoiceAttribute is applied on the field of the enum. If so, return the mapped name, otherwise take the field name itself.
            //
            ChoiceAttribute[] ca = fi.GetCustomAttributes(typeof(ChoiceAttribute), false) as ChoiceAttribute[];
            if (ca != null && ca.Length != 0 && ca[0] != null)
                return ca[0].Choice;
            else
                return field;
        }

        #endregion

        #region Query ordering expression parsing (OrderBy*, ThenBy*)

        /// <summary>
        /// Parses a query ordering expression, resulting in a CAML OrderBy element (<see crf="_order"/>).
        /// </summary>
        /// <param name="ordering">Lambda expression of the query ordering key selector to parse.</param>
        /// <param name="orderBy">Indicates whether or not the ordering is a top-level ordering.</param>
        /// <param name="descending">Indicates whether or not the ordering should be descending.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <remarks>Multiple ordering expressions per query are supported.</remarks>
        private void ParseOrdering(LambdaExpression ordering, bool orderBy, bool descending, int ppS, int ppE)
        {
            //
            // If this is a top-level ordering or no ordering expression has been encountered before, construct the OrderBy CAML element.
            //
            if (_results.Order == null || orderBy)
                _results.Order = _factory.OrderBy();

            int ppS2 = ppS + ordering.Parameters[0].Name.Length + " => ".Length;
            int ppE2 = ppE;

            //
            // Trim ToString() calls for ordering expressions on textual fields. Allows more flexibility.
            //
            Expression orderExpression = DropToString(ordering.Body, ref ppE2);

            //
            // Convert the ordering expression as a MemberExpression.
            //
            MemberExpression me = orderExpression as MemberExpression;

            bool? isHasValue;
            me = (MemberExpression)CheckForNullableType(me, out isHasValue);

            //
            // Make sure the expression is a MemberExpression and points to a property on the entity type.
            //
            PropertyInfo property;
            if (me != null && me.Member.DeclaringType == _results.EntityType && (property = (me.Member as PropertyInfo)) != null
                //
                // If nullable, it shouldn't be a call to HasValue.
                //
                && (!isHasValue.HasValue || (isHasValue.HasValue && !isHasValue.Value))
                )
            {
                //
                // Check presence of field attribute.
                //
                FieldAttribute fa = Helpers.GetFieldAttribute(property);
                if (fa == null)
                    _results.Order.AppendChild(this.MissingFieldMappingAttribute(property.Name)); /* PARSE ERROR */
                else if (fa.FieldType == FieldType.Counter || fa.FieldType == FieldType.LookupMulti || fa.FieldType == FieldType.MultiChoice)
                    _results.Order.AppendChild(this.UnsupportedOrdering(ppS, ppE)); /* PARSE ERROR */

                //
                // Obtain a FieldRef element for the property on the entity type being referred to.
                //
                XmlElement fieldRef = GetFieldRef(property);

                //
                // For descending orderings, an Ascending="FALSE" attribute should be added to the FieldRef elements.
                //
                if (descending)
                    fieldRef.Attributes.Append(_factory.DescendingAttribute());

                //
                // Append the FieldRef element to the OrderBy ordering clause.
                //
                _results.Order.AppendChild(fieldRef);
            }
            else
                _results.Order.AppendChild(this.UnsupportedOrdering(ppS, ppE)); /* PARSE ERROR */
        }

        #endregion

        #region Query projection expression parsing (Select)

        /// <summary>
        /// Parses a query projection expression, resulting in a CAML ViewFields element (<see crf="_projection"/>).
        /// </summary>
        /// <param name="projection">Lambda expression of the query projection to parse.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <remarks>Only one projection expression can be parsed per query.</remarks>
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
            // If no projection has been encountered before, construct the ViewFields CAML element.
            // There should only be one projection per query.
            //
            if (_results.Projection == null)
                _results.Projection = _factory.ViewFields();
            else
            {
                this.SecondProjectionExpression(ppS, ppE); /* PARSE ERROR */
                return;
            }

            //
            // Compile the projection for execution during query result fetching.
            //
            _results.Project = projection.Compile();

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
                    _results.Projection.AppendChild(this.AfterGrouping("Select", ppS, ppE)); /* PARSE ERROR */
                    return;
                }
            }
            //
            // Regular projections.
            //
            else
            {
                //
                // Create the set with entity properties used in projection and populate it.
                //
                _results.ProjectionProperties = new HashSet<PropertyInfo>();
                FindEntityProperties(projection.Body, projection.Parameters[0], _results.ProjectionProperties);
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

        #endregion

        #region Query result restriction (Take)

        /// <summary>
        /// Sets the restriction on the number of results returned by the query, with semantics like "TOP" in SQL.
        /// </summary>
        /// <param name="limit">Restriction on the number of results returned by the query.</param>
        /// <remarks>Multiple restrictions per query are supported and will result in the mimimum of all restrictions to be effective.</remarks>
        private void SetResultRestriction(int limit)
        {
            //
            // If no top value has been set yet, take the specified value; otherwise, take the minimum of the current value and the specified value.
            //
            _results.Top = (_results.Top == null ? limit : Math.Min(_results.Top.Value, limit));
        }

        #endregion

        #region Query grouping parsing (GroupBy)

        /// <summary>
        /// Parses a query grouping expression, resulting in a CAML GroupBy element.
        /// </summary>
        /// <param name="keySelector">Lambda expression of the grouping key selection.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <remarks>Only one grouping expression can be parsed per query.</remarks>
        private void ParseGrouping(LambdaExpression keySelector, int ppS, int ppE)
        {
            //
            // Only one grouping expression is supported.
            //
            if (_results.Grouping != null)
                this.MultipleGroupings(ppS, ppE); /* PARSE ERROR */

            //
            // Construct the GroupBy CAML element.
            //
            _results.Grouping = _factory.GroupBy();

            int ppS2 = ppS + keySelector.Parameters[0].Name.Length + " => ".Length;
            int ppE2 = ppE;

            //
            // Trim ToString() calls for grouping expressions on textual fields. Allows more flexibility.
            //
            Expression groupExpression = DropToString(keySelector.Body, ref ppE2);

            //
            // Convert the grouping expression as a MemberExpression.
            //
            MemberExpression me = groupExpression as MemberExpression;

            bool? isHasValue;
            me = (MemberExpression)CheckForNullableType(me, out isHasValue);

            //
            // Make sure the expression is a MemberExpression and points to a property on the entity type.
            //
            PropertyInfo property;
            if (me != null && me.Member.DeclaringType == _results.EntityType && (property = (me.Member as PropertyInfo)) != null
                //
                // If nullable, it shouldn't be a call to HasValue.
                //
                && (!isHasValue.HasValue || (isHasValue.HasValue && !isHasValue.Value))
                )
            {
                //
                // Check presence of field attribute.
                //
                FieldAttribute fa = Helpers.GetFieldAttribute(property);
                if (fa == null)
                    _results.Grouping.AppendChild(this.MissingFieldMappingAttribute(property.Name)); /* PARSE ERROR */
                else if (fa.FieldType == FieldType.Counter || fa.FieldType == FieldType.LookupMulti || fa.FieldType == FieldType.MultiChoice)
                    _results.Grouping.AppendChild(this.UnsupportedGrouping(ppS, ppE)); /* PARSE ERROR */

                //
                // Obtain a FieldRef element for the property on the entity type being referred to.
                //
                XmlElement fieldRef = GetFieldRef(property);

                //
                // Append the FieldRef element to the GroupBy ordering clause.
                //
                _results.Grouping.AppendChild(fieldRef);

                //
                // Pre-compile the grouping key selector for use during result yielding.
                //
                _results.Group = keySelector.Compile();

                //
                // Set grouping key type.
                //
                _results.GroupKeyType = property.PropertyType;

                //
                // Keep group field.
                //
                _results.GroupField = property;
            }
            else
                _results.Grouping.AppendChild(this.UnsupportedGrouping(ppS, ppE)); /* PARSE ERROR */
        }

        #endregion

        #region Helpers

        #region Helper methods to work with conditions and entity property expressions.

        /// <summary>
        /// Checks whether an expression is a reference to an entity property or not.
        /// </summary>
        /// <param name="e">Expression to be checked.</param>
        /// <returns>True if the expression refers to an entity property; otherwise false.</returns>
        private static bool IsEntityPropertyReference(Expression e)
        {
            MemberExpression me = e as MemberExpression;
            PropertyInfo prop;

            if (me != null && (prop = me.Member as PropertyInfo) != null)
            {
                FieldAttribute field = Helpers.GetFieldAttribute(prop);
                if (field != null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to drop excessive tail ToString calls on string instances for a given expression.
        /// </summary>
        /// <param name="e">Expression to drop excessive tail ToString calls for.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>Expression without tail ToString calls on string instances.</returns>
        private static Expression DropToString(Expression e, ref int ppE)
        {
            while (true)
            {
                //
                // Only look for ToString calls on the root level, as MethodCallExpression objects.
                //
                MethodCallExpression mc = e as MethodCallExpression;

                //
                // Only parameterless ToString() calls on strings should be trimmed off.
                //
                if (mc != null && mc.Object != null && mc.Object.Type == typeof(string) && mc.Method.Name == "ToString" && mc.Method.GetParameters().Length == 0)
                {
                    e = mc.Object;
                    ppE -= ".ToString()".Length;
                }
                else
                    break;
            }
            return e;
        }

        /// <summary>
        /// Recursive method to ensure that a given expression doesn't contain a dependency on a given lambda parameter.
        /// </summary>
        /// <param name="e">Expression to validate.</param>
        /// <param name="parameter">Forbidden lambda parameter to look for.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>True if the given expression is lambda free, false otherwise.</returns>
        /// <exception cref="NotSupportedException">Occurs when the specified lambda parameter is found in the expression.</exception>
        private bool EnsureLambdaFree(Expression e, ParameterExpression parameter, int ppS, int ppE)
        {
            //
            // Some recursive calls can cause a check on a null-valued expression node.
            //
            if (e == null)
                return true;

            //
            // TODO: ppS and ppE should be narrowed in recursive calls.
            //

            #region Local variables

            //
            // These variables are kept to avoid needless casting which would occur in constructs like this:
            //   if (e is Abc)
            //   {
            //      Abc abc = (Abc)e;
            //      ...
            //   }
            //

            MemberExpression me;
            ParameterExpression pe;
            BinaryExpression be;
            UnaryExpression ue;
            ConditionalExpression ce;
            InvocationExpression ie;
            LambdaExpression le;
            ListInitExpression lie;
            MemberInitExpression mie;
            MethodCallExpression mce;
            NewExpression ne;
            NewArrayExpression nae;
            TypeBinaryExpression tbe;

            #endregion

            //
            // Base case - member expression is candidate for entity property reference.
            //
            if ((me = e as MemberExpression) != null)
            {
                return EnsureLambdaFree(me.Expression, parameter, ppS, ppE);
            }
            //
            // Constants are lambda-free by definition.
            //
            else if (e is ConstantExpression)
            {
                return true;
            }
            //
            // Base case - reference to lambda expression parameter is candidate for a reference to the whole entity type.
            //
            else if ((pe = e as ParameterExpression) != null)
            {
                //
                // Check that the parameter matches the projection lambda expression's parameter.
                //
                if (pe == parameter)
                {
                    this.MultipleEntityReferencesInCondition(ppS, ppE); /* PARSE ERROR */
                    return false;
                }
                else
                    return true;
            }
            //
            // b.Method(b.Left, b.Right)
            //
            else if ((be = e as BinaryExpression) != null)
            {
                return EnsureLambdaFree(be.Left, parameter, ppS, ppE)
                    && EnsureLambdaFree(be.Right, parameter, ppS, ppE);
            }
            //
            // u.Method(u.Operand)
            //
            else if ((ue = e as UnaryExpression) != null)
            {
                return EnsureLambdaFree(ue.Operand, parameter, ppS, ppE);
            }
            //
            // (c.Test ? c.IfTrue : c.IfFalse)
            //
            else if ((ce = e as ConditionalExpression) != null)
            {
                return EnsureLambdaFree(ce.IfFalse, parameter, ppS, ppE)
                    && EnsureLambdaFree(ce.IfTrue, parameter, ppS, ppE)
                    && EnsureLambdaFree(ce.Test, parameter, ppS, ppE);
            }
            //
            // i.Expression(i.Arguments[0], ..., i.Arguments[i.Argument.Count - 1])
            //
            else if ((ie = e as InvocationExpression) != null)
            {
                bool res = EnsureLambdaFree(ie.Expression, parameter, ppS, ppE);
                foreach (Expression ex in ie.Arguments)
                    res = res && EnsureLambdaFree(ex, parameter, ppS, ppE);
                return res;
            }
            //
            // (l.Parameters[0], ..., l.Parameters[l.Parameters.Count - 1]) => l.Body
            //
            else if ((le = e as LambdaExpression) != null)
            {
                bool res = EnsureLambdaFree(le.Body, parameter, ppS, ppE);
                foreach (Expression ex in le.Parameters)
                    res = res && EnsureLambdaFree(ex, parameter, ppS, ppE);
                return res;
            }
            //
            // li.NewExpression { li.Expressions[0], ..., li.Expressions[li.Expressions.Count - 1] }
            //
            else if ((lie = e as ListInitExpression) != null)
            {
                bool res = EnsureLambdaFree(lie.NewExpression, parameter, ppS, ppE);
                foreach (ElementInit init in lie.Initializers)
                    foreach (Expression ex in init.Arguments)
                        res = res && EnsureLambdaFree(ex, parameter, ppS, ppE);
                return res;
            }
            //
            // Member initialization expression requires recursive processing of MemberBinding objects.
            //
            else if ((mie = e as MemberInitExpression) != null)
            {
                bool res = EnsureLambdaFree(mie.NewExpression, parameter, ppS, ppE);

                //
                // Maintain a queue to mimick recursion on MemberBinding objects. Enqueue the original bindings.
                //
                Queue<MemberBinding> memberBindings = new Queue<MemberBinding>();
                foreach (MemberBinding b in mie.Bindings)
                    memberBindings.Enqueue(b);

                //
                // Process all bindings.
                //
                while (memberBindings.Count > 0)
                {
                    MemberBinding b = memberBindings.Dequeue();

                    MemberAssignment ma = b as MemberAssignment;
                    MemberListBinding mlb = b as MemberListBinding;
                    MemberMemberBinding mmb = b as MemberMemberBinding;

                    if (ma != null)
                        res = res && EnsureLambdaFree(ma.Expression, parameter, ppS, ppE);
                    else if (mlb != null)
                        foreach (ElementInit init in mlb.Initializers)
                            foreach (Expression ex in init.Arguments)
                                res = res && EnsureLambdaFree(ex, parameter, ppS, ppE);
                    //
                    // Recursion if a MemberBinding contains other bindings.
                    //
                    else if (mmb != null)
                        foreach (MemberBinding mb in mmb.Bindings)
                            memberBindings.Enqueue(mb);
                }
                return res;
            }
            //
            // mc.Object->mc.Method(mc.Arguments[0], ..., mc.Arguments[mc.Arguments.Count - 1])
            //
            else if ((mce = e as MethodCallExpression) != null)
            {
                bool res = true;

                if (mce.Object != null)
                    res = res && EnsureLambdaFree(mce.Object, parameter, ppS, ppE);

                foreach (Expression ex in mce.Arguments)
                    res = res && EnsureLambdaFree(ex, parameter, ppS, ppE);
                return res;
            }
            //
            // new n.Constructor(n.Arguments[0], ..., n.Arguments[n.Arguments.Count - 1])
            //
            else if ((ne = e as NewExpression) != null)
            {
                bool res = true;
                foreach (Expression ex in ne.Arguments)
                    res = res && EnsureLambdaFree(ex, parameter, ppS, ppE);
                return res;
            }
            //
            // { na.Expressions[0], ..., na.Expressions[n.Expressions.Count - 1] }
            //
            else if ((nae = e as NewArrayExpression) != null)
            {
                bool res = true;
                foreach (Expression ex in nae.Expressions)
                    res = res && EnsureLambdaFree(ex, parameter, ppS, ppE);
                return res;
            }
            //
            // tb.Expression is tb.Type
            //
            else if ((tbe = e as TypeBinaryExpression) != null)
            {
                return EnsureLambdaFree(tbe.Expression, parameter, ppS, ppE);
            }
            //
            // Unknown construct (CHECK).
            //
            else
            {
                ParseErrors.FatalError(); /* PARSE ERROR */
                return false;
            }
        }

        #endregion

        #region Helper methods for FieldRef element creation

        /// <summary>
        /// Gets a CAML FieldRef element for the specified entity property.
        /// </summary>
        /// <param name="property">Entity property to get the FieldRef element for.</param>
        /// <returns>FieldRef element for the specified entity property.</returns>
        private XmlElement GetFieldRef(PropertyInfo property)
        {
            //
            // Get mapping attribute and make sure it has been set.
            //
            FieldAttribute fld = Helpers.GetFieldAttribute(property);
            if (fld == null)
                return /* PARSE ERROR */ this.MissingFieldMappingAttribute(property.Name);

            return _factory.FieldRef(fld.Field);
        }

        #endregion

        #region Helpers methods to work with entity properties

        /// <summary>
        /// Recursive method to find all references to entity properties in a projection expression.
        /// </summary>
        /// <param name="e">Expression to search for references to entity properties.</param>
        /// <param name="parameter">Lambda parameter used by the expression. Used to detect references to the entity type itself.</param>
        /// <param name="output">Output set to store results in.</param>
        private void FindEntityProperties(Expression e, ParameterExpression parameter, HashSet<PropertyInfo> output)
        {
            #region Local variables

            //
            // These variables are kept to avoid needless casting which would occur in constructs like this:
            //   if (e is Abc)
            //   {
            //      Abc abc = (Abc)e;
            //      ...
            //   }
            //

            MemberExpression me;
            ParameterExpression pe;
            BinaryExpression be;
            UnaryExpression ue;
            ConditionalExpression ce;
            InvocationExpression ie;
            LambdaExpression le;
            ListInitExpression lie;
            MemberInitExpression mie;
            MethodCallExpression mce;
            NewExpression ne;
            NewArrayExpression nae;
            TypeBinaryExpression tbe;

            #endregion

            //
            // Base case - member expression is candidate for entity property reference.
            //
            if ((me = e as MemberExpression) != null)
            {
                bool? isHasValue;
                me = (MemberExpression)CheckForNullableType(me, out isHasValue);

                //
                // Check that the member expression refers to a property on the entity type of the original query.
                //
                PropertyInfo prop = me.Member as PropertyInfo;
                if (prop != null && me.Member.DeclaringType == _results.EntityType && Helpers.GetFieldAttribute(prop) != null)
                    output.Add(prop);

                //
                // Recursion.
                //
                if (me.Expression != parameter)
                    FindEntityProperties(me.Expression, parameter, output);
            }
            //
            // Base case - reference to lambda expression parameter is candidate for a reference to the whole entity type.
            //
            else if ((pe = e as ParameterExpression) != null)
            {
                //
                // Check that the parameter matches the projection lambda expression's parameter.
                //
                if (pe == parameter)
                    foreach (PropertyInfo prop in _results.EntityType.GetProperties())
                        if (Helpers.GetFieldAttribute(prop) != null)
                            output.Add(prop);
            }
            //
            // b.Method(b.Left, b.Right)
            //
            else if ((be = e as BinaryExpression) != null)
            {
                FindEntityProperties(be.Left, parameter, output);
                FindEntityProperties(be.Right, parameter, output);
            }
            //
            // u.Method(u.Operand)
            //
            else if ((ue = e as UnaryExpression) != null)
            {
                FindEntityProperties(ue.Operand, parameter, output);
            }
            //
            // (c.Test ? c.IfTrue : c.IfFalse)
            //
            else if ((ce = e as ConditionalExpression) != null)
            {
                FindEntityProperties(ce.IfFalse, parameter, output);
                FindEntityProperties(ce.IfTrue, parameter, output);
                FindEntityProperties(ce.Test, parameter, output);
            }
            //
            // i.Expression(i.Arguments[0], ..., i.Arguments[i.Argument.Count - 1])
            //
            else if ((ie = e as InvocationExpression) != null)
            {
                FindEntityProperties(ie.Expression, parameter, output);
                foreach (Expression ex in ie.Arguments)
                    FindEntityProperties(ex, parameter, output);
            }
            //
            // (l.Parameters[0], ..., l.Parameters[l.Parameters.Count - 1]) => l.Body
            //
            else if ((le = e as LambdaExpression) != null)
            {
                FindEntityProperties(le.Body, parameter, output);
                foreach (Expression ex in le.Parameters)
                    FindEntityProperties(ex, parameter, output);
            }
            //
            // li.NewExpression { li.Expressions[0], ..., li.Expressions[li.Expressions.Count - 1] }
            //
            else if ((lie = e as ListInitExpression) != null)
            {
                FindEntityProperties(lie.NewExpression, parameter, output);
                foreach (ElementInit init in lie.Initializers)
                    foreach (Expression ex in init.Arguments)
                        FindEntityProperties(ex, parameter, output);
            }
            //
            // Member initialization expression requires recursive processing of MemberBinding objects.
            //
            else if ((mie = e as MemberInitExpression) != null)
            {
                FindEntityProperties(mie.NewExpression, parameter, output);

                //
                // Maintain a queue to mimick recursion on MemberBinding objects. Enqueue the original bindings.
                //
                Queue<MemberBinding> memberBindings = new Queue<MemberBinding>();
                foreach (MemberBinding b in mie.Bindings)
                    memberBindings.Enqueue(b);

                MemberAssignment ma;
                MemberListBinding mlb;
                MemberMemberBinding mmb;

                //
                // Process all bindings.
                //
                while (memberBindings.Count > 0)
                {
                    MemberBinding b = memberBindings.Dequeue();

                    if ((ma = b as MemberAssignment) != null)
                        FindEntityProperties(ma.Expression, parameter, output);
                    else if ((mlb = b as MemberListBinding) != null)
                        foreach (ElementInit init in mlb.Initializers)
                            foreach (Expression ex in init.Arguments)
                                FindEntityProperties(ex, parameter, output);
                    //
                    // Recursion if a MemberBinding contains other bindings.
                    //
                    else if ((mmb = b as MemberMemberBinding) != null)
                        foreach (MemberBinding mb in mmb.Bindings)
                            memberBindings.Enqueue(mb);
                }
            }
            //
            // mc.Object->mc.Method(mc.Arguments[0], ..., mc.Arguments[mc.Arguments.Count - 1])
            //
            else if ((mce = e as MethodCallExpression) != null)
            {
                if (mce.Object != null)
                    FindEntityProperties(mce.Object, parameter, output);
                foreach (Expression ex in mce.Arguments)
                    FindEntityProperties(ex, parameter, output);
            }
            //
            // new n.Constructor(n.Arguments[0], ..., n.Arguments[n.Arguments.Count - 1])
            //
            else if ((ne = e as NewExpression) != null)
            {
                foreach (Expression ex in ne.Arguments)
                    FindEntityProperties(ex, parameter, output);
            }
            //
            // { na.Expressions[0], ..., na.Expressions[n.Expressions.Count - 1] }
            //
            else if ((nae = e as NewArrayExpression) != null)
            {
                foreach (Expression ex in nae.Expressions)
                    FindEntityProperties(ex, parameter, output);
            }
            else if ((tbe = e as TypeBinaryExpression) != null)
            {
                FindEntityProperties(tbe.Expression, parameter, output);
            }
        }

        #endregion

        #region Query composition guards

        /// <summary>
        /// Guards entry to a parse method. Checks that no projection has been carried out previously.
        /// </summary>
        /// <param name="method">Query operator where the guard call originates from.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>true if guard signals an error; otherwise false</returns>
        private bool GuardProjection(string method, int ppS, int ppE)
        {
            if (_results.Projection != null)
            {
                this.AfterProjection(method, ppS, ppE); /* PARSE ERROR */
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Guards entry to a parse method. Checks that no grouping has been carried out previously.
        /// </summary>
        /// <param name="method">Query operator where the guard call originates from.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>true if guard signals an error; otherwise false</returns>
        private bool GuardGrouping(string method, int ppS, int ppE)
        {
            if (_results.Grouping != null)
            {
                this.AfterGrouping(method, ppS, ppE); /* PARSE ERROR */
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Guards entry to a parse method. Checks that no "take" result restriction has been carried out previously.
        /// </summary>
        /// <param name="method">Query operator where the guard call originates from.</param>
        /// <param name="ppS">Start position for parser error tracking.</param>
        /// <param name="ppE">End position for parser error tracking.</param>
        /// <returns>true if guard signals an error; otherwise false</returns>
        private bool GuardTake(string method, int ppS, int ppE)
        {
            if (_results.Top != null)
            {
                this.AfterTake(method, ppS, ppE); /* PARSE ERROR */
                return true;
            }
            else
                return false;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Helper class to hold information about a SharePoint query.
    /// </summary>
    public sealed class QueryInfo
    {
        /// <summary>
        /// Entity type of the source list items.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// SharePoint data context that will be used to execute the query.
        /// </summary>
        public SharePointDataContext Context { get; set; }

        /// <summary>
        /// Set of PropertyInfo objects for all the fields used in the projection portion of the query. Used to build the projection without duplicates.
        /// </summary>
        public HashSet<PropertyInfo> ProjectionProperties { get; set; }

        /// <summary>
        /// Delegate for the projection logic, generated by compiling the projection's lambda expression (e.g. u => new { u.Name }). Takes an object of the original entity type used in the query.
        /// </summary>
        public Delegate Project { get; set; }

        /// <summary>
        /// Fields required to perform the projection clause of the query, based on the CAML query format's ViewFields element. Can be empty in case no projection is done and/or all fields are required in the query result.
        /// </summary>
        public XmlElement Projection { get; set; }

        /// <summary>
        /// Where clause of the query, based on the CAML query format's Where element.
        /// </summary>
        public XmlElement Where { get; set; }

        /// <summary>
        /// Ordering clause of the query, based on the CAML query format's OrderBy element.
        /// </summary>
        public XmlElement Order { get; set; }

        /// <summary>
        /// Delegate for the grouping key selector, generated by compiling the grouping's lambda expression (e.g. u => u.Country). Takes an object of the original entity type used in the query.
        /// </summary>
        public Delegate Group { get; set; }

        /// <summary>
        /// Type of the grouping key.
        /// </summary>
        public Type GroupKeyType { get; set; }

        /// <summary>
        /// Grouping clause of the query, based on the CAML query format's GroupBy element.
        /// </summary>
        public XmlElement Grouping { get; set; }

        /// <summary>
        /// Grouping field of the grouping clause of the query.
        /// </summary>
        public PropertyInfo GroupField { get; set; }

        /// <summary>
        /// Optional number of "top" rows to query for, with the semantics of the TOP construct in SQL. Gathered by parsing Take(n) calls.
        /// </summary>
        public int? Top { get; set; }
    }

    /// <summary>
    /// Helper class to hold information about Lookup field subqueries.
    /// </summary>
    internal class LookupInfo
    {
        /// <summary>
        /// Keeps chain of lookup properties. To follow a chain from the root entity type, start at the end.
        /// </summary>
        /// <example>
        /// order.Product.Supplier becomes { Product::Supplier ,  Order::Product } internally
        /// but should be interpreted as   { Order::Product -> Product::Supplier }
        /// </example>
        private List<PropertyInfo> lookups = new List<PropertyInfo>();

        /// <summary>
        /// Adds a new lookup property to the chain represented by the current LookupInfo instance.
        /// </summary>
        /// <param name="property">Lookup property to add to the chain.</param>
        public void Add(PropertyInfo property)
        {
            lookups.Add(property);
        }

        /// <summary>
        /// Generate the patch for the current lookup chain.
        /// </summary>
        /// <param name="factory">Factory to generate CAML elements from.</param>
        /// <param name="node">Node to be patched using the current lookup chain.</param>
        /// <example>
        /// Assume the following chain of lookups: { Order::Product -> Product::Supplier }
        /// 
        /// This will produce the following patch surrounding node (%NODE%):
        /// %NODE% := 
        /// &lt;Patch Field="Product"&gt;
        ///    &lt;Patch Field="Product"&gt;
        ///       %NODE%
        ///    &lt;/Patch&gt;
        /// &lt;/Patch&gt;
        /// </example>
        public void Patch(CamlFactory factory, ref XmlElement node)
        {
            foreach (PropertyInfo lookup in lookups)
            {
                XmlElement patch = factory.Patch(lookup.Name);
                patch.AppendChild(node);
                node = patch;
            }
        }

        /// <summary>
        /// Coalesces two lookup chains to find the greatest common head.
        /// </summary>
        /// <param name="first">First lookup chain.</param>
        /// <param name="second">Second lookup chain.</param>
        /// <returns>Greatest common head.</returns>
        /// <remarks>Both referenced lookup chains will get their common head trimmed of.</remarks>
        /// <example>
        /// Assume two lookup chains:
        /// first  := { Order::Product -> Product::Supplier }
        /// second := { Order::Product }
        /// 
        /// This will return the following result chain { Order::Product }
        /// and will modify the input chains to:
        /// first  := { Product::Supplier }
        /// second := { }
        /// </example>
        public static LookupInfo Coalesce(ref LookupInfo first, ref LookupInfo second)
        {
            Debug.Assert(first != null && second != null, "One of both inputs to LookupInfo::Coalesce is null.");

            List<PropertyInfo> f = first.lookups;
            int fi = f.Count - 1;
            List<PropertyInfo> s = second.lookups;
            int si = s.Count - 1;

            Stack<PropertyInfo> res = new Stack<PropertyInfo>();
            while (fi >= 0 && si >= 0 && f[fi] == s[si])
            {
                res.Push(f[fi]);
                f.RemoveAt(fi--);
                s.RemoveAt(si--);
            }

            LookupInfo result = new LookupInfo();
            while (res.Count > 0)
                result.Add(res.Pop());

            return result;
        }
    }
}

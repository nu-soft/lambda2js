﻿using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Lambda2Js
{
    public class JavascriptConversionContext
    {
        [NotNull]
        private readonly JavascriptWriter result;

        [NotNull]
        private Expression node;

        internal bool preventDefault;

        internal bool gotWriter;

        public JavascriptConversionContext(
            [NotNull] Expression node,
            [NotNull] ExpressionVisitor visitor,
            [NotNull] JavascriptWriter result,
            [NotNull] JavascriptCompilationOptions options)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            this.result = result;
            this.Visitor = visitor;
            this.Options = options;
            this.node = node;
        }

        /// <summary>
        /// Gets the node being converted.
        /// [Do not set this property, as the setter will be soon removed. Use either `WriteLambda` or `WriteExpression` method.]
        /// </summary>
        /// <remarks>
        /// The preferred way to process another node, instead of setting this property,
        /// is calling either `WriteLambda` or `WriteExpression` method.
        /// </remarks>
        [NotNull]
        public Expression Node
        {
            get
            {
                return this.node;
            }
            #region Supported will be removed in v2
#if V1
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.node = value;
            }
#endif
#endregion
            }

        public void PreventDefault()
        {
            this.preventDefault = true;
        }

        [NotNull]
        public ExpressionVisitor Visitor { get; private set; }

        [NotNull]
        public JavascriptCompilationOptions Options { get; private set; }

        /// <summary>
        /// Gets a JavaScript writer, to output JavaScript code as the result of a node conversion.
        /// When this method is used, it marks the context as being used already,
        /// so that the node is not compiled again by any other extension or default behavior of the converter.
        /// </summary>
        public JavascriptWriter GetWriter()
        {
            this.gotWriter = true;
            return this.result;
        }

        public void WriteLambda<T>(Expression<T> expression)
        {
            this.Visitor.Visit(expression);
        }

        public void WriteExpression<T>(Expression<T> expression)
        {
            this.Visitor.Visit(expression.Body);
        }
    }
}

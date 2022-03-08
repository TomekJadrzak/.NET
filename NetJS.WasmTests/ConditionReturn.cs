using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazor.WasmTests
{
    /// <summary>
    /// Adds missing return to condition code
    /// </summary>
    public  class ConditionReturn : CSharpSyntaxWalker
    {
        private ReturnStatementSyntax _returnStatement = null;

        /// <summary>
        /// Process code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string Process( string code )
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText( code );
            SyntaxNode root = tree.GetRoot();

            root = Process( root );

            return root.ToFullString();
        }

        /// <summary>
        /// Process code
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public SyntaxNode Process( SyntaxNode root )
        {
            _returnStatement = null;

            Visit( root );

            if ( _returnStatement == null )
            {
                return AddReturn( root );
            }

            if ( _returnStatement.SemicolonToken.IsMissing )
            {
                return FixReturn( root );
            }

            return root;
        }

        /// <inheritdoc/>
        public override void VisitReturnStatement( ReturnStatementSyntax node )
        {
            _returnStatement = node;
        }

        /// <inheritdoc/>
        public override void VisitMethodDeclaration( MethodDeclarationSyntax node )
        {
            // skip
        }

        /// <inheritdoc/>
        public override void VisitLocalFunctionStatement( LocalFunctionStatementSyntax node )
        {
            // skip
        }

        /// <inheritdoc/>
        public override void VisitAnonymousMethodExpression( AnonymousMethodExpressionSyntax node )
        {
            // skip
        }

        /// <inheritdoc/>
        public override void VisitSimpleLambdaExpression( SimpleLambdaExpressionSyntax node )
        {
            // skip
        }

        /// <inheritdoc/>
        public override void VisitParenthesizedLambdaExpression( ParenthesizedLambdaExpressionSyntax node )
        {
            // skip
        }

        private SyntaxNode FixReturn(SyntaxNode root)
        {
            root = root.ReplaceNode(
                _returnStatement,
                _returnStatement.WithSemicolonToken( SyntaxFactory.Token( SyntaxKind.SemicolonToken ) ) );

            return root;
        }

        private static SyntaxNode AddReturn(SyntaxNode root)
        {
            ExpressionStatementSyntax lastStatement = FindLastExpressionStatement( root );

            if ( lastStatement != null )
            {
                root = root.ReplaceNode(
                    lastStatement,
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.Token( SyntaxKind.ReturnKeyword )
                        .WithTrailingTrivia(
                            SyntaxFactory.Whitespace( " " ) ),
                        lastStatement.Expression.WithTrailingTrivia(),
                        SyntaxFactory.Token( SyntaxKind.SemicolonToken )
                            .WithTrailingTrivia( SyntaxFactory.CarriageReturnLineFeed ) )
                    .WithLeadingTrivia( lastStatement.GetLeadingTrivia() ) );
            }

            return root;
        }

        private static ExpressionStatementSyntax FindLastExpressionStatement(SyntaxNode root)
        {
            ExpressionStatementSyntax expressionSyntax = null;

            foreach ( SyntaxNode node in root.ChildNodes() )
            {
                if ( node.IsKind( SyntaxKind.GlobalStatement ) )
                {
                    foreach (SyntaxNode child in node.ChildNodes() )
                    {
                        switch (child.Kind())
                        {
                            case SyntaxKind.ExpressionStatement:
                                expressionSyntax = (ExpressionStatementSyntax) child;
                                break;
                            case SyntaxKind.LocalFunctionStatement:
                                break;
                            default:
                                expressionSyntax = null;
                                break;
                        }
                    }
                }
            }

            return expressionSyntax;
        }
    }
}

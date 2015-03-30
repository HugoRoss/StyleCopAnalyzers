﻿namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Helpers;
    using System.Collections.Generic;



    /// <summary>
    /// An element within a C# code file is out of order in relation to the other elements in the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code elements within a file do not follow a standard ordering
    /// scheme.</para>
    ///
    /// <para>To comply with this rule, elements at the file root level or within a namespace must be positioned in the
    /// following order:</para>
    ///
    /// <list type="bullet">
    /// <item>Extern alias directives</item>
    /// <item>Using directives</item>
    /// <item>Namespaces</item>
    /// <item>Delegates</item>
    /// <item>Enums</item>
    /// <item>Interfaces</item>
    /// <item>Structs</item>
    /// <item>Classes</item>
    /// </list>
    ///
    /// <para>Within a class, struct, or interface, elements must be positioned in the following order:</para>
    ///
    /// <list type="bullet">
    /// <item>Fields</item>
    /// <item>Constructors</item>
    /// <item>Finalizers</item>
    /// <item>Delegates</item>
    /// <item>Events</item>
    /// <item>Enums</item>
    /// <item>Interfaces</item>
    /// <item>Properties</item>
    /// <item>Indexers</item>
    /// <item>Methods</item>
    /// <item>Structs</item>
    /// <item>Classes</item>
    /// </list>
    ///
    /// <para>Complying with a standard ordering scheme based on element type can increase the readability and
    /// maintainability of the file and encourage code reuse.</para>
    ///
    /// <para>When implementing an interface, it is sometimes desirable to group all members of the interface next to
    /// one another. This will sometimes require violating this rule, if the interface contains elements of different
    /// types. This problem can be solved through the use of partial classes.</para>
    ///
    /// <list type="number">
    /// <item>Add the partial attribute to the class, if the class is not already partial.</item>
    /// <item>Add a second partial class with the same name. It is possible to place this in the same file, just below
    /// the original class, or within a second file.</item>
    /// <item>Move the interface inheritance and all members of the interface implementation to the second part of the
    /// class.</item>
    /// </list>
    ///
    /// <para>For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Represents a customer of the system.
    /// /// &lt;/summary&gt;
    /// public partial class Customer
    /// {
    ///     // Contains the main functionality of the class.
    /// }
    ///
    /// /// &lt;content&gt;
    /// /// Implements the ICollection class.
    /// /// &lt;/content&gt;
    /// public partial class Customer : ICollection
    /// {
    ///     public int Count
    ///     {
    ///         get { return this.count; }
    ///     }
    ///
    ///     public bool IsSynchronized
    ///     {
    ///         get { return false; }
    ///     }
    ///
    ///     public object SyncRoot
    ///     {
    ///         get { return null; }
    ///     }
    ///
    ///     public void CopyTo(Array array, int index)
    ///     {
    ///         throw new NotImplementedException();
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1201ElementsMustAppearInTheCorrectOrder : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1201ElementsMustAppearInTheCorrectOrder"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1201";
        private const string Title = "Elements must appear in the correct order";
        private const string MessageFormat = "A {0} should not follow a {1}.";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "An element within a C# code file is out of order in relation to the other elements in the code.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1201.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        // extern alias and usings are missing here because the compiler itself is enforcing the right order.
        private static readonly ImmutableArray<SyntaxKind> OuterOrder = ImmutableArray.Create(
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration);

        private static readonly ImmutableArray<SyntaxKind> TypeMemberOrder = ImmutableArray.Create(
            SyntaxKind.FieldDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration);

        private static readonly Dictionary<SyntaxKind, string> MemberNames = new Dictionary<SyntaxKind, string>
        {
            [SyntaxKind.NamespaceDeclaration] = "namespace",
            [SyntaxKind.DelegateDeclaration] = "delegate",
            [SyntaxKind.EnumDeclaration] = "enum",
            [SyntaxKind.InterfaceDeclaration] = "interface",
            [SyntaxKind.StructDeclaration] = "struct",
            [SyntaxKind.ClassDeclaration] = "class",
            [SyntaxKind.FieldDeclaration] = "field",
            [SyntaxKind.ConstructorDeclaration] = "constructor",
            [SyntaxKind.DestructorDeclaration] = "destructor",
            [SyntaxKind.DelegateDeclaration] = "delegate",
            [SyntaxKind.EventDeclaration] = "event",
            [SyntaxKind.EnumDeclaration] = "enum",
            [SyntaxKind.InterfaceDeclaration] = "interface",
            [SyntaxKind.PropertyDeclaration] = "property",
            [SyntaxKind.IndexerDeclaration] = "indexer",
            [SyntaxKind.MethodDeclaration] = "method"
        };

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(this.HandleNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = context.Node as TypeDeclarationSyntax;

            HandleMemberList(context, typeDeclaration.Members, TypeMemberOrder);
        }

        private void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = context.Node as CompilationUnitSyntax;

            HandleMemberList(context, compilationUnit.Members, OuterOrder);
        }

        private void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = context.Node as NamespaceDeclarationSyntax;

            HandleMemberList(context, compilationUnit.Members, OuterOrder);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members, ImmutableArray<SyntaxKind> order)
        {
            for (int i = 0; i < members.Count - 1; i++)
            {
                int index = order.IndexOf(members[i].Kind());

                int nextIndex = order.IndexOf(members[i + 1].Kind());

                if (index > nextIndex)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(members[i + 1]), MemberNames[members[i + 1].Kind()], MemberNames[members[i].Kind()]));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Quantum.QsCompiler.CsharpGeneration;
using Microsoft.Quantum.QsCompiler.SyntaxTokens;
using Microsoft.Quantum.QsCompiler.SyntaxTree;

namespace Microsoft.Quantum.QsCompiler.AutoEmulation
{
    /// <summary>
    /// A Q# rewrite step for auto emulation
    /// </summary>
    ///
    /// <para>
    /// This rewrite step creates custom emulators for operations that have the
    /// `EmulateWith` attribute.  This attribute holds an alternative operation,
    /// with the same signature, as its first argument, and a simulator, for which
    /// the alternative operation should be used, as its second argument.
    /// </para>
    public class RewriteStep : IRewriteStep
    {
        public string Name => "AutoEmulation";

        // This rewrite step needs to be run before C# code generation
        public int Priority => -2;

        public IDictionary<string, string?> AssemblyConstants { get; } = new Dictionary<string, string?>();

        public IEnumerable<IRewriteStep.Diagnostic> GeneratedDiagnostics => diagnostics;

        public bool ImplementsPreconditionVerification => false;

        public bool ImplementsTransformation => true;

        public bool ImplementsPostconditionVerification => false;

        public bool PostconditionVerification(QsCompilation compilation) => throw new System.NotImplementedException();

        public bool PreconditionVerification(QsCompilation compilation) => throw new System.NotImplementedException();

        public bool Transformation(QsCompilation compilation, [NotNullWhen(true)] out QsCompilation? transformed)
        {
            // we do not change the Q# syntax tree
            transformed = compilation;

            // global callables
            var globalCallables = compilation.Namespaces.GlobalCallableResolutions();

            // collect all callables that have an emulation attribute
            var globals = globalCallables.Where(p => p.Value.Source.CodeFile.EndsWith(".qs"))
                                         .Where(p => p.Value.Attributes.Any(HasEmulationAttribute));

            if (!globals.Any())
            {
                diagnostics.Add(new IRewriteStep.Diagnostic {
                    Severity = DiagnosticSeverity.Info,
                    Message = "AutoEmulation: no operations have @EmulateWith attribute",
                    Stage = IRewriteStep.Stage.Transformation
                });
                return true;
            }

            // no need to generate any C# file, if there is no emulation attribute, or if we cannot retrieve the output path
            if (!AssemblyConstants.TryGetValue(Microsoft.Quantum.QsCompiler.ReservedKeywords.AssemblyConstants.OutputPath, out var outputPath))
            {
                diagnostics.Add(new IRewriteStep.Diagnostic {
                    Severity = DiagnosticSeverity.Error,
                    Message = "AutoEmulation: cannot determine output path for generated C# code",
                    Stage = IRewriteStep.Stage.Transformation
                });
                return false;
            }

            diagnostics.Add(new IRewriteStep.Diagnostic {
                Severity = DiagnosticSeverity.Info,
                Message = $"AutoEmulation: Generating file __AutoEmulation__.g.cs in {outputPath}",
                Stage = IRewriteStep.Stage.Transformation
            });

            using var writer = new StreamWriter(Path.Combine(outputPath, "__AutoEmulation__.g.cs"));
            var context = CodegenContext.Create(compilation, AssemblyConstants);

            var generator = new CodeGenerator(context);
            foreach (var (key, callable) in globals)
            {
                var attributeArguments = callable.Attributes.Where(HasEmulationAttribute).Select(GetEmulationAttributeArguments);
                foreach (var (alternativeOperation, _) in attributeArguments)
                {
                    var period = alternativeOperation.LastIndexOf('.');
                    if (period == -1)
                    {
                        diagnostics.Add(new IRewriteStep.Diagnostic {
                            Severity = DiagnosticSeverity.Error,
                            Message = $"AutoEmulation: name of alternative operation in {key.Namespace}.{key.Name} must be completely specified (including namespace)",
                            Stage = IRewriteStep.Stage.Transformation
                        });
                        return false;
                    }

                    var qualifiedName = new QsQualifiedName(alternativeOperation.Substring(0, period), alternativeOperation.Substring(period + 1));
                    if (!globalCallables.TryGetValue(qualifiedName, out var alternativeCallable))
                    {
                        diagnostics.Add(new IRewriteStep.Diagnostic {
                            Severity = DiagnosticSeverity.Error,
                            Message = $"AutoEmulation: cannot find alternative operation `{alternativeOperation}`",
                            Stage = IRewriteStep.Stage.Transformation
                        });
                        return false;
                    }

                    var callableSignature = callable.Signature;
                    var alternativeSignature = alternativeCallable.Signature;

                    if (!callable.Signature.Equals(alternativeCallable.Signature))
                    {
                        diagnostics.Add(new IRewriteStep.Diagnostic {
                            Severity = DiagnosticSeverity.Error,
                            Message = $"AutoEmulation: signature of `{alternativeOperation}` does not match the one of {key.Namespace}.{key.Name}",
                            Stage = IRewriteStep.Stage.Transformation
                        });
                        return false;
                    }

                    if (!GetSpecializationKinds(callable).IsSubsetOf(GetSpecializationKinds(alternativeCallable)))
                    {
                        diagnostics.Add(new IRewriteStep.Diagnostic {
                            Severity = DiagnosticSeverity.Error,
                            Message = $"AutoEmulation: specializations of `{alternativeOperation}` must be a superset of specializations of {key.Namespace}.{key.Name}",
                            Stage = IRewriteStep.Stage.Transformation
                        });
                        return false;
                    }
                }
                generator.AddCallable(key, callable, attributeArguments);
            }
            generator.WriteTo(writer);


            return true;
        }

        private static bool HasEmulationAttribute(QsDeclarationAttribute attribute) =>
            attribute.TypeId.IsValue && attribute.TypeId.Item.Namespace == "Microsoft.Quantum.Core" && attribute.TypeId.Item.Name == "EmulateWith";

        private static (string AlternativeOperation, string InSimulator) GetEmulationAttributeArguments(QsDeclarationAttribute attribute) =>
            attribute.Argument.Expression switch
            {
                QsExpressionKind<TypedExpression, Identifier, ResolvedType>.ValueTuple tuple =>
                    tuple.Item switch
                    {
                        var arr when arr.Count() == 2 =>
                            (arr.ElementAt(0).Expression, arr.ElementAt(1).Expression) switch
                            {
                                (QsExpressionKind<TypedExpression, Identifier, ResolvedType>.StringLiteral alternativeOperation, QsExpressionKind<TypedExpression, Identifier, ResolvedType>.StringLiteral inSimulator) => (alternativeOperation.Item1, inSimulator.Item1),
                                _ => throw new Exception("Unexpected argument")
                            },
                        _ => throw new Exception("Unexpected argument")
                    },
                _ => throw new Exception("Unexpected argument")
            };

        private static ISet<QsSpecializationKind> GetSpecializationKinds(QsCallable callable) =>
            callable.Specializations.Select(spec => spec.Kind).OrderBy(kind => kind).ToHashSet();

        private List<IRewriteStep.Diagnostic> diagnostics = new List<IRewriteStep.Diagnostic>();
    }
}

﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Quantum.Simulation.QuantumProcessor.Extensions //ToDo: update namespace to a more appropriate name
{
    open Microsoft.Quantum.Canon;

    // Private helper operations.
    internal operation Delay<'T>(op : ('T => Unit), arg : 'T, aux : Unit) : Unit {
        op(arg);
    }

    internal operation DelayC<'T>(op : ('T => Unit is Ctl), arg : 'T, aux : Unit) : Unit is Ctl {
        op(arg);
    }

    internal operation DelayA<'T>(op : ('T => Unit is Adj), arg : 'T, aux : Unit) : Unit is Adj {
        op(arg);
    }

    internal operation DelayCA<'T>(op : ('T => Unit is Ctl + Adj), arg : 'T, aux : Unit) : Unit is Ctl + Adj {
        op(arg);
    }


    // Private helper operations.
    operation ApplyIfElseIntrinsic(measurementResult : Result, onResultZeroOp : (Unit => Unit) , onResultOneOp : (Unit => Unit)) : Unit {
        body intrinsic;
    }

    operation ApplyIfElseIntrinsicA(measurementResult : Result, onResultZeroOp : (Unit => Unit is Adj) , onResultOneOp : (Unit => Unit is Adj)) : Unit is Adj {
        body (...) {
            ApplyIfElseIntrinsic(measurementResult, onResultZeroOp, onResultOneOp);
        }
        adjoint (...) {
            ApplyIfElseIntrinsic(measurementResult, Adjoint onResultZeroOp, Adjoint onResultOneOp);
        }
    }

    operation ApplyIfElseIntrinsicC(measurementResult : Result, onResultZeroOp : (Unit => Unit is Ctl) , onResultOneOp : (Unit => Unit is Ctl)) : Unit is Ctl {
        body (...) {
            ApplyIfElseIntrinsic(measurementResult, onResultZeroOp, onResultOneOp);
        }
        controlled (ctls, ...) {
            ApplyIfElseIntrinsic(measurementResult, Controlled onResultZeroOp(ctls, _), Controlled onResultOneOp(ctls, _));
        }
    }

    operation ApplyIfElseIntrinsicCA(measurementResult : Result, onResultZeroOp : (Unit => Unit is Ctl + Adj) , onResultOneOp : (Unit => Unit is Ctl + Adj)) : Unit is Ctl + Adj {
        body (...) {
            ApplyIfElseIntrinsic(measurementResult, onResultZeroOp, onResultOneOp);
        }
        adjoint (...) {
            ApplyIfElseIntrinsic(measurementResult, Adjoint onResultZeroOp, Adjoint onResultOneOp);
        }
        controlled (ctls, ...) {
            ApplyIfElseIntrinsic(measurementResult, Controlled onResultZeroOp(ctls, _), Controlled onResultOneOp(ctls, _));
        }
        controlled adjoint (ctls, ...) {
            ApplyIfElseIntrinsic(measurementResult, Controlled Adjoint onResultZeroOp(ctls, _), Controlled Adjoint onResultOneOp(ctls, _));
        }
    }


    // Private helper operations.
    operation ApplyConditionallyIntrinsic(measurementResults : Result[], resultsValues : Result[], onEqualOp : (Unit => Unit) , onNonEqualOp : (Unit => Unit)) : Unit {
        body intrinsic;
    }

    operation ApplyConditionallyIntrinsicA(measurementResults : Result[], resultsValues : Result[], onEqualOp : (Unit => Unit is Adj) , onNonEqualOp : (Unit => Unit is Adj)) : Unit is Adj {
        body (...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, onEqualOp, onNonEqualOp);
        }
        adjoint (...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, Adjoint onEqualOp, Adjoint onNonEqualOp);
        }
    }

    operation ApplyConditionallyIntrinsicC(measurementResults : Result[], resultsValues : Result[], onEqualOp : (Unit => Unit is Ctl) , onNonEqualOp : (Unit => Unit is Ctl)) : Unit is Ctl {
        body (...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, onEqualOp, onNonEqualOp);
        }
        controlled (ctls, ...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, Controlled onEqualOp(ctls, _), Controlled onNonEqualOp(ctls, _));
        }
    }

    operation ApplyConditionallyIntrinsicCA(measurementResults : Result[], resultsValues : Result[], onEqualOp : (Unit => Unit is Ctl + Adj) , onNonEqualOp : (Unit => Unit is Ctl + Adj)) : Unit is Ctl + Adj {
        body (...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, onEqualOp, onNonEqualOp);
        }
        adjoint (...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, Adjoint onEqualOp, Adjoint onNonEqualOp);
        }
        controlled (ctls, ...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, Controlled onEqualOp(ctls, _), Controlled onNonEqualOp(ctls, _));
        }
        controlled adjoint (ctls, ...) {
            ApplyConditionallyIntrinsic(measurementResults, resultsValues, Controlled Adjoint onEqualOp(ctls, _), Controlled Adjoint onNonEqualOp(ctls, _));
        }
    }


    // Public operations that match Canon names.
    // This corresponds to "if" statement of the following form in Q#:
    // if (measurementResult == Zero) {onResultZeroOp(zeroArg);} else {onResultOneOp(oneArg);}
    operation ApplyIfElseR<'T,'U>(measurementResult : Result, (onResultZeroOp : ('T => Unit), zeroArg : 'T) , (onResultOneOp : ('U => Unit), oneArg : 'U)) : Unit {
        let zeroOp = Delay(onResultZeroOp, zeroArg, _);
        let oneOp = Delay(onResultOneOp, oneArg, _);
        ApplyIfElseIntrinsic(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfElseRA<'T,'U>(measurementResult : Result, (onResultZeroOp : ('T => Unit is Adj), zeroArg : 'T) , (onResultOneOp : ('U => Unit is Adj), oneArg : 'U)) : Unit is Adj {
        let zeroOp = DelayA(onResultZeroOp, zeroArg, _);
        let oneOp = DelayA(onResultOneOp, oneArg, _);
        ApplyIfElseIntrinsicA(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfElseRC<'T,'U>(measurementResult : Result, (onResultZeroOp : ('T => Unit is Ctl), zeroArg : 'T) , (onResultOneOp : ('U => Unit is Ctl), oneArg : 'U)) : Unit is Ctl {
        let zeroOp = DelayC(onResultZeroOp, zeroArg, _);
        let oneOp = DelayC(onResultOneOp, oneArg, _);
        ApplyIfElseIntrinsicC(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfElseRCA<'T,'U>(measurementResult : Result, (onResultZeroOp : ('T => Unit is Adj + Ctl), zeroArg : 'T) , (onResultOneOp : ('U => Unit is Adj + Ctl), oneArg : 'U)) : Unit is Ctl + Adj {
        let zeroOp = DelayCA(onResultZeroOp, zeroArg, _);
        let oneOp = DelayCA(onResultOneOp, oneArg, _);
        ApplyIfElseIntrinsicCA(measurementResult, zeroOp, oneOp);
    }


    // Public operations that match Canon names.
    // This corresponds to "if" statement of the following form in Q#:
    // if (measurementResult == Zero) {onResultZeroOp(zeroArg);}
    operation ApplyIfZero<'T>(measurementResult : Result, (onResultZeroOp : ('T => Unit), zeroArg : 'T)) : Unit {
        let zeroOp = Delay(onResultZeroOp, zeroArg, _);
        let oneOp = Delay(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsic(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfZeroA<'T>(measurementResult : Result, (onResultZeroOp : ('T => Unit is Adj), zeroArg : 'T)) : Unit is Adj{
        let zeroOp = DelayA(onResultZeroOp, zeroArg, _);
        let oneOp = DelayA(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsicA(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfZeroC<'T>(measurementResult : Result, (onResultZeroOp : ('T => Unit is Ctl), zeroArg : 'T)) : Unit is Ctl {
        let zeroOp = DelayC(onResultZeroOp, zeroArg, _);
        let oneOp = DelayC(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsicC(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfZeroCA<'T>(measurementResult : Result, (onResultZeroOp : ('T => Unit is Ctl + Adj), zeroArg : 'T)) : Unit is Ctl + Adj {
        let zeroOp = DelayCA(onResultZeroOp, zeroArg, _);
        let oneOp = DelayCA(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsicCA(measurementResult, zeroOp, oneOp);
    }


    // Public operations that match Canon names.
    // This corresponds to "if" statement of the following form in Q#:
    // if (measurementResult == One) {onResultOneOp(oneArg);}
    operation ApplyIfOne<'T>(measurementResult : Result, (onResultOneOp : ('T => Unit), oneArg : 'T)) : Unit {
        let oneOp = Delay(onResultOneOp, oneArg, _);
        let zeroOp = Delay(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsic(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfOneA<'T>(measurementResult : Result, (onResultOneOp : ('T => Unit is Adj), oneArg : 'T)) : Unit is Adj {
        let oneOp = DelayA(onResultOneOp, oneArg, _);
        let zeroOp = DelayA(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsicA(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfOneC<'T>(measurementResult : Result, (onResultOneOp : ('T => Unit is Ctl), oneArg : 'T)) : Unit is Ctl {
        let oneOp = DelayC(onResultOneOp, oneArg, _);
        let zeroOp = DelayC(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsicC(measurementResult, zeroOp, oneOp);
    }

    operation ApplyIfOneCA<'T>(measurementResult : Result, (onResultOneOp : ('T => Unit is Ctl + Adj), oneArg : 'T)) : Unit is Ctl + Adj {
        let oneOp = DelayCA(onResultOneOp, oneArg, _);
        let zeroOp = DelayCA(NoOp<Unit>, (), _);
        ApplyIfElseIntrinsicCA(measurementResult, zeroOp, oneOp);
    }


    // Public operations that match Canon names.
    // This corresponds to "if" statement of the following form in Q#:
    // if ((measurementResults[0] == resultsValues[0]) && (measurementResults[1] == resultsValues[1])) {onEqualOp(equalArg);} else {onNonEqualOp(nonEqualArg);}
    operation ApplyConditionally<'T,'U>(measurementResults : Result[], resultsValues : Result[], (onEqualOp : ('T => Unit), equalArg : 'T) , (onNonEqualOp : ('U => Unit), nonEqualArg : 'U)) : Unit {
        let equalOp = Delay(onEqualOp,equalArg,_);
        let nonEqualOp = Delay(onNonEqualOp,nonEqualArg,_);
        ApplyConditionallyIntrinsic(measurementResults, resultsValues, equalOp, nonEqualOp);
    }

    operation ApplyConditionallyA<'T,'U>(measurementResults : Result[], resultsValues : Result[], (onEqualOp : ('T => Unit is Adj), equalArg : 'T) , (onNonEqualOp : ('U => Unit is Adj), nonEqualArg : 'U)) : Unit is Adj {
        let equalOp = DelayA(onEqualOp, equalArg, _);
        let nonEqualOp = DelayA(onNonEqualOp, nonEqualArg, _);
        ApplyConditionallyIntrinsicA(measurementResults, resultsValues, equalOp, nonEqualOp);
    }

    operation ApplyConditionallyC<'T,'U>(measurementResults : Result[], resultsValues : Result[], (onEqualOp : ('T => Unit is Ctl), equalArg : 'T) , (onNonEqualOp : ('U => Unit is Ctl), nonEqualArg : 'U)) : Unit is Ctl {
        let equalOp = DelayC(onEqualOp, equalArg, _);
        let nonEqualOp = DelayC(onNonEqualOp, nonEqualArg, _);
        ApplyConditionallyIntrinsicC(measurementResults, resultsValues, equalOp, nonEqualOp);
    }

    operation ApplyConditionallyCA<'T,'U>(measurementResults : Result[], resultsValues : Result[], (onEqualOp : ('T => Unit is Ctl + Adj), equalArg : 'T) , (onNonEqualOp : ('U => Unit is Ctl + Adj), nonEqualArg : 'U)) : Unit is Ctl + Adj {
        let equalOp = DelayCA(onEqualOp, equalArg, _);
        let nonEqualOp = DelayCA(onNonEqualOp, nonEqualArg, _);
        ApplyConditionallyIntrinsicCA(measurementResults, resultsValues, equalOp, nonEqualOp);
    }

}

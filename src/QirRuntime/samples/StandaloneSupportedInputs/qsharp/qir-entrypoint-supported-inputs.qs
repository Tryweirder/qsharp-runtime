﻿namespace Quantum.StandaloneSupportedInputs {

    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Intrinsic;

    
    @EntryPoint()
    operation ExerciseSupportedInputs (anInt : Int, aDouble : Double) : Unit {
        Message("Exercise Supported Inputs");
        Message($"anInt: {anInt}");
        Message($"anDouble: {aDouble}");
    }
}

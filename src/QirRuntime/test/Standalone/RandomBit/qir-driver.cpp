// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include <iostream>
#include "../Shared/CLI11.hpp"

// Can manually add calls to DebugLog in the ll files for debugging.
extern "C" void DebugLog(int64_t value)
{
    std::cout << value << std::endl;
}
extern "C" void DebugLogPtr(char* value)
{
    std::cout << (const void*)value << std::endl;
}

extern "C" void SetupQirToRunOnFullStateSimulator();
extern "C" bool Microsoft__Quantum__Testing__QIR__RandomBit__body(); // NOLINT

int main(int argc, char *argv[])
{
    std::cout << "QIR Driver" << std::endl;
    SetupQirToRunOnFullStateSimulator();
    
    bool bit = Microsoft__Quantum__Testing__QIR__RandomBit__body();
    std::cout << bit << std::endl;
    return 0;
}

using Ion;
using Ion.Memory;



//var ntoskrnlBase = Drivers.Find("ntoskrnl.exe");
//var process = ExtendedProcess.CurrentProcess;
//var module = process.ModuleManager.Load(@"C:\Windows\System32\ntoskrnl.exe");
//var dump = module.Dump();

//var kdDebuggerNotPresentOffset = GetKdDebuggerNotPresentOffset(dump);
//var valueOffset = GetPatchGuardValueOffset(process, dump);

//Console.WriteLine();

//static unsafe int GetKdDebuggerNotPresentOffset(IMemoryDump dump)
//{
//    const int opCodeLength = 2;
//    const int ripInstructionLength = 6;

//    var offset = dump.ScanFirst("38 0D ? ? ? ? 75 02 EB FE", 0, 0, false); // cmp KdDebuggerNotPresent, cl
//    var callbackAddress = dump.Address + offset;
//    var callbackRipOffset = *(int*)(callbackAddress + opCodeLength);
//    var address = callbackAddress + callbackRipOffset + ripInstructionLength;

//    return (int)(address.ToInt64() - dump.Address.ToInt64());
//}

//static unsafe int GetPatchGuardValueOffset(IProcess process, IMemoryDump dump)
//{
//    var pe32 = process.GetPE32(dump.Address);
//    var section = pe32.FindSection(".rdata");
//    var offset = dump.ScanFirst("00 00 00 00 00 00 00 00", 0, 0, false, (int)section.VirtualAddress);
//    var address = dump.Address + offset;

//    return 0;
//}


Console.WriteLine();
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using Utils;

namespace Code
{
    public class CallAdress
    {
        public long offset;
        public long startAddress;
        public long realAddress;
        public long codeBase;
        public long endAddress;
        public bool isTreated;
        public bool call;
        public CodeLine cdLine;
        public bool Contains(long address)
        {
            return ((startAddress <= address) & (address < endAddress));
        }
        public void setEnd(long e)
        {
            endAddress = e + codeBase;
            isTreated = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param nameIndex="PositionOfStructureInFile">PositionOfStructureInFile of the call</param>
        /// <param nameIndex="adressOfCall">addressOfFunction to call</param>
        /// <param nameIndex="cdBase"></param>
        /// <param nameIndex="callOrJump"></param>
        /// <param nameIndex="callingLine"></param>
        public CallAdress(long position, long adressOfCall, long cdBase, bool callOrJump, CodeLine callingLine)
        {
            offset = position;
            startAddress = adressOfCall;
            realAddress = startAddress - cdBase;
            isTreated = false;
            call = callOrJump;
            codeBase = cdBase;
            cdLine = callingLine;
        }
        public override string ToString()
        {
            return realAddress.ToString("x8");
        }
    }
    public class Bloc
    {
        public CallAdress startAddress;
        public List<CodeLine> code;
        public long codeBase;
        public string blocName;
        public bool Contains(long add)
        {
            return ((start <= add) & (add <= end));
        }
        public long endAddress
        {
            get
            {
                if (code.Count > 0)
                    return code[code.Count - 1].PositionOfStructureInFile + codeBase;
                return 0;
            }
        }
        public long start, end;
        public Bloc(long start, long end)
        {
            this.start = start;
            this.end = end;
        }
        public Bloc(CallAdress start, long codeB, string name)
        {
            startAddress = start;
            code = new List<CodeLine>();
            codeBase = codeB;
            blocName = name;
        }
    }
    public class Disassemble : IMAGE_BASE_DATA
    {
        Executable exe;
        public int lineNumber;

        private int codeSize;
        public int codeBase;
        public long imageBase;
        public int endCode;
        public int addressOfEntryPoint;
        public int debugData;
        private int debugSize;
        public List<long> references = new List<long>();
        static SortedList<int, Instruction> instructions = new SortedList<int, Instruction>();
        public List<CallAdress> subroutines = new List<CallAdress>();
        public Queue<CallAdress> calls = new Queue<CallAdress>();
        private List<long> blocAddress = new List<long>();
        public List<CodeLine> code;
        public SortedList<long, Bloc> blocs = new SortedList<long, Bloc>();
        public SortedList<long, CodeLine> lines = new SortedList<long, CodeLine>();
        public List<Label> labels = new List<Label>();
        public List<Bloc> blocList = new List<Bloc>();
        public List<CodeLine> CodeLines
        {
            get
            {
                List<CodeLine> l = new List<CodeLine>();
                foreach (CodeLine c in lines.Values)
                    l.Add(c);
                return l;
            }
        }
        public static void ReadOpCode()
        {
            string codeFile = "x86code.dat";
            if (File.Exists(codeFile))
            {
                Stream stream = File.Open(codeFile, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();
                instructions = (SortedList<int, Instruction>)bformatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                string fileName = @"F:\Mes Documents\Visual Studio 2010\Projects\BinHed\BinHed\bin\Debug\x86reference.Xml";
                if (!File.Exists(fileName))
                {
                    return;
                }
                XmlDocument xml = new XmlDocument();
                xml.Load(fileName);
                foreach (XmlNode nod in xml.ChildNodes[3].ChildNodes)
                {
                    int type = 1;
                    switch (nod.Name)
                    {
                        case "one-byte":

                            break;
                        case "two-byte":
                            type = 2;
                            break;
                    }
                    foreach (XmlNode son in nod.ChildNodes)
                    {
                        if (son.Name == "pri_opcd")
                        {
                            Instruction ins = new Instruction(son, type);
                            instructions.Add(ins.long0pCode, ins);
                        }
                    }
                }
                Stream stream = File.Open(codeFile, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, instructions);
                stream.Close();
            }
        }
        public Disassemble(Executable exe, BinaryFileReader sw)
        {
            ReadOpCode();
            lines.Clear();
            this.exe = exe;
            //       InitData(sw);
            byte[] buffer = InitData(exe, sw);
            if (buffer == null)
                return;
            CallAdress c = new CallAdress(addressOfEntryPoint, addressOfEntryPoint, codeBase, false, null);
 //           ParseInstructionsFollowCalls32Old(buffer, c, "");
            BitStreamReader sb = new BitStreamReader(buffer, true);
            ParseInstructionsFollowCalls32(sb, c, "");
            while (calls.Count > 0)
            {
                #region Main loop
                c = calls.Dequeue();
                long st = c.startAddress & 0xFFFFFF;
                if ((st >= addressOfEntryPoint) & (st < endCode))
                {
                    CodeLine cdl;
                    if (!lines.TryGetValue(c.startAddress, out cdl))
                    {
                        try
                        {
                            int count = ParseInstructionsFollowCalls32(sb, c, "");
                            if (lines.Count > 10000)
                                return;
                        }
                        catch
                        {
                            Trace.WriteLine(c.realAddress);
                        }
                    }
                }
                #endregion
            }
            references.Sort();
            for (int i = 0; i < references.Count; i++)
            {
                #region Try to analyse references
                long ad = references[i];
                //Find following
                int next = references.Count - 1;
                int addr = (int)(ad - exe.NT_Headers.OptionalHeader.ImageBase - codeBase);
                if (addr == 0xe44)
                { }
                if ((addr < buffer.Length) & (addr > 0))
                {
                    bool done = false;
                    long nextAddr = buffer.Length;
                    for (int j = 0; j < blocList.Count; j++)
                    {
                        if (blocList[j].Contains(addr))
                        {
                            done = true;
                            break;
                        }
                    }
                    if (!done)
                    {
                        foreach (CodeLine cx in lines.Values)
                        {
                            if (cx.PositionOfStructureInFile > addr)
                            {
                                nextAddr = cx.PositionOfStructureInFile;
                                break;
                            }

                        }
                        if (i + 1 < references.Count)
                            if (nextAddr > references[i + 1] - exe.NT_Headers.OptionalHeader.ImageBase - codeBase)
                                nextAddr = references[i + 1] - exe.NT_Headers.OptionalHeader.ImageBase - codeBase;
                        if ((i < references.Count - 1) && (nextAddr - addr < 0xF0))
                        {
                            int length = (int)(nextAddr - addr);
                            try
                            {
                                SortData(buffer, addr, length);
                            }
                            catch { }
                        }
                        else
                        {
                            //                  int strLength = 4;
                            //                   SortData(codeBuffer, addr, strLength);
                        }
                    }
                }
                #endregion
            }
            long position = lines.Values[0].PositionOfStructureInFile;
            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    #region Find empty blocks
                    CodeLine cc = lines.Values[i];
                    if (cc.PositionOfStructureInFile - position > 0)
                    {
                        if (cc.PositionOfStructureInFile - position > 1)
                            try
                            {
                                SortData(buffer, (int)position, (int)(cc.PositionOfStructureInFile - position));
                            }
                            catch { }
                        else
                            try
                            {
                                lines.Add(position + codeBase, new CodeLine(position, buffer[(int)position], "", position));
                            }
                            catch { }
                    }
                    position = cc.PositionOfStructureInFile + cc.BinaryCode.Length / 2;
                    if (cc.BinaryCode.Length % 2 == 1)
                        position++;
                    #endregion
                }
            }
            catch { }
        }
        private void SortData(byte[] buffer, int addr, int length)
        {
            #region Try to find a string
            byte[] dat = new byte[length];
            Buffer.BlockCopy(buffer, addr, dat, 0, dat.Length);
            int end = dat.Length - 1;
            while (dat[end] == 0x90) //Trim filling
            {
                end--;
                if (end <= 0)
                    break;
            }
            if (end == 0)
            {
                lines.Add(addr + codeBase, new CodeLine(dat, addr, "Filling", addr));
            }
            else
                switch (dat[end])
                {
                    case 0xC3: // Code ??
                        break;
                    case 0:
                        #region String

                        string g = "";
                        if (dat[end - 1] == 0)
                            g = Encoding.Unicode.GetString(dat);
                        else
                            g = Encoding.Default.GetString(dat);
                        try
                        {
                            lines.Add(addr + codeBase, new CodeLine(dat, addr, g, addr));
                        }
                        catch { }
                        #endregion
                        break;
                    default:
                        ParseStandardData(addr, dat);
                        break;
                }
            #endregion
        }
        private void ParseStandardData(int addr, byte[] dat)
        {
            #region Standard user Strings
            if (dat.Length % 4 == 0)
            {
                byte[] k = BitConverter.GetBytes((int)exe.NT_Headers.OptionalHeader.ImageBase);
                byte[] j = BitConverter.GetBytes((int)exe.NT_Headers.OptionalHeader.BaseOfData);
                for (int z = 0; z < dat.Length / 4; z++)
                {
                    byte[] word = new byte[4];
                    Buffer.BlockCopy(dat, z * 4, word, 0, word.Length);
                    string txt = "raw data";
                    int x = BitConverter.ToInt32(word, 0);
                    if (x < ((int)exe.NT_Headers.OptionalHeader.ImageBase + (int)exe.NT_Headers.OptionalHeader.BaseOfData)
                       & (x >= (int)exe.NT_Headers.OptionalHeader.ImageBase + (int)exe.NT_Headers.OptionalHeader.BaseOfCode))
                        if (Array.IndexOf(references.ToArray(), x) < 0)
                        {
                            references.Add(x);
                            references.Sort(); txt = "L" + x.ToString("x8") + " - label";

                        }
                    if (x >= (int)exe.NT_Headers.OptionalHeader.ImageBase + (int)exe.NT_Headers.OptionalHeader.BaseOfCode)
                    {
                        //  txt = "L" + x.ToString("x8") + " - label";
                    }
                    try
                    {
                        lines.Add(addr + codeBase + z * 4, new CodeLine(word, addr + z * 4, txt, addr + z * 4));
                    }
                    catch
                    {
                        //                            return;
                    }
                }
            }
            else
            {
                lines.Add(addr + codeBase, new CodeLine(dat, addr, "raw data", addr));
            }
            #endregion
        }
        private void InitData(BitStreamReader sw)
        {
            #region Init
            code = new List<CodeLine>();
            addressOfEntryPoint = exe.NT_Headers.OptionalHeader.AddressOfEntryPoint;
            codeBase = exe.NT_Headers.OptionalHeader.BaseOfCode;
            imageBase = exe.NT_Headers.OptionalHeader.ImageBase;
            byte[] codeBuffer = null;
            int p = sw.Position;
            foreach (IMAGE_SECTION_HEADER hd in exe.Sections.Sections)
            {
                if (hd.Name.Contains(".text"))
                {
                    codeSize = hd.VirtualSize;
                    sw.Position = hd.pointerToRawData;
                    PositionOfStructureInFile = sw.Position;
                    LengthInFile = hd.sizeOfRawData;
                    break;
                }
            }
            endCode = codeBase + codeSize;
            sw.Position = p;
            #endregion
            #region Import Address Table
            if (exe.IAT.IATEntries.Count != 0)
            {
                blocList.Add(new Bloc(exe.IAT.startIAT, exe.IAT.endIAT));
                foreach (IMAGE_IMPORT_DIRECTORY_IAT_ENTRY imat in exe.IAT.IATEntries)
                {
                    CodeLine cd = new CodeLine(imat.ArrayPointer, imat.Address - exe.NT_Headers.OptionalHeader.SizeOfHeaders, imat.Name, imat.PositionOfStructureInFile);
                    lines.Add(imat.Address - exe.NT_Headers.OptionalHeader.SizeOfHeaders + codeBase, cd);
                }
            }
            #endregion
            #region Imports
            if ((exe.DataDirs[1].Import != null) & (exe.DataDirs[1].sectionNumber == 0))// true only if Import is in .text
            {
                // at that address  n * 5 words ( adress of table , of nameIndex , ..) finished by empty 5 words
                long position = exe.DataDirs[1].OffsetInSection;
                long start = position;
                string[] names = new string[] { "original Thunk", "time Date Stamp", "forwarder Chain", "name", "first Thunk", "" };
                for (int i = 0; i < exe.DataDirs[1].Import.Descriptors.Count; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        byte[] buf = sw.ReadBytes(4);
                        lines.Add(position + codeBase - exe.NT_Headers.OptionalHeader.SizeOfHeaders, new CodeLine(buf, sw.Position - 4 - exe.NT_Headers.OptionalHeader.SizeOfHeaders, names[j], position));
                        position += 4;
                    }
                }
                for (int j = 0; j < 5; j++)
                {
                    byte[] buf = sw.ReadBytes(4);
                    lines.Add(position + codeBase - exe.NT_Headers.OptionalHeader.SizeOfHeaders, new CodeLine(buf, position - exe.NT_Headers.OptionalHeader.SizeOfHeaders, "End of table", position));
                    position += 4;
                }
                foreach (IMAGE_IMPORT_DESCRIPTOR id in exe.DataDirs[1].Import.Descriptors)
                {
                    lines.Add(id.name, new CodeLine(Encoding.Default.GetBytes(id.Name), id.name - codeBase, id.Name, id.name - exe.NT_Headers.OptionalHeader.SizeOfHeaders));
                }
                foreach (IMAGE_IMPORT_DESCRIPTOR id in exe.DataDirs[1].Import.Descriptors)
                {
                    #region Thunk details
                    position = id.originalThunk;
                    //                   if (position - codeBase < codeBuffer.Length)// true only if Import is in .text
                    {
                        sw.Position = (int) position - exe.NT_Headers.OptionalHeader.SizeOfHeaders;
                        for (int i = 0; i < id.FirstThunks.Count; i++)
                        {
                            byte[] buf = new byte[4];
                            Buffer.BlockCopy(codeBuffer, (int)position - codeBase, buf, 0, 4);
                            lines.Add(position, new CodeLine(buf, position - codeBase, "Thunk " + i.ToString(), position));
                            position += 4;
                        }
                        byte[] buff = new byte[4];
                        Buffer.BlockCopy(codeBuffer, (int)position - codeBase, buff, 0, 4);
                        lines.Add(position, new CodeLine(buff, position - codeBase, "Filling ", position));
                        foreach (IMAGE_THUNK_DATA imt in id.FirstThunks)
                        {
                            position = imt.Function;
                            if (imt.AddressOfData != null)
                                if (imt.AddressOfData.Name != "")
                                {
                                    byte[] y = Encoding.Default.GetBytes(imt.AddressOfData.Name);
                                    lines.Add(imt.Function, new CodeLine(imt.Function - codeBase, imt.AddressOfData.Hint, imt.AddressOfData.Hint.ToString("x4"), imt.Function));
                                    lines.Add(imt.Function + 2, new CodeLine(Encoding.Default.GetBytes(imt.AddressOfData.Name), imt.Function + 2 - codeBase, imt.AddressOfData.Name, imt.Function + 2));
                                    lines.Add(imt.Function + 2 + imt.AddressOfData.Name.Length, new CodeLine(imt.Function + imt.AddressOfData.Name.Length - codeBase, 0, "", imt.Function + 2 + imt.AddressOfData.Name.Length));
                                    position = imt.Function + imt.AddressOfData.Name.Length;
                                }
                        }
                        Bloc b = new Bloc(start, position);
                        blocList.Add(b);
                    }
                    #endregion
                }
            }
            #endregion
            #region Exports
            if ((exe.DataDirs[0].Export != null) & (exe.DataDirs[0].sectionNumber == 0))
            {

                int indes = (int)exe.DataDirs[0].Export.PositionOfStructureInFile + (codeBase - exe.NT_Headers.OptionalHeader.SizeOfHeaders);// Faux
                int start = indes;
                List<byte> a = new List<byte>();
                string[] names = new string[] {"Reserved","TimeStamp","Version", "Address of Name", "Base", "Number of Functions", 
                    "Number of Names", "Address of functions", "Address of Names","Adress of Ordinals"};
                for (int i = 0; i < 9; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, names[i], indes));
                    indes += 4;
                }
                for (int i = 0; i < exe.DataDirs[0].Export.NumberOfFunctions; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, "Address of Function", indes));
                    indes += 4;
                }
                for (int i = 0; i < exe.DataDirs[0].Export.NumberOfNames; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, "Address of Name", indes));
                    indes += 4;
                }
                for (int i = 0; i < exe.DataDirs[0].Export.NumberOfFunctions; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, "Ordinal", indes));
                    indes += 4;
                }
                int x = codeBuffer[exe.DataDirs[0].Export.AddressOfName - codeBase];
                indes = exe.DataDirs[0].Export.AddressOfName;
                while (x != 0)
                {
                    a.Add((byte)x);
                    indes++;
                    x = codeBuffer[indes - codeBase];
                }
                try
                {
                    lines.Add(exe.DataDirs[0].Export.AddressOfName, new CodeLine(a.ToArray(), exe.DataDirs[0].Export.AddressOfName - codeBase, Encoding.Default.GetString(a.ToArray()), indes));
                }
                catch { }
                foreach (EXPORTED_FUNCTION exf in exe.DataDirs[0].Export.Exports)
                {
                    CallAdress d = new CallAdress(exf.addressOfFunction, exf.addressOfFunction, codeBase, false, null);
                    try
                    {
                        lines.Add(exf.addressOfName, new CodeLine(Encoding.Default.GetBytes(exf.Name), exf.addressOfName - codeBase, exf.Name, exf.addressOfName));
                        indes = exf.addressOfName + exf.Name.Length * 2;
                        ParseInstructionsFollowCalls32(sw, d, exf.Name);
                    }
                    catch { }
                }
                blocList.Add(new Bloc(start, indes));
            }
            #endregion
            #region Load configuration table
            int startConfig = (int)exe.DataDirs[10].VirtualAddress;
            int configSize = exe.DataDirs[10].Size;
            if (configSize > 0)
            {
                for (int i = 0; i < 0x10; i++)
                {
                    if (startConfig < codeBuffer.Length)
                    {
                        byte[] buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, startConfig - codeBase + 4 * i, buf, 0, 4);
                        lines.Add(startConfig + 4 * i, new CodeLine(buf, startConfig - codeBase + 4 * i, "Config table", startConfig));
                    }
                }
            }
            #endregion
            #region Relocation table
            startConfig = (int)exe.DataDirs[5].VirtualAddress;
            configSize = exe.DataDirs[5].Size;
            //   Bloc reloc = new Bloc(new CallAdress(startConfig - codeBase, startConfig, codeBase, false, null), codeBase, "Relocation table");
            //   byte[] buf = new byte[4];
            //  Buffer.BlockCopy(codeBuffer, startConfig - codeBase, buf, 0, 4);
            //      conf.code.Add(new CodeLine(startConfig - codeBase + 4 * u, buf, "userStrings"));
            //   blocs.Add(startConfig, conf);
            #endregion
            #region Debug table
            debugData = (int)exe.DataDirs[6].VirtualAddress;
            debugSize = exe.DataDirs[6].Size;
            int ind = debugData;
            if (debugSize > 0)
            {
                if (ind - codeBase < codeBuffer.Length)
                {
                    SortedList<long, IMAGE_DEBUG_DIRECTORY> imdeb = new SortedList<long, IMAGE_DEBUG_DIRECTORY>();
                    while (ind < debugData + debugSize)
                    {
                        IMAGE_DEBUG_DIRECTORY dbg = new IMAGE_DEBUG_DIRECTORY();
                        byte[] buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "characteristics", ind));
                        dbg.characteristics = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "time stamp", ind));
                        dbg.timeDateStamp = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "version", ind));
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, ((IMAGE_DEBUG)BitConverter.ToInt32(buf, 0)).ToString(), ind));
                        dbg.Type = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "size of data", ind));
                        dbg.SizeOfData = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "address of data", ind));
                        dbg.AddressOfRawData = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "pointer to data", ind));
                        dbg.PointerToRawData = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        imdeb.Add(dbg.AddressOfRawData, dbg);
                    }
                    for (int j = 0; j < imdeb.Count; j++)
                    {
                        IMAGE_DEBUG_DIRECTORY dbg = imdeb.Values[j];
                        byte[] buf = new byte[dbg.SizeOfData];
                        switch ((IMAGE_DEBUG)dbg.Type)
                        {
                            case IMAGE_DEBUG.IMAGE_DEBUG_TYPE_CODEVIEW:
                                Buffer.BlockCopy(codeBuffer, dbg.AddressOfRawData - codeBase, buf, 0, buf.Length);
                                BitStreamReader ms = new BitStreamReader(buf, false);
                                string signature = ms.ReadString(4);
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(Encoding.Default.GetBytes(signature), dbg.AddressOfRawData + ms.Position - 4 - codeBase, signature, dbg.AddressOfRawData));
                                int Offset = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, Offset, Offset.ToString("x8"), dbg.AddressOfRawData));
                                int Timestamp = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, Timestamp, "Timestamp", dbg.AddressOfRawData));
                                int Age = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, Age, "Age", dbg.AddressOfRawData));
                                int unk = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, unk, unk.ToString("x8"), dbg.AddressOfRawData));
                                int stNum = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, stNum, "Number of strings", dbg.AddressOfRawData));
                                for (int i = 0; i < stNum; i++)
                                {
                                    try
                                    {
                                        string dt = ms.ReadString();
                                        lines.Add(dbg.AddressOfRawData + ms.Position - dt.Length - 1, new CodeLine(buf, dbg.AddressOfRawData + ms.Position - dt.Length - 1 - codeBase, dt, dbg.AddressOfRawData));
                                    }
                                    catch { }
                                }
                                break;
                            default:
                                Buffer.BlockCopy(codeBuffer, dbg.AddressOfRawData - codeBase, buf, 0, dbg.SizeOfData);
                                lines.Add(dbg.AddressOfRawData, new CodeLine(buf, dbg.AddressOfRawData - codeBase, "Debug data : " + dbg.AddressOfRawData.ToString("x8"), dbg.AddressOfRawData));
                                break;
                        }
                    }
                }
            }
            #endregion
        }
        private byte[] InitData(Executable exe, BinaryFileReader sw)
        {
            #region Init
            this.exe = exe;
            code = new List<CodeLine>();
            addressOfEntryPoint = exe.NT_Headers.OptionalHeader.AddressOfEntryPoint;
            codeBase = exe.NT_Headers.OptionalHeader.BaseOfCode;
            imageBase = exe.NT_Headers.OptionalHeader.ImageBase;
            byte[] codeBuffer = null;
            long p = sw.Position;
            foreach (IMAGE_SECTION_HEADER hd in exe.Sections.Sections)
            {
                if (hd.Name.Contains(".text"))
                {
                    codeSize = hd.VirtualSize;
                    sw.Position = hd.pointerToRawData;
                    PositionOfStructureInFile = sw.Position;
                    LengthInFile = hd.sizeOfRawData;
                    codeBuffer = sw.ReadBytes(hd.sizeOfRawData);
                    break;
                }
            }
            endCode = codeBase + codeSize;
            sw.Position = p;
            #endregion
            #region Import Address Table
            if (exe.IAT.IATEntries.Count != 0)
            {
                blocList.Add(new Bloc(exe.IAT.startIAT, exe.IAT.endIAT));
                foreach (IMAGE_IMPORT_DIRECTORY_IAT_ENTRY imat in exe.IAT.IATEntries)
                {
                    CodeLine cd = new CodeLine(imat.ArrayPointer, imat.Address - exe.NT_Headers.OptionalHeader.SizeOfHeaders, imat.Name, imat.Address);
                    lines.Add(imat.Address - exe.NT_Headers.OptionalHeader.SizeOfHeaders + codeBase, cd);
                }
            }
            #endregion
            #region Imports
            if ((exe.DataDirs[1].Import != null) & (exe.DataDirs[1].sectionNumber == 0))// true only if Import is in .text
            {
                // at that address  n * 5 words ( adress of table , of nameIndex , ..) finished by empty 5 words
                long position = exe.DataDirs[1].OffsetInSection;
                long start = position;
                if (position - codeBase < codeBuffer.Length)
                {
                    string[] names = new string[] { "original Thunk", "time Date Stamp", "forwarder Chain", "name", "first Thunk", "" };
                    for (int i = 0; i < exe.DataDirs[1].Import.Descriptors.Count; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            byte[] buf = new byte[4];
                            Buffer.BlockCopy(codeBuffer, (int)position - exe.NT_Headers.OptionalHeader.SizeOfHeaders, buf, 0, 4);
                            lines.Add(position + codeBase - exe.NT_Headers.OptionalHeader.SizeOfHeaders, new CodeLine(buf, position - exe.NT_Headers.OptionalHeader.SizeOfHeaders, names[j], position));
                            position += 4;
                        }
                    }
                    for (int j = 0; j < 5; j++)
                    {
                        byte[] buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, (int)position - exe.NT_Headers.OptionalHeader.SizeOfHeaders, buf, 0, 4);
                        lines.Add(position + codeBase - exe.NT_Headers.OptionalHeader.SizeOfHeaders, new CodeLine(buf, position - exe.NT_Headers.OptionalHeader.SizeOfHeaders, "End of table", position));
                        position += 4;
                    }
                    foreach (IMAGE_IMPORT_DESCRIPTOR id in exe.DataDirs[1].Import.Descriptors)
                    {
                        lines.Add(id.name, new CodeLine(Encoding.Default.GetBytes(id.Name), id.name - codeBase, id.Name, position));
                    }
                }
                foreach (IMAGE_IMPORT_DESCRIPTOR id in exe.DataDirs[1].Import.Descriptors)
                {
                    #region Thunk details
                    position = id.originalThunk;
                    if (position - codeBase < codeBuffer.Length)// true only if Import is in .text
                    {
                        for (int i = 0; i < id.FirstThunks.Count; i++)
                        {
                            byte[] buf = new byte[4];
                            Buffer.BlockCopy(codeBuffer, (int)position - codeBase, buf, 0, 4);
                            lines.Add(position, new CodeLine(buf, position - codeBase, "Thunk " + i.ToString(), position));
                            position += 4;
                        }
                        byte[] buff = new byte[4];
                        Buffer.BlockCopy(codeBuffer, (int)position - codeBase, buff, 0, 4);
                        lines.Add(position, new CodeLine(buff, position - codeBase, "Filling ", position));
                        foreach (IMAGE_THUNK_DATA imt in id.FirstThunks)
                        {
                            position = imt.Function;
                            if (imt.AddressOfData != null)
                                if (imt.AddressOfData.Name != "")
                                {
                                    byte[] y = Encoding.Default.GetBytes(imt.AddressOfData.Name);
                                    lines.Add(imt.Function, new CodeLine(BitConverter.GetBytes(imt.AddressOfData.Hint), imt.Function - codeBase, imt.AddressOfData.Hint.ToString("x4"), position));
                                    position += 2;
                                    lines.Add(imt.Function + 2, new CodeLine(Encoding.Default.GetBytes(imt.AddressOfData.Name), position - codeBase, imt.AddressOfData.Name, position));
                                    position += imt.AddressOfData.Name.Length;
                                    lines.Add(imt.Function + 2 + imt.AddressOfData.Name.Length, new CodeLine(imt.AddressOfData.filler.ToArray(), position - codeBase, "null", position));
                                }
                        }
                        Bloc b = new Bloc(start, position);
                        blocList.Add(b);
                    }
                    #endregion
                }
            }
            #endregion
            #region Exports
            if ((exe.DataDirs[0].Export != null) & (exe.DataDirs[0].sectionNumber == 0))
            {

                int indes = (int)exe.DataDirs[0].Export.PositionOfStructureInFile + (codeBase - exe.NT_Headers.OptionalHeader.SizeOfHeaders);// Faux
                int start = indes;
                List<byte> a = new List<byte>();
                string[] names = new string[] {"Reserved","TimeStamp","Version", "Address of Name", "Base", "Number of Functions", 
                    "Number of Names", "Address of functions", "Address of Names","Adress of Ordinals"};
                for (int i = 0; i < 9; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, names[i], indes));
                    indes += 4;
                }
                for (int i = 0; i < exe.DataDirs[0].Export.NumberOfFunctions; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, "Address of Function", indes));
                    indes += 4;
                }
                for (int i = 0; i < exe.DataDirs[0].Export.NumberOfNames; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, "Address of Name", indes));
                    indes += 4;
                }
                for (int i = 0; i < exe.DataDirs[0].Export.NumberOfFunctions; i++)
                {
                    byte[] dat = new byte[4];
                    Buffer.BlockCopy(codeBuffer, indes - codeBase, dat, 0, dat.Length);
                    lines.Add(indes, new CodeLine(dat, indes - codeBase, "Ordinal", indes));
                    indes += 4;
                }
                int x = codeBuffer[exe.DataDirs[0].Export.AddressOfName - codeBase];
                indes = exe.DataDirs[0].Export.AddressOfName;
                while (x != 0)
                {
                    a.Add((byte)x);
                    indes++;
                    x = codeBuffer[indes - codeBase];
                }
                try
                {
                    lines.Add(exe.DataDirs[0].Export.AddressOfName, new CodeLine(a.ToArray(), exe.DataDirs[0].Export.AddressOfName - codeBase, Encoding.Default.GetString(a.ToArray()), indes));
                }
                catch { }
                foreach (EXPORTED_FUNCTION exf in exe.DataDirs[0].Export.Exports)
                {
                    CallAdress d = new CallAdress(exf.addressOfFunction, exf.addressOfFunction, codeBase, false, null);
                    try
                    {
                        lines.Add(exf.addressOfName, new CodeLine(Encoding.Default.GetBytes(exf.Name), exf.addressOfName - codeBase, exf.Name, exf.addressOfName));
                        indes = exf.addressOfName + exf.Name.Length * 2;
                        ParseInstructionsFollowCalls32(new BitStreamReader(codeBuffer, true), d, exf.Name);
                    }
                    catch { }
                }
                blocList.Add(new Bloc(start, indes));
            }
            #endregion
            #region Load configuration table
            int startConfig = (int)exe.DataDirs[10].VirtualAddress;
            int configSize = exe.DataDirs[10].Size;
            if (configSize > 0)
            {
                for (int i = 0; i < 0x10; i++)
                {
                    if (startConfig < codeBuffer.Length)
                    {
                        byte[] buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, startConfig - codeBase + 4 * i, buf, 0, 4);
                        lines.Add(startConfig + 4 * i, new CodeLine(buf, startConfig - codeBase + 4 * i, "Config table", startConfig));
                    }
                }
            }
            #endregion
            #region Relocation table
            startConfig = (int)exe.DataDirs[5].VirtualAddress;
            configSize = exe.DataDirs[5].Size;
            //   Bloc reloc = new Bloc(new CallAdress(startConfig - codeBase, startConfig, codeBase, false, null), codeBase, "Relocation table");
            //   byte[] buf = new byte[4];
            //  Buffer.BlockCopy(codeBuffer, startConfig - codeBase, buf, 0, 4);
            //      conf.code.Add(new CodeLine(startConfig - codeBase + 4 * u, buf, "userStrings"));
            //   blocs.Add(startConfig, conf);
            #endregion
            #region Debug table
            debugData = (int)exe.DataDirs[6].VirtualAddress;
            debugSize = exe.DataDirs[6].Size;
            int ind = debugData;
            if (debugSize > 0)
            {
                if (ind - codeBase < codeBuffer.Length)
                {
                    SortedList<long, IMAGE_DEBUG_DIRECTORY> imdeb = new SortedList<long, IMAGE_DEBUG_DIRECTORY>();
                    while (ind < debugData + debugSize)
                    {
                        IMAGE_DEBUG_DIRECTORY dbg = new IMAGE_DEBUG_DIRECTORY();
                        byte[] buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "characteristics", ind));
                        dbg.characteristics = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "time stamp", ind));
                        dbg.timeDateStamp = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "version", ind));
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, ((IMAGE_DEBUG)BitConverter.ToInt32(buf, 0)).ToString(), ind));
                        dbg.Type = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "size of data", ind));
                        dbg.SizeOfData = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "address of data", ind));
                        dbg.AddressOfRawData = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        buf = new byte[4];
                        Buffer.BlockCopy(codeBuffer, ind - codeBase, buf, 0, 4);
                        lines.Add(ind, new CodeLine(buf, ind - codeBase, "pointer to data", ind));
                        dbg.PointerToRawData = BitConverter.ToInt32(buf, 0);
                        ind += 4;
                        imdeb.Add(dbg.AddressOfRawData, dbg);
                    }
                    for (int j = 0; j < imdeb.Count; j++)
                    {
                        IMAGE_DEBUG_DIRECTORY dbg = imdeb.Values[j];
                        byte[] buf = new byte[dbg.SizeOfData];
                        switch ((IMAGE_DEBUG)dbg.Type)
                        {
                            case IMAGE_DEBUG.IMAGE_DEBUG_TYPE_CODEVIEW:
                                Buffer.BlockCopy(codeBuffer, dbg.AddressOfRawData - codeBase, buf, 0, buf.Length);
                                BitStreamReader ms = new BitStreamReader(buf, false);
                                string signature = ms.ReadString(4);
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(Encoding.Default.GetBytes(signature), dbg.AddressOfRawData + ms.Position - 4 - codeBase, signature, dbg.AddressOfRawData));
                                int Offset = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, Offset, Offset.ToString("x8"), dbg.AddressOfRawData));
                                int Timestamp = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, Timestamp, "Timestamp", dbg.AddressOfRawData));
                                int Age = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, Age, "Age", dbg.AddressOfRawData));
                                int unk = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, unk, unk.ToString("x8"), dbg.AddressOfRawData));
                                int stNum = ms.ReadInteger();
                                lines.Add(dbg.AddressOfRawData + ms.Position - 4, new CodeLine(dbg.AddressOfRawData + ms.Position - 4 - codeBase, stNum, "Number of strings", dbg.AddressOfRawData));
                                for (int i = 0; i < stNum; i++)
                                {
                                    try
                                    {
                                        string dt = ms.ReadString();
                                        lines.Add(dbg.AddressOfRawData + ms.Position - dt.Length - 1, new CodeLine(buf, dbg.AddressOfRawData + ms.Position - dt.Length - 1 - codeBase, dt, dbg.AddressOfRawData));
                                    }
                                    catch { }
                                }
                                break;
                            default:
                                Buffer.BlockCopy(codeBuffer, dbg.AddressOfRawData - codeBase, buf, 0, dbg.SizeOfData);
                                lines.Add(dbg.AddressOfRawData, new CodeLine(buf, dbg.AddressOfRawData - codeBase, "Debug data : " + dbg.AddressOfRawData.ToString("x8"), dbg.AddressOfRawData));
                                break;
                        }
                    }
                }
            }
            #endregion
            return codeBuffer;
        }
        private int ParseInstructionsFollowCalls32Old(byte[] buffer, CallAdress c, string blocName)
        {
            //http://wiki.osdev.org/X86_Instruction_Encoding
            int count = 0;
            int index = (int)c.realAddress;
            long start = index;
            int currentCode = buffer[index];
            Trace.Write("Start " + index.ToString("x8"));
            while ((index < exe.NT_Headers.OptionalHeader.BaseOfCode + exe.NT_Headers.OptionalHeader.SizeOfCode) & (currentCode != 0xCC))
            {
                foreach (Bloc b in blocList)
                {
                    if (b.Contains(index + codeBase))
                        return count;
                }
                if ((buffer[index] == 0x90) & (buffer[index + 1] == 0x90))
                    break;
                #region Base parameters
                int position;
                bool twoByte;
                byte[] displacement;
                byte[] immediateOperand;
                CodeLine cd;
                position = index;
                int operandSize = 4;
                byte FPU = 0;
                List<byte> prefix = new List<byte>();
                byte opcode;
                bool HasModRm = false;
                bool HasSib = false;
                byte modRM = 0xff;
                byte opcodeExtension = 0xFF;
                List<byte[]> operands = new List<byte[]>();
                #endregion
                #region Prefixes F0h, F2h, F3h, 66h, 67h, D8h-DFh, 2Eh, 36h, 3Eh, 26h, 64h and 65h
                while (buffer[index] == 0xF0 ||
                       buffer[index] == 0xF2 ||
                       buffer[index] == 0xF3 ||
                      (buffer[index] & 0xFC) == 0x64 ||
                      (buffer[index] & 0xF8) == 0xD8 ||
                      (buffer[index] & 0x7E) == 0x62)
                {
                    if (buffer[index] == 0x66)
                    {
                        operandSize = 2;
                    }
                    else if ((buffer[index] & 0xF8) == 0xD8)
                    {
                        // Floating point instructions
                        FPU = buffer[index];
                        index++;
                        break;
                    }
                    prefix.Add(buffer[index]);
                    index++;
                }
                #endregion
                #region Two-byte opcode byte
                twoByte = false;
                if (buffer[index] == 0x0F)
                {
                    twoByte = true;
                    index++;
                }
                #endregion
                #region Opcode byte
                opcode = buffer[index];
                index++;
                #endregion
                #region Get mod R/M byte
                if (FPU > 0)
                {
                    if ((opcode & 0xC0) != 0xC0)
                    {
                        HasModRm = true;
                        modRM = (byte)opcode;
                    }
                    else
                    {
                        opcodeExtension = (byte)opcode;
                    }
                }
                else if (!twoByte)
                {
                    if ((opcode & 0xC4) == 0x00 ||
                       (opcode & 0xF4) == 0x60 && ((opcode & 0x0A) == 0x02 || (opcode & 0x09) == 0x9) ||
                       (opcode & 0xF0) == 0x80 ||
                       (opcode & 0xF8) == 0xC0 && (opcode & 0x0E) != 0x02 ||
                       (opcode & 0xFC) == 0xD0 ||
                       (opcode & 0xF6) == 0xF6)
                    {
                        HasModRm = true;
                        modRM = buffer[index];
                        index++;
                    }
                }
                else
                {
                    if ((opcode & 0xF0) == 0x00 && (opcode & 0x0F) >= 0x04 && (opcode & 0x0D) != 0x0D ||
                       (opcode & 0xF0) == 0x30 ||
                       opcode == 0x77 ||
                       (opcode & 0xF0) == 0x80 ||
                       (opcode & 0xF0) == 0xA0 && (opcode & 0x07) <= 0x02 ||
                       (opcode & 0xF8) == 0xC8)
                    {
                        // No mod R/M byte 
                    }
                    else
                    {
                        HasModRm = true;
                        modRM = buffer[index];
                        index++;
                    }
                }
                #endregion
                #region Scale_Index_Base
                byte Scale_Index_Base = 0xff;
                if ((modRM & 0x07) == 0x04 &&
                   (modRM & 0xC0) != 0xC0)
                {
                    HasSib = true;
                    Scale_Index_Base = buffer[index];
                    index++;   // Scale_Index_Base
                }
                #endregion
                #region Displacement
                int displacementLength = 0;
                if ((modRM & 0x07) != 0x07)
                {
                    if ((modRM & 0xC5) == 0x05) displacementLength = 4;
                    else { }
                } // Dword displacementLength, no base 
                if ((modRM & 0xC0) == 0x40) displacementLength = 1;   // Byte displacementLength 
                if ((modRM & 0xC0) == 0x80) displacementLength = 4;   // Dword displacementLength 
                displacement = null;
                string function = "";
                if (displacementLength > 0)
                {
                    displacement = new byte[displacementLength];
                    Buffer.BlockCopy(buffer, index, displacement, 0, displacementLength);
                    if (displacementLength == 4)
                    {
                        function = FindFunctionName(displacement);
                    }
                }
                index += displacementLength;
                #endregion
                #region Immediate operande
                immediateOperand = null;
                if (FPU > 0)
                {
                    // Can't have immediate immediateOperand 
                }
                else if (!twoByte)
                {
                    #region one byte instruction
                    if ((opcode & 0xC7) == 0x04 ||
                       (opcode & 0xFE) == 0x6A ||   // PUSH/POP/IMUL 
                       (opcode & 0xF0) == 0x70 ||   // Jcc 
                       opcode == 0x80 ||
                       opcode == 0x83 ||
                       (opcode & 0xFD) == 0xA0 ||   // MOV 
                       opcode == 0xA8 ||            // TEST 
                       (opcode & 0xF8) == 0xB0 ||   // MOV
                       (opcode & 0xFE) == 0xC0 ||   // RCL 
                       opcode == 0xC6 ||            // MOV 
                       opcode == 0xCD ||            // INT 
                       (opcode & 0xFE) == 0xD4 ||   // AAD/AAM 
                       (opcode & 0xF8) == 0xE0 ||   // LOOP/JCXZ 
                       opcode == 0xEB ||
                       opcode == 0xF6 && (modRM & 0x30) == 0x00)   // TEST 
                    {
                        immediateOperand = new byte[1];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += 1;
                    }
                    else if ((opcode & 0xF7) == 0xC2)
                    {
                        immediateOperand = new byte[2];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += 2;   // RET 
                    }
                    else if ((opcode & 0xFC) == 0x80 ||
                            (opcode & 0xC7) == 0x05 ||
                            (opcode & 0xF8) == 0xB8 ||
                            (opcode & 0xFE) == 0xE8 ||      // CALL/Jcc 
                            (opcode & 0xFE) == 0x68 ||
                            (opcode & 0xFC) == 0xA0 ||
                            (opcode & 0xEE) == 0xA8 ||
                            opcode == 0xC7 ||
                            opcode == 0xF7 && (modRM & 0x30) == 0x00)
                    {
                        immediateOperand = new byte[operandSize];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += operandSize;
                    }
                    #endregion
                }
                else
                {
                    #region two byte instructions
                    if (opcode == 0xBA ||            // BT 
                      opcode == 0x0F ||            // 3DNow! 
                      (opcode & 0xFC) == 0x70 ||   // PSLLW 
                      (opcode & 0xF7) == 0xA4 ||   // SHLD 
                      opcode == 0xC2 ||
                      opcode == 0xC4 ||
                      opcode == 0xC5 ||
                      opcode == 0xC6)
                    {
                        immediateOperand = new byte[1];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += 1;
                    }
                    else if ((opcode & 0xF0) == 0x80)
                    {
                        immediateOperand = new byte[operandSize];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += operandSize;   // Jcc -u
                    }
                    #endregion
                }
                if ((immediateOperand != null) && (immediateOperand.Length == 4))
                {
                    {
                        try
                        {
                            function = FindFunctionName(immediateOperand);
                        }
                        catch { }
                    }
                }
                #endregion
                #region Identify instruction
                Instruction ins = null;
                int regNum = 0;
                if (position == 0x33e)
                { }
                int selectedIndex = 0;
                currentCode = opcode;
                if (twoByte)
                    opcode = (byte)(0x0F * 256 + opcode);
                try
                {
                    ins = GetInstructionFromOpCode(prefix, FPU, opcode, modRM, out regNum, out selectedIndex);
                }
                catch { }
                cd = new CodeLine(position, codeBase + imageBase, ins, prefix, FPU, opcode, HasModRm, modRM, Scale_Index_Base, displacement, immediateOperand, function, opcodeExtension, regNum, selectedIndex, position);
                code.Add(cd);
                try
                {
                    lines.Add(position + codeBase, cd);
                }
                catch { return count; }
                count++;
                AddReference(displacement, function);
                AddReference(immediateOperand, function);
                #endregion
                #region Jump or call
                if ((cd.ToString().Contains("CALL") || cd.ToString().Contains("J")) & ((displacement != null) || (immediateOperand != null)))
                {
                    int[] jumpShort = new int[] { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0xE3, 0xEB };
                    int[] jumplong = new int[] { 0xE9, 0xEA, 0xFF, 0x9A, 0xE8 };
                    CallAdress adress = null;
                    if (Array.IndexOf(jumpShort, currentCode) >= 0)
                    {
                        uint x = cd.immediateOperand[0];
                        if ((x & 0x80) == 0x80)// negative
                        {
                            x += 0xFFFFFF00;
                        }
                        int y = (int)x;

                        adress = new CallAdress(position, (c.codeBase + index + y), c.codeBase, false, cd);
                    }
                    if (Array.IndexOf(jumplong, currentCode) >= 0)
                    {
                        #region long call
                        switch (currentCode)
                        {
                            case 0x9A://Call ptr
                                if (cd.immediateOperand.Length == 4)
                                    adress = new CallAdress(position, c.codeBase + index + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, false, cd);
                                break;
                            case 0xE9://JMP rel16:32
                                if (cd.immediateOperand.Length == 4)
                                    adress = new CallAdress(position, c.codeBase + index + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, false, cd);
                                break;
                            case 0xE8://call rel
                                if (cd.immediateOperand.Length == 4)
                                    adress = new CallAdress(position, c.codeBase + index + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, true, cd);
                                break;
                            case 0xFF:// call ms/m ou Jmp
                                if (cd.displacement.Length == 4)
                                    adress = new CallAdress(position, BitConverter.ToInt32(cd.displacement, 0), c.codeBase, true, cd);
                                break;
                            default:
                                if (cd.displacement.Length == 40)
                                    adress = new CallAdress(position, c.codeBase + index + BitConverter.ToInt32(cd.displacement, 0), c.codeBase, true, cd);
                                break;
                        }
                        #endregion
                    }
                    if (twoByte & ((currentCode & 0x80) == 0x80))
                    {
                        if (cd.immediateOperand.Length == 4)
                            adress = new CallAdress(position, c.codeBase + index + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, true, cd);
                    }
                    if (adress != null)
                    {
                        //    references.Add(adress.PositionOfStructureInFile, adress.startAddress);
                        if (Array.IndexOf(blocAddress.ToArray(), adress.startAddress) < 0)
                        {
                            calls.Enqueue(adress);
                            blocAddress.Add(adress.startAddress);
                            subroutines.Add(adress);
                        }
                        #region Jump
                        if (cd.ToString().Contains("JMP"))
                        {
                            /*         c.setEnd(index);
                                     try
                                     {
                                         blocs.Add(c.startAddress, block);
                                     }
                                     catch { }
                                     Trace.WriteLine(" End block JMP " + currentCode.ToString("x2") + " " + index.ToString("x8"));
                                     if (index == 0xfa5)
                                     {
                                     }
                                     return count;*/
                        }
                        #endregion
                    }
                }
                #region test RET for endIAT of block
                switch (currentCode)
                {
                    case 0xC2:
                    case 0xC3:
                    //               case 0xC9:
                    case 0xCA:
                    case 0xCB:
                    case 0xCF:
                        Bloc b = new Bloc(start, position);
                        blocList.Add(b);
                        c.setEnd(index);
                        Trace.WriteLine(" End block RET " + currentCode.ToString("x2") + " " + index.ToString("x8"));
                        return count;
                }
                #endregion
                #endregion
            }
            c.endAddress = index;
            Trace.WriteLine(" End block EXIT " + currentCode.ToString("x2") + " " + index.ToString("x8"));
            return count;
        }
        private int ParseInstructionsFollowCalls32(BitStreamReader sw, CallAdress c, string blocName)
        {
            //http://wiki.osdev.org/X86_Instruction_Encoding
            int count = 0;
            sw.Position = (int)c.realAddress;
            long start = (int)c.realAddress;
            byte currentCode = sw.ReadByte();
         //   Trace.Write("Start " + index.ToString("x8"));
            while ((sw.Position < exe.NT_Headers.OptionalHeader.BaseOfCode + exe.NT_Headers.OptionalHeader.SizeOfCode) & (currentCode != 0xCC))
            {
                foreach (Bloc b in blocList)
                {
                    if (b.Contains(sw.Position + codeBase))
                        return count;
                }
                 #region Base parameters
                int position;
                bool twoByte;
                byte[] displacement;
                byte[] immediateOperand;
                CodeLine cd;
                position = sw.Position;
                int operandSize = 4;
                byte FPU = 0;
                List<byte> prefix = new List<byte>();
                byte opcode;
                bool HasModRm = false;
                bool HasSib = false;
                byte modRM = 0xff;
                byte opcodeExtension = 0xFF;
                List<byte[]> operands = new List<byte[]>();
                #endregion
                #region Prefixes F0h, F2h, F3h, 66h, 67h, D8h-DFh, 2Eh, 36h, 3Eh, 26h, 64h and 65h
                while (currentCode == 0xF0 ||
                       currentCode == 0xF2 ||
                       currentCode == 0xF3 ||
                      (currentCode & 0xFC) == 0x64 ||
                      (currentCode & 0xF8) == 0xD8 ||
                      (currentCode & 0x7E) == 0x62)
                {
                    if (currentCode == 0x66)
                    {
                        operandSize = 2;
                    }
                    else if ((currentCode & 0xF8) == 0xD8)
                    {
                        // Floating point instructions
                        FPU = currentCode;
                        currentCode = sw.ReadByte();
                        break;
                    }
                    prefix.Add(currentCode);
                    currentCode = sw.ReadByte();
                }
                #endregion
                #region Two-byte opcode byte
                twoByte = false;
                if (currentCode == 0x0F)
                {
                    twoByte = true;
                    currentCode = sw.ReadByte();
                }
                #endregion
                #region Opcode byte
                opcode = currentCode;
                currentCode = sw.ReadByte();
                #endregion
                #region Get mod R/M byte
                if (FPU > 0)
                {
                    if ((opcode & 0xC0) != 0xC0)
                    {
                        HasModRm = true;
                        modRM = (byte)opcode;
                    }
                    else
                    {
                        opcodeExtension = (byte)opcode;
                    }
                }
                else if (!twoByte)
                {
                    if ((opcode & 0xC4) == 0x00 ||
                       (opcode & 0xF4) == 0x60 && ((opcode & 0x0A) == 0x02 || (opcode & 0x09) == 0x9) ||
                       (opcode & 0xF0) == 0x80 ||
                       (opcode & 0xF8) == 0xC0 && (opcode & 0x0E) != 0x02 ||
                       (opcode & 0xFC) == 0xD0 ||
                       (opcode & 0xF6) == 0xF6)
                    {
                        HasModRm = true;
                        modRM = currentCode;
                        currentCode = sw.ReadByte();
                    }
                }
                else
                {
                    if ((opcode & 0xF0) == 0x00 && (opcode & 0x0F) >= 0x04 && (opcode & 0x0D) != 0x0D ||
                       (opcode & 0xF0) == 0x30 ||
                       opcode == 0x77 ||
                       (opcode & 0xF0) == 0x80 ||
                       (opcode & 0xF0) == 0xA0 && (opcode & 0x07) <= 0x02 ||
                       (opcode & 0xF8) == 0xC8)
                    {
                        // No mod R/M byte 
                    }
                    else
                    {
                        HasModRm = true;
                        modRM = currentCode;
                        currentCode = sw.ReadByte();
                    }
                }
                #endregion
                #region Scale_Index_Base
                byte Scale_Index_Base = 0xff;
                if ((modRM & 0x07) == 0x04 &&
                   (modRM & 0xC0) != 0xC0)
                {
                    HasSib = true;
                    Scale_Index_Base = currentCode;
                    currentCode = sw.ReadByte();   // Scale_Index_Base
                }
                #endregion
                #region Displacement
                int displacementLength = 0;
                if ((modRM & 0x07) != 0x07)
                {
                    if ((modRM & 0xC5) == 0x05) displacementLength = 4;
                    else { }
                } // Dword displacementLength, no base 
                if ((modRM & 0xC0) == 0x40) displacementLength = 1;   // Byte displacementLength 
                if ((modRM & 0xC0) == 0x80) displacementLength = 4;   // Dword displacementLength 
                displacement = null;
                string function = "";
                if (displacementLength > 0)
                {
                    displacement = sw.ReadBytes(displacementLength);
                    if (displacementLength == 4)
                    {
                        function = FindFunctionName(displacement);
                    }
                }
                #endregion
                #region Immediate operande
                immediateOperand = null;
                if (FPU > 0)
                {
                    // Can't have immediate immediateOperand 
                }
                else if (!twoByte)
                {
                    #region one byte instruction
                    if ((opcode & 0xC7) == 0x04 ||
                       (opcode & 0xFE) == 0x6A ||   // PUSH/POP/IMUL 
                       (opcode & 0xF0) == 0x70 ||   // Jcc 
                       opcode == 0x80 ||
                       opcode == 0x83 ||
                       (opcode & 0xFD) == 0xA0 ||   // MOV 
                       opcode == 0xA8 ||            // TEST 
                       (opcode & 0xF8) == 0xB0 ||   // MOV
                       (opcode & 0xFE) == 0xC0 ||   // RCL 
                       opcode == 0xC6 ||            // MOV 
                       opcode == 0xCD ||            // INT 
                       (opcode & 0xFE) == 0xD4 ||   // AAD/AAM 
                       (opcode & 0xF8) == 0xE0 ||   // LOOP/JCXZ 
                       opcode == 0xEB ||
                       opcode == 0xF6 && (modRM & 0x30) == 0x00)   // TEST 
                    {
                        immediateOperand = sw.ReadBytes(1);
                    }
                    else if ((opcode & 0xF7) == 0xC2)
                    {
                        immediateOperand = sw.ReadBytes(2);   // RET 
                    }
                    else if ((opcode & 0xFC) == 0x80 ||
                            (opcode & 0xC7) == 0x05 ||
                            (opcode & 0xF8) == 0xB8 ||
                            (opcode & 0xFE) == 0xE8 ||      // CALL/Jcc 
                            (opcode & 0xFE) == 0x68 ||
                            (opcode & 0xFC) == 0xA0 ||
                            (opcode & 0xEE) == 0xA8 ||
                            opcode == 0xC7 ||
                            opcode == 0xF7 && (modRM & 0x30) == 0x00)
                    {
                        immediateOperand = sw.ReadBytes(4); 
                    }
                    #endregion
                }
                else
                {
                    #region two byte instructions
                    if (opcode == 0xBA ||            // BT 
                      opcode == 0x0F ||            // 3DNow! 
                      (opcode & 0xFC) == 0x70 ||   // PSLLW 
                      (opcode & 0xF7) == 0xA4 ||   // SHLD 
                      opcode == 0xC2 ||
                      opcode == 0xC4 ||
                      opcode == 0xC5 ||
                      opcode == 0xC6)
                    {
                        immediateOperand = sw.ReadBytes(1);
                    }
                    else if ((opcode & 0xF0) == 0x80)
                    {
                        immediateOperand =sw.ReadBytes(operandSize);
                    }
                    #endregion
                }
                if ((immediateOperand != null) && (immediateOperand.Length == 4))
                {
                    {
                        try
                        {
                            function = FindFunctionName(immediateOperand);
                        }
                        catch { }
                    }
                }
                #endregion
                #region Identify instruction
                Instruction ins = null;
                int regNum = 0;
                if (position == 0x33e)
                { }
                int selectedIndex = 0;
                currentCode = opcode;
                if (twoByte)
                    opcode = (byte)(0x0F * 256 + opcode);
                try
                {
                    ins = GetInstructionFromOpCode(prefix, FPU, opcode, modRM, out regNum, out selectedIndex);
                }
                catch { }
                cd = new CodeLine(position, codeBase + imageBase, ins, prefix, FPU, opcode, HasModRm, modRM, Scale_Index_Base, displacement, immediateOperand, function, opcodeExtension, regNum, selectedIndex, position);
                code.Add(cd);
                try
                {
                    lines.Add(position + codeBase, cd);
                }
                catch { return count; }
                count++;
                AddReference(displacement, function);
                AddReference(immediateOperand, function);
                #endregion
                #region Jump or call
                if ((cd.ToString().Contains("CALL") || cd.ToString().Contains("J")) & ((displacement != null) || (immediateOperand != null)))
                {
                    int[] jumpShort = new int[] { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0xE3, 0xEB };
                    int[] jumplong = new int[] { 0xE9, 0xEA, 0xFF, 0x9A, 0xE8 };
                    CallAdress adress = null;
                    if (Array.IndexOf(jumpShort, currentCode) >= 0)
                    {
                        uint x = cd.immediateOperand[0];
                        if ((x & 0x80) == 0x80)// negative
                        {
                            x += 0xFFFFFF00;
                        }
                        int y = (int)x;

                        adress = new CallAdress(position, (c.codeBase + sw.Position + y), c.codeBase, false, cd);
                    }
                    if (Array.IndexOf(jumplong, currentCode) >= 0)
                    {
                        #region long call
                        switch (currentCode)
                        {
                            case 0x9A://Call ptr
                                if (cd.immediateOperand.Length == 4)
                                    adress = new CallAdress(position, c.codeBase + sw.Position + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, false, cd);
                                break;
                            case 0xE9://JMP rel16:32
                                if (cd.immediateOperand.Length == 4)
                                    adress = new CallAdress(position, c.codeBase + sw.Position + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, false, cd);
                                break;
                            case 0xE8://call rel
                                if (cd.immediateOperand.Length == 4)
                                    adress = new CallAdress(position, c.codeBase + sw.Position + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, true, cd);
                                break;
                            case 0xFF:// call ms/m ou Jmp
                                if (cd.displacement.Length == 4)
                                    adress = new CallAdress(position, BitConverter.ToInt32(cd.displacement, 0), c.codeBase, true, cd);
                                break;
                            default:
                                if (cd.displacement.Length == 40)
                                    adress = new CallAdress(position, c.codeBase + sw.Position + BitConverter.ToInt32(cd.displacement, 0), c.codeBase, true, cd);
                                break;
                        }
                        #endregion
                    }
                    if (twoByte & ((currentCode & 0x80) == 0x80))
                    {
                        if (cd.immediateOperand.Length == 4)
                            adress = new CallAdress(position, c.codeBase + sw.Position + BitConverter.ToInt32(cd.immediateOperand, 0), c.codeBase, true, cd);
                    }
                    if (adress != null)
                    {
                        //    references.Add(adress.PositionOfStructureInFile, adress.startAddress);
                        if (Array.IndexOf(blocAddress.ToArray(), adress.startAddress) < 0)
                        {
                            calls.Enqueue(adress);
                            blocAddress.Add(adress.startAddress);
                            subroutines.Add(adress);
                        }
                        #region Jump
                        if (cd.ToString().Contains("JMP"))
                        {
                            /*         c.setEnd(index);
                                     try
                                     {
                                         blocs.Add(c.startAddress, block);
                                     }
                                     catch { }
                                     Trace.WriteLine(" End block JMP " + currentCode.ToString("x2") + " " + index.ToString("x8"));
                                     if (index == 0xfa5)
                                     {
                                     }
                                     return count;*/
                        }
                        #endregion
                    }
                }
                #region test RET for endIAT of block
                switch (currentCode)
                {
                    case 0xC2:
                    case 0xC3:
                    //               case 0xC9:
                    case 0xCA:
                    case 0xCB:
                    case 0xCF:
                        Bloc b = new Bloc(start, position);
                        blocList.Add(b);
                        c.setEnd(sw.Position);
                        Trace.WriteLine(" End block RET " + currentCode.ToString("x2") + " " + sw.Position.ToString("x8"));
                        return count;
                }
                #endregion
                #endregion
            currentCode = sw.ReadByte();
            }
            c.endAddress = sw.Position;

            Trace.WriteLine(" End block EXIT " + currentCode.ToString("x2") + " " + sw.Position.ToString("x8"));
            return count;
        }
        public static List<CodeLine> ParseBloc(byte[] buffer, long positionInFile)
        {
            List<CodeLine> code = new List<CodeLine>();
            BitStreamReader sw = new BitStreamReader(buffer, true);
            StreamWriter swr = new StreamWriter("Assem.txt");
            while (sw.Position < buffer.Length)
            {
                swr.Write(sw.Position.ToString("x2") + "\t");
                CodeLine dl = new CodeLine(sw, positionInFile);
                code.Add(dl);
                swr.WriteLine(dl.ToString());
            }
            swr.Close();

            return code;
        }
        public static List<CodeLine> ParseStub(byte[] buffer, long position)
        {
            //http://wiki.osdev.org/X86_Instruction_Encoding
            ReadOpCode();
            List<CodeLine> code = new List<CodeLine>();
            int index = 0;
            long start = index;
            int currentCode = buffer[index];
            Trace.Write("Start " + index.ToString("x8"));
            while ((index < buffer.Length))
            {
                int startInstruction = index;
                bool twoByte;
                byte[] displacement;
                byte[] immediateOperand;
                CodeLine cd;
                #region
                int operandSize = 4;
                byte FPU = 0;
                List<byte> prefix = new List<byte>();
                int opcode;
                byte modRM = 0xff;
                byte opcodeExtension = 0xFF;
                List<byte[]> operands = new List<byte[]>();
                #endregion
                #region Prefixes F0h, F2h, F3h, 66h, 67h, D8h-DFh, 2Eh, 36h, 3Eh, 26h, 64h and 65h
                while (buffer[index] == 0xF0 ||
                       buffer[index] == 0xF2 ||
                       buffer[index] == 0xF3 ||
                      (buffer[index] & 0xFC) == 0x64 ||
                      (buffer[index] & 0xF8) == 0xD8 ||
                      (buffer[index] & 0x7E) == 0x62)
                {
                    if (buffer[index] == 0x66)
                    {
                        operandSize = 2;
                    }
                    else if ((buffer[index] & 0xF8) == 0xD8)
                    {
                        // Floating point instructions
                        FPU = buffer[index];
                        index++;
                        break;
                    }
                    prefix.Add(buffer[index]);
                    index++;
                }
                #endregion
                #region Two-byte opcode byte
                twoByte = false;
                if (buffer[index] == 0x0F)
                {
                    twoByte = true;
                    index++;
                }
                #endregion
                #region Opcode byte
                opcode = buffer[index];
                #endregion
                #region Get mod R/M byte
                if (FPU > 0)
                {
                    if ((opcode & 0xC0) != 0xC0)
                    {
                        modRM = (byte)opcode;
                    }
                    else
                    {
                        opcodeExtension = (byte)opcode;
                    }
                }
                else if (!twoByte)
                {
                    if ((opcode & 0xC4) == 0x00 ||
                       (opcode & 0xF4) == 0x60 && ((opcode & 0x0A) == 0x02 || (opcode & 0x09) == 0x9) ||
                       (opcode & 0xF0) == 0x80 ||
                       (opcode & 0xF8) == 0xC0 && (opcode & 0x0E) != 0x02 ||
                       (opcode & 0xFC) == 0xD0 ||
                       (opcode & 0xF6) == 0xF6)
                    {
                        modRM = buffer[index];
                        index++;
                    }
                }
                else
                {
                    if ((opcode & 0xF0) == 0x00 && (opcode & 0x0F) >= 0x04 && (opcode & 0x0D) != 0x0D ||
                       (opcode & 0xF0) == 0x30 ||
                       opcode == 0x77 ||
                       (opcode & 0xF0) == 0x80 ||
                       (opcode & 0xF0) == 0xA0 && (opcode & 0x07) <= 0x02 ||
                       (opcode & 0xF8) == 0xC8)
                    {
                        // No mod R/M byte 
                    }
                    else
                    {
                        modRM = buffer[index];
                        index++;
                    }
                }
                #endregion
                #region Scale_Index_Base
                int Scale_Index_Base = -1;
                if ((modRM & 0x07) == 0x04 &&
                   (modRM & 0xC0) != 0xC0)
                {
                    Scale_Index_Base = buffer[index];
                    index++;   // Scale_Index_Base
                }
                #endregion
                #region Displacement
                int displacementLength = 0;
                if ((modRM & 0x07) != 0x07)
                {
                    if ((modRM & 0xC5) == 0x05) displacementLength = 4;
                    else { }
                } // Dword displacementLength, no base 
                if ((modRM & 0xC0) == 0x40) displacementLength = 1;   // Byte displacementLength 
                if ((modRM & 0xC0) == 0x80) displacementLength = 4;   // Dword displacementLength 
                displacement = null;
                index += displacementLength;
                #endregion
                #region Immediate operande
                immediateOperand = null;
                index++;
                if (FPU > 0)
                {
                    // Can't have immediate immediateOperand 
                }
                else if (!twoByte)
                {
                    #region one byte instruction
                    if ((opcode == 0x21) || (opcode == 0xb4) || (opcode & 0xC7) == 0x04 ||
                       (opcode & 0xFE) == 0x6A ||   // PUSH/POP/IMUL 
                       (opcode & 0xF0) == 0x70 ||   // Jcc 
                       opcode == 0x80 ||
                       opcode == 0x83 ||
                       (opcode & 0xFD) == 0xA0 ||   // MOV 
                       opcode == 0xA8 ||            // TEST 
                       (opcode & 0xF8) == 0xB0 ||   // MOV
                       (opcode & 0xFE) == 0xC0 ||   // RCL 
                       opcode == 0xC6 ||            // MOV 
                       opcode == 0xCD ||            // INT 
                       (opcode & 0xFE) == 0xD4 ||   // AAD/AAM 
                       (opcode & 0xF8) == 0xE0 ||   // LOOP/JCXZ 
                       opcode == 0xEB ||
                       opcode == 0xF6 && (modRM & 0x30) == 0x00)   // TEST 
                    {
                        immediateOperand = new byte[1];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += 1;
                    }
                    else if ((opcode == 0xba) || (opcode == 0xb8) || (opcode & 0xF7) == 0xC2)
                    {
                        immediateOperand = new byte[2];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += 2;   // RET 
                    }
                    else if ((opcode & 0xFC) == 0x80 ||
                            (opcode & 0xC7) == 0x05 ||
                            (opcode & 0xF8) == 0xB8 ||
                            (opcode & 0xFE) == 0xE8 ||      // CALL/Jcc 
                            (opcode & 0xFE) == 0x68 ||
                            (opcode & 0xFC) == 0xA0 ||
                            (opcode & 0xEE) == 0xA8 ||
                            opcode == 0xC7 ||
                            opcode == 0xF7 && (modRM & 0x30) == 0x00)
                    {
                        immediateOperand = new byte[operandSize];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += operandSize;
                    }
                    #endregion
                }
                else
                {
                    #region two byte instructions
                    if (opcode == 0xBA ||            // BT 
                      opcode == 0x0F ||            // 3DNow! 
                      (opcode & 0xFC) == 0x70 ||   // PSLLW 
                      (opcode & 0xF7) == 0xA4 ||   // SHLD 
                      opcode == 0xC2 ||
                      opcode == 0xC4 ||
                      opcode == 0xC5 ||
                      opcode == 0xC6)
                    {
                        immediateOperand = new byte[1];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += 1;
                    }
                    else if ((opcode & 0xF0) == 0x80)
                    {
                        immediateOperand = new byte[operandSize];
                        Buffer.BlockCopy(buffer, index, immediateOperand, 0, immediateOperand.Length);
                        index += operandSize;   // Jcc -u
                    }
                    #endregion
                }
                #endregion
                #region Identify instruction
                Instruction ins = null;
                int regNum = 0;
                int selectedIndex = 0;
                currentCode = opcode;
                if (twoByte)
                    opcode = 0x0F * 256 + opcode;
                try
                {
                    ins = GetInstructionFromOpCode(prefix, FPU, opcode, modRM, out regNum, out selectedIndex);
                }
                catch { }
                cd = new CodeLine(ins, prefix, FPU, modRM, displacement, immediateOperand, opcodeExtension, position + startInstruction);
                code.Add(cd);
                if (cd.BinaryCode == "cd21")
                {
                    if (code[code.Count - 2].BinaryCode.EndsWith("4c"))
                    {
                        //Exit
                        return code;
                    }
                }
                #endregion
            }
            return code;
        }
        private void AddReference(byte[] displace, string function)
        {
            if (displace != null)
                if ((displace.Length == 4) & (function == ""))
                {
                    byte[] k = BitConverter.GetBytes((int)exe.NT_Headers.OptionalHeader.ImageBase);
                    byte[] j = BitConverter.GetBytes((int)exe.NT_Headers.OptionalHeader.BaseOfData);
                    if (displace[3] == k[3])
                    {
                        int x = BitConverter.ToInt32(displace, 0);
                        {
                            if (Array.IndexOf(references.ToArray(), x) < 0)
                                references.Add(x);
                        }
                    }
                }
        }
        private string FindFunctionName(byte[] displace)
        {
            string function = "";
            if (exe.IAT != null)
            {
                int d = BitConverter.ToInt32(displace, 0);
                foreach (IMAGE_IMPORT_DIRECTORY_IAT_ENTRY imt in exe.IAT.IATEntries)
                {
                    if ((imt.Address + exe.NT_Headers.OptionalHeader.BaseOfCode - exe.NT_Headers.OptionalHeader.SizeOfHeaders) == (d & 0xFFFF))
                    {
                        function = imt.Name;
                        break;
                    }
                }
            }
            return function;
        }
        public static Instruction GetInstructionFromOpCode(List<byte> prefix, int FPU, int cod, byte modRm, out int regNum, out int selectedEntry)
        {
            //immediate reg instructions
            regNum = -1;
            selectedEntry = 0;
            int codSt = cod;
            if (FPU > 0)
            {
                Instruction ins = instructions[FPU];
                try
                {
                    selectedEntry = ins.subInstructions[cod];
                }
                catch
                {
                    int ext = (cod & 0x38) >> 3;// 1C ?
                    selectedEntry = ins.instByextension[ext];
                }
                switch (FPU)
                {
                    case 0xD8:
                    case 0xD9:
                    case 0xDA:
                    case 0xDC:
                    case 0xDD:
                    case 0xDE:
                    case 0xDF:
                        if ((cod & 0x08) == 0x08)
                            regNum = cod & 0x07;
                        break;
                    case 0xDB:
                        if ((cod & 0xE8) == 0xE8)
                            regNum = cod & 0x07;
                        break;
                }
                return ins;
            }
            else
            {
                if (cod <= 0xff)
                {
                    switch (cod & 0xF0)
                    {
                        case 0x90:
                            if ((cod & 0x0F) < 0x8)
                            {
                                codSt = cod & 0xF8;
                                regNum = cod & 0x07;
                            }
                            break;
                        case 0x40:
                        case 0x50:
                        case 0xB0:
                            codSt = cod & 0xF8;
                            regNum = cod & 0x07;
                            break;
                    }
                    switch (cod & 0xF8)
                    {
                        case 0xB8:
                            codSt = cod & 0xF8;
                            regNum = cod & 0x07;
                            break;
                    }
                }
            }
            Instruction inst = instructions[codSt];
            if (inst.HasSubInstructions)
                try
                {
                    selectedEntry = inst.subInstructions[cod];
                }
                catch
                {
                }
            if (inst.HasExtensions)
                if (modRm != 0xff)
                {
                    int ext = (modRm & 0x38) >> 3;
                    try
                    {
                        selectedEntry = inst.instByextension[ext];
                    }
                    catch { }
                }
            return inst;
        }
    }
    public class Label
    {
        public long offset;
        public string label;
        public Label(long o, string l)
        {
            offset = o;
            label = l;
        }
    }
    public class CodeLine : IMAGE_BASE_DATA
    {
        private Instruction instruction;
        public string Label
        {
            get { return label; }
            set { label = value; }
        }
        public Instruction Instruction
        {
            get { return instruction; }
            set { instruction = value; }
        }
        public string Addressing_Mode
        {
            get { return addressing_Mode; }
            set { addressing_Mode = value; }
        }
        public bool TwoByteInstruction
        {
            get { return Two_Byte_Instruction == 0x0F; }
        }
        public List<byte> prefix = new List<byte>();
        public byte Floating_Point_Préfix = 0;
        public byte Two_Byte_Instruction = 0;
        public byte modRM = 0xff;
        public byte Scale_Index_Base = 0xff;
        string addressing_Mode;
        int selectedEntry;
        int rightSyntax;
        private string label;
        private byte opCode;
        Direction direction;
        WordSize wordSize;
        private bool isJumpOrCall
        {
            get { return (ToString().Contains("CALL")) || (ToString().Contains("J")); }
        }

        public byte OpCode
        {
            get { return opCode; }
            set { opCode = value; }
        }
        public byte[] immediateOperand;
        public byte[] displacement;
        public int displaceInt;
        public int imOperandInt;
        public long refersTo;
        public byte[] data;
        public long dtaInt;
        public string text;
        private bool hasModRM;
        private List<string> operandes;

        public bool HasModRM
        {
            get { return modRM!=0xff; }
        }
        public List<string> Operandes
        {
            get { return operandes; }
            set { operandes = value; }
        }
        #region données
        static string[] reg32ascii = new string[] { "eax", "ecx", "edx", "ebx", "esp", "ebp", "esi", "edi" };
        static string[] reg16ascii = new string[] { "ax", "cx", "dx", "bx", "sp", "bp", "si", "di" };
        static string[] reg8ascii = new string[] { "al", "cl", "dl", "bl", "ah", "ch", "dh", "bh" };
        static string[] regind16ascii = new string[] { "bx+si", "bx+di", "bp+si", "bp+di", "si", "di", "bp", "bx" };
        static string[] regfascii = new string[] { "st(0)", "st(1)", "st(2)", "st(3)", "st(4)", "st(5)", "st(6)", "st(7)" };
        static string[] regmascii = new string[] { "mm0", "mm1", "mm2", "mm3", "mm4", "mm5", "mm6", "mm7" };
        static string[] regxascii = new string[] { "xmm0", "xmm1", "xmm2", "xmm3", "xmm4", "xmm5", "xmm6", "xmm7" };
        static string[] regsascii = new string[] { "es", "cs", "ss", "ds", "fs", "gs", "??", "??" };
        static string[] regcascii = new string[] { "cr0", "cr1", "cr2", "cr3", "cr4", "cr5", "cr6", "cr7" };
        static string[] regdascii = new string[] { "dr0", "dr1", "dr2", "dr3", "dr4", "dr5", "dr6", "dr7" };
        static string[] regtascii = new string[] { "tr0", "tr1", "tr2", "tr3", "tr4", "tr5", "tr6", "tr7" };
        static string[] regzascii = new string[] { "b", "c", "d", "e", "h", "l", "(hl)", "a" };
        static List<byte> opCodePrefix = new List<byte>() { 0xF0, 0xF2, 0xF3, 0x66, 0x67, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDE, 0xDF, 0x2E, 0x36, 0x3E, 0x26, 0x64, 0x65 };
        #endregion
        public CodeLine(BitStreamReader sw, long offset)
        {
            #region Base parameters
            int operandSize = 4;
            byte opcodeExtension = 0xff;
            #endregion
            PositionOfStructureInFile = sw.Position + offset;
            byte currentByte = sw.ReadByte();
            if (currentByte == 0xcd)
            {

            } 
            #region Prefixes F0h, F2h, F3h, 66h, 67h, D8h-DFh, 2Eh, 36h, 3Eh, 26h, 64h and 65h
            while(opCodePrefix.Contains(currentByte))
            {
                if (currentByte == 0x66)
                {
                    //0x66 : 16 bits operands prefix
                    operandSize = 2;
                }
                else if ((currentByte & 0xF8) == 0xD8)
                {
                    // Floating point instructions
                    Floating_Point_Préfix = currentByte;
                    currentByte = sw.ReadByte();
                    break;
                }
                prefix.Add(currentByte);
                currentByte = sw.ReadByte();
            }
            #endregion
            #region Two-byte opcode byte
            if (currentByte == 0x0F)
            {
                Two_Byte_Instruction = currentByte;
                currentByte = sw.ReadByte();
            }
            #endregion
            #region Get Opcode byte
            opCode = currentByte;
            #endregion
            #region Get mod R/M byte
            if (Floating_Point_Préfix > 0)
            {
                if ((opCode & 0xC0) != 0xC0)
                {
                    modRM = (byte)opCode;
                }
                else
                {
                    opcodeExtension = (byte)opCode;
                }
            }
            else if (!TwoByteInstruction)
            {
                if ((opCode & 0xC4) == 0x00 ||
                   (opCode & 0xF4) == 0x60 && ((opCode & 0x0A) == 0x02 || (opCode & 0x09) == 0x9) ||
                   (opCode & 0xF0) == 0x80 ||
                   (opCode & 0xF8) == 0xC0 && (opCode & 0x0E) != 0x02 ||
                   (opCode & 0xFC) == 0xD0 ||
                   (opCode & 0xF6) == 0xF6)
                {
                    modRM = sw.ReadByte();
                }
            }
            else
            {
                if ((opCode & 0xF0) == 0x00 && (opCode & 0x0F) >= 0x04 && (opCode & 0x0D) != 0x0D ||
                   (opCode & 0xF0) == 0x30 ||
                   opCode == 0x77 ||
                   (opCode & 0xF0) == 0x80 ||
                   (opCode & 0xF0) == 0xA0 && (opCode & 0x07) <= 0x02 ||
                   (opCode & 0xF8) == 0xC8)
                {
                    // No mod R/M byte 
                }
                else
                {
                    modRM = sw.ReadByte();
                }
            }
            #endregion
            #region Scale_Index_Base
            if ((modRM & 0x07) == 0x04 && (modRM & 0xC0) != 0xC0)
                Scale_Index_Base = sw.ReadByte(); // Scale_Index_Base
            #endregion
            #region Displacement
            int displacementLength = 0;
            if (HasModRM)
            {
                if ((modRM & 0x07) != 0x07)
                    if ((modRM & 0xC5) == 0x05) displacementLength = 4;// Dword displacementLength, no base
                if ((modRM & 0xC0) == 0x40) displacementLength = 1;   // Byte displacementLength 
                if ((modRM & 0xC0) == 0x80) displacementLength = 4;   // Dword displacementLength 
            }
            displacement = null;
            if (displacementLength > 0)
            {
                displacement = sw.ReadBytes(displacementLength);
            }
            #endregion
            #region Immediate operand
            immediateOperand = null;
            if (Floating_Point_Préfix > 0)
            {
                // Can't have immediate immediateOperand 
            }
            else
            {
                if (TwoByteInstruction)
                {
                    #region two byte instructions
                    if (opCode == 0xBA ||            // BT 
                      opCode == 0x0F ||            // 3DNow! 
                      (opCode & 0xFC) == 0x70 ||   // PSLLW 
                      (opCode & 0xF7) == 0xA4 ||   // SHLD 
                      opCode == 0xC2 ||
                      opCode == 0xC4 ||
                      opCode == 0xC5 ||
                      opCode == 0xC6)
                    {
                        immediateOperand = sw.ReadBytes(1);
                    }
                    else if ((opCode & 0xF0) == 0x80)
                    {
                        immediateOperand = sw.ReadBytes(operandSize);   // Jcc -u
                    }
                    #endregion
                }
                else
                {
                    #region one byte instruction
                    if ((opCode & 0xC7) == 0x04 ||
                       (opCode & 0xFE) == 0x6A ||   // PUSH/POP/IMUL 
                       (opCode & 0xF0) == 0x70 ||   // Jcc 
                       opCode == 0x80 ||
                       opCode == 0x83 ||
                       (opCode & 0xFD) == 0xA0 ||   // MOV 
                       opCode == 0xA8 ||            // TEST 
                       (opCode & 0xF8) == 0xB0 ||   // MOV
                       (opCode & 0xFE) == 0xC0 ||   // RCL 
                       opCode == 0xC6 ||            // MOV 
                       opCode == 0xCD ||            // INT 
                       (opCode & 0xFE) == 0xD4 ||   // AAD/AAM 
                       (opCode & 0xF8) == 0xE0 ||   // LOOP/JCXZ 
                       opCode == 0xEB ||
                       opCode == 0xF6 && (modRM & 0x30) == 0x00)   // TEST 
                    {
                        immediateOperand = sw.ReadBytes(1);
                    }
                    else if ((opCode & 0xF7) == 0xC2)
                    {
                        immediateOperand = sw.ReadBytes(2);   // RET 
                    }
                    else if ((opCode & 0xFC) == 0x80 ||
                            (opCode & 0xC7) == 0x05 ||
                            (opCode & 0xF8) == 0xB8 ||
                            (opCode & 0xFE) == 0xE8 ||      // CALL/Jcc 
                            (opCode & 0xFE) == 0x68 ||
                            (opCode & 0xFC) == 0xA0 ||
                            (opCode & 0xEE) == 0xA8 ||
                            opCode == 0xC7 ||
                            opCode == 0xF7 && (modRM & 0x30) == 0x00)
                    {
                        immediateOperand = sw.ReadBytes(operandSize);
                    }
                    #endregion
                }
            }
            #endregion
            LengthInFile = sw.Position - PositionOfStructureInFile;
            #region Identify instruction
            if (TwoByteInstruction)
                opCode = (byte)(0x0F * 256 + opCode);
            int regNum = 0;
            instruction = Disassemble.GetInstructionFromOpCode(prefix, Floating_Point_Préfix, opCode, modRM, out regNum, out selectedEntry);
            #endregion
            string[] opers = new string[4];
            string pref = "";
            int opcodeLoc = opCode;
            if (Floating_Point_Préfix > 0)
                opcodeLoc = Floating_Point_Préfix;
            operandes = new List<string>();
            //Direction. 1 = Register is Destination, 0 = Register is source.
            int directionBit = (opcodeLoc & 2) >> 1;
            direction = (Direction)directionBit;
            //Operation size. 1 = Word, 0 = byte
            int wordBit = (opcodeLoc & 1);
            wordSize = (WordSize)wordBit;
            #region Parse ModRm
            short imOperandInt = 0;
            if (isJumpOrCall)
            {
                if (displacement != null)
                    displaceInt = operandToInt(displacement);
                if (immediateOperand != null)
                    imOperandInt = (short)operandToInt(immediateOperand);
            }
            if (immediateOperand != null)
            {
                if (isJumpOrCall)
                {
                    if ((BinaryCode.Length % 2) == 1)
                        imOperandInt++;
                }
                else
                {
                    if (immediateOperand.Length == 2)
                    {
                        imOperandInt = (short)operandToInt(immediateOperand);
                        operandes.Add(pref + imOperandInt.ToString("x4"));
                    }
                    else
                    {
                        imOperandInt = immediateOperand[0];
                    }
                }
            }

            if (HasModRM)
            {
                string op = ParseModRm(modRM, Scale_Index_Base, wordBit, directionBit, displacement, immediateOperand, "");
                operandes.Add(op);
            }
            else
            {
                #region Find predefined operandes for one byte instructions with no ModRM
                int first = opcodeLoc >> 4;
                int sec = opcodeLoc & 0x0F;
                if ((first < 4) || (first == 0xE))
                {
                    switch (sec)
                    {
                        case 0x04:
                        case 0x0C:
                            opers[0] = "al";
                            break;
                        case 0x05:
                        case 0x0D:
                            opers[0] = "eax";
                            break;
                    }
                }
                switch (first)
                {
                    case 0x1:
                    case 0x0:
                        if ((sec == 0x06) || (sec == 0x07))
                            opers[0] = "es";
                        if ((sec == 0x0E) || (sec == 0x0F))
                            opers[0] = "ds";
                        break;
                    case 0x4:
                    case 0x5:
                        opers[0] = reg32ascii[sec & 0x7];
                        break;
                    case 0x7:
                        opers[0] = operandToText(immediateOperand);
                        // (sw.Position+immediateOperand[0]).ToString("x2");
                        break;
                    case 0xA:
                        switch (sec)
                        {
                            case 0x00:
                            case 0x08:
                            case 0x0c:
                                opers[0] = "al";
                                break;
                            case 0x02:
                                opers[1] = "al";
                                break;
                            case 0x04:
                                opers[1] = "ah";
                                break;
                            case 0x01:
                            case 0x09:
                            case 0x0D:
                                opers[0] = "eax";
                                break;
                            case 0x03:
                                opers[1] = "eax";
                                break;
                        }
                        break;
                    case 0xB:
                        if (sec < 0x8)
                        {
                            opers[0] = reg8ascii[sec];
                            opers[1] = operandToText(immediateOperand);
                        }
                        else
                        {
                            opers[0] = reg32ascii[sec & 0x7];
                            opers[1] = operandToText(immediateOperand);
                        }
                        break;
                    case 0xC:
                        switch (sec)
                        {
                            case 4:
                                opers[0] = "es";
                             //   operandes.Add("es");
                                break;
                            case 5:
                                opers[0] = "ds";
                              //  operandes.Add("ds");
                                break;
                            case 8:
                            case 9:
                                opers[0] = "ebp";
                              //  operandes.Add("ebp");
                                break;
                            case 0xd:
                                opers[0] = operandToText(immediateOperand);
                                break;
                        }
                        break;
                }
                switch (opcodeLoc)
                {
                    case 0x9E:
                    case 0x9F:
                        opers[0] = "ah";
                //        operandes.Add("ah");
                        break;
                    case 0xD4:
                    case 0xD5:
                        opers[0] = "al";
                        opers[0] = "ah";
                  //      operandes.Add("al");
                   //     operandes.Add("ah");
                        break;
                    case 0xD6:
                    case 0xD7:
                        opers[0] = "al";
                 //       operandes.Add("al");
                        break;
                }

                foreach (string s in opers)
                    if (s != null)
                        operandes.Add(s);
                if (operandes.Count > 0)
                {
                }
                #endregion
            }
            #endregion
            #region select op
            int rightSyntax = 0;
            if (instruction.entries[selectedEntry].syntax[rightSyntax].src.Count > 0)
            {
                string op = "";
                if (instruction.entries[selectedEntry].syntax[rightSyntax].src[0].nr != null)
                    op += instruction.entries[selectedEntry].syntax[rightSyntax].src[0].nr;
            }
            if (instruction.entries[selectedEntry].syntax[rightSyntax].dest.Count > 0)
            {
                string op = "";
                if (instruction.entries[selectedEntry].syntax[rightSyntax].dest[0].nr != null)
                    op += instruction.entries[selectedEntry].syntax[rightSyntax].dest[0].nr;
            }
            #endregion
            #region Opcode extension
            if (opcodeExtension != 0xff)
            {
            }
            #endregion
        }
        private string ParseModRm(byte modRM, byte scale_Index_base, int wordSize, int direction, byte[] displacement, byte[] immediateOp, string function)
        {
            int Mod = (modRM & 0xC0) >> 6;//Register mode
            int reg = (modRM & 0x38) >> 3;//Reg, if wordBit = 0 ; wordBit = 1 double word
            int RorM = (modRM & 0x07);//Register/Memory immediateOperand     
            switch (Mod)
            {
                case 0:
                    addressing_Mode = "Indirect addressing mode";
                    break;
                case 1:
                    addressing_Mode = "Indirect addressing mode -  8-bit displacement added ";
                    break;
                case 2:
                    addressing_Mode = "Indirect addressing mode -  32-bit displacement added ";
                    break;
                case 3:
                    addressing_Mode = "Direct addressing mode reg <-> R/M";
                    break;
            }
            string op = "";
            string Scale_Index_Base = "";
            string disp8 = "";
            string disp32 = "";
            if (displacement != null)
                switch (displacement.Length)
                {
                    case 1:
                        byte disp = displacement[0];
                        if ((disp & 0x80) == 0x80)
                        {
                            disp = (byte)(0xff - disp + 1);
                            disp8 += "-";
                        }
                        else
                            disp8 += "+";
                        disp8 += "0x" + disp.ToString("x2");//+displacementLength 8 bits
                        break;
                    case 4:
                        int a = operandToInt(displacement);
                        if ((a & 8000000000) == 0x8000000000)
                        {
                            a = (int)(0xffffffff - a) + 1;
                        }
                        disp32 = "0x" + a.ToString("x8");
                        if ((function != null) && (function != ""))
                            disp32 = function;
                        break;
                }
            if (scale_Index_base != 0xff)
            {
                int scale = (scale_Index_base & 0xC0) >> 6;
                int index = (scale_Index_base & 0x38) >> 3;
                int sibbase = (scale_Index_base & 0x07);
                //Scale_Index_Base mode
                Scale_Index_Base += reg32ascii[index];
                if (index != sibbase)
                    Scale_Index_Base += "," + reg32ascii[sibbase];
                if (scale > 0)
                    Scale_Index_Base += "*" + scale.ToString();
            }
            if (immediateOperand != null)
            {
            }
            #region
            switch (Mod)
            {
                case 0:
                    switch (RorM)
                    {
                        case 4: op = Scale_Index_Base;
                            break;
                        case 5: op = disp32;
                            break;
                        default:
                            if (wordSize == 0)
                                op = reg8ascii[RorM];
                            else
                                op = reg32ascii[RorM];
                            break;
                    }
                    if (immediateOperand != null)
                        op += "," + operandToText(immediateOperand);
                    else
                        if (direction == 1)
                            op = reg32ascii[reg] + "," + op;
                        else
                            op += "," + reg32ascii[reg];
                    break;
                case 1:
                    switch (RorM)
                    {
                        case 4: op = Scale_Index_Base + disp8;
                            break;
                        default:
                            if (wordSize == 0)
                                op = "[" + reg32ascii[RorM]  + disp8+ "]";
                            else
                                op = "[" + reg32ascii[RorM]  + disp32+ "]";

                            break;
                    }
                    if (immediateOperand != null)
                        if (direction == 1)
                            op += "," + operandToText(immediateOperand);
                        else
                            op = operandToText(immediateOperand) + "," + op;
                    else
                        if (direction == 1)
                            op = reg8ascii[reg] + "," + op;
                        else
                            op = op + "," + reg8ascii[reg];

                    break;
                case 2:
                    switch (RorM)
                    {
                        case 4: op = Scale_Index_Base + disp32.ToString();
                            break;
                        default:
                            op = "[" + reg32ascii[RorM] + disp32 + "]";
                            break;
                    }
                    if (immediateOperand != null)
                        op += "," + operandToText(immediateOperand);
                    break;
                case 3:
                    switch (RorM)
                    {
                        default:
                            if (wordSize == 0)
                            {
                                op = reg16ascii[RorM];
                            }
                            else
                            {
                                op = reg32ascii[RorM];
                            }
                            break;
                    }
                    if (immediateOperand != null)
                        op += "," + operandToText(immediateOperand);
                    else
                    {
                        string op2 = "";
                        if (wordSize == 0)
                        {
                            op2 = regsascii[reg];
                        }
                        else
                        {
                            op2 = reg32ascii[reg];
                        }
                        if (direction == 1)
                            op = op2 + "," + op;
                        else
                            op += "," + op2;
                    }
                    break;
            }
            #endregion
            return op;
        }
        /// <summary>
        /// Text line
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="positionInSource"></param>
        public CodeLine(long offset, long data, string text, long positionInSource)
        {
            this.PositionOfStructureInFile = offset + 0x400;

            this.dtaInt = data;
            this.text = text;
        }
        /// <summary>
        /// Line Text
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="text"></param>
        /// <param name="positionInSource"></param>
        public CodeLine(byte[] data, long offset, string text, long positionInSource)
        {
            this.PositionOfStructureInFile = offset + 0x400;
            LengthInFile = data.Length;
            this.data = data;
            this.text = text;
        }
        /// <summary>
        /// Main method : ParseFollows Calls
        /// </summary>
        /// <param name="position"></param>
        /// <param name="offsetToCode"></param>
        /// <param name="ins"></param>
        /// <param name="prefix"></param>
        /// <param name="FPU"></param>
        /// <param name="opCode"></param>
        /// <param name="hasModRm"></param>
        /// <param name="modRM"></param>
        /// <param name="Scale_Index_Base"></param>
        /// <param name="displacement"></param>
        /// <param name="immediateOperand"></param>
        /// <param name="function"></param>
        /// <param name="opCodeExtension"></param>
        /// <param name="regNum"></param>
        /// <param name="selectedEntry"></param>
        /// <param name="positionInSource"></param>
        public CodeLine(long position, long offsetToCode, Instruction ins, List<byte> prefix, byte FPU, byte opCode, bool hasModRm, byte modRM, byte Scale_Index_Base, byte[] displacement, byte[] immediateOperand, string function, byte opCodeExtension, int regNum, int selectedEntry, long positionInSource)
        {
            this.PositionOfStructureInFile = positionInSource + 0x400;
            string[] opers = new string[4];
            string pref = "";
            operandes = new List<string>();
            this.rightSyntax = 0;
            this.Floating_Point_Préfix = FPU;
            this.opCode = opCode;
            this.instruction = ins;
            this.selectedEntry = selectedEntry;
            int opcodeLoc = opCode;
            this.displacement = displacement;
            this.immediateOperand = immediateOperand;
            this.modRM = (byte)modRM;
            this.Scale_Index_Base = (byte)Scale_Index_Base;//http://wiki.osdev.org/X86_Instruction_Encoding
            string functionName = "";
            if (function != null)
            {
                functionName = function;
                if ((functionName == "") & (displacement != null))
                    if (displacement.Length == 4)
                        functionName = "0x" + BitConverter.ToInt32(displacement, 0).ToString("x8");
            }
            #region Prefixes
            if (FPU > 0)
                opcodeLoc = FPU;
            else
            {
                #region Find predefined operandes for one byte instructions
                int first = opcodeLoc >> 4;
                int sec = opcodeLoc & 0x0F;
                if ((first < 4) || (first == 0xE))
                {
                    switch (sec)
                    {
                        case 0x04:
                        case 0x0C:
                            opers[0] = "al";
                            operandes.Add("al");
                            break;
                        case 0x05:
                        case 0x0D:
                            opers[0] = "eax";
                            operandes.Add("eax");
                            break;
                    }
                }
                else if (first == 0xA)
                {
                    switch (sec)
                    {
                        case 0x00:
                        case 0x08:
                        case 0x0c:
                            opers[0] = "al";
                            operandes.Add("al");
                            break;
                        case 0x02:
                            opers[1] = "al";
                            operandes.Add("al");//second
                            break;
                        case 0x01:
                        case 0x09:
                        case 0x0D:
                            opers[0] = "eax";
                            operandes.Add("eax");
                            break;
                        case 0x03:
                            opers[1] = "eax";
                            operandes.Add("eax");//second
                            break;
                    }
                }
                else if (first == 0xC)
                {
                    switch (sec)
                    {
                        case 4:
                            opers[0] = "es";
                            operandes.Add("es");
                            break;
                        case 5:
                            opers[0] = "ds";
                            operandes.Add("ds");
                            break;
                        case 8:
                        case 9:
                            opers[0] = "ebp";
                            operandes.Add("ebp");
                            break;
                    }
                }
                switch (opcodeLoc)
                {
                    case 0x9E:
                    case 0x9F:
                        opers[0] = "ah";
                        operandes.Add("ah");
                        break;
                    case 0xD4:
                    case 0xD5:
                        opers[0] = "al";
                        opers[0] = "ah";
                        operandes.Add("al");
                        operandes.Add("ah");
                        break;
                    case 0xD6:
                    case 0xD7:
                        opers[0] = "al";
                        operandes.Add("al");
                        break;
                }

                operandes = new List<string>();
                foreach (string s in opers)
                    if(s!=null)
                    operandes.Add(s);
                if (operandes.Count > 0)
                {
                }
                #endregion
            }
            #endregion
        /*    #region Prefix
              if (prefix != null)
            {
                switch (prefix[0])
                {
                    case 0x2E: // CS segment override 
                        pref = "cs:";
                        break;
                    case 0x36: // SS segment override 
                        pref = "ss:";
                        break;
                    case 0x3E: // DS segment override 
                        pref = "ds:";
                        break;
                    case 0x26: // ES segment override 
                        pref = "es:";
                        break;
                    case 0x64: // FS segment override 
                        pref = "fs:";
                        break;
                    case 0x65: // GS segment override 
                        pref = "gs:";
                        break;
                    case 0x66: // Operand-size override prefix : 16 bits operand
                        break;
                    case 0x67: // Address-size override prefix 
                        break;
                    case 0xF0: // LOCK prefix 
                        break;
                    case 0xF2: // REPNE/REPNZ prefix 
                        break;
                    case 0xF3: // REP or REPE/REPZ prefix 
                        break;
                }
            }
            #endregion*/
            string[] reg32ascii = new string[] { "eax", "ecx", "edx", "ebx", "esp", "ebp", "esi", "edi" };
            int directionBit = (opcodeLoc & 2) >> 1;//Direction. 1 = Register is Destination, 0 = Register is source.
            int wordBit = (opcodeLoc & 1);//Operation size. 1 = Word, 0 = byte
            if (opCodeExtension == 0xff)
                if (regNum != -1)
                    operandes.Add(reg32ascii[regNum]);
            #region Parse ModRm
            if (isJumpOrCall)
            {
                if (displacement != null)
                    displaceInt = operandToInt(displacement);
                if (immediateOperand != null)
                    imOperandInt = operandToInt(immediateOperand);
            }
            if (position == 0x16D)
            {
            }
            if (hasModRm)
            {
                string aa = ParseModRm(modRM, Scale_Index_Base, wordBit, directionBit, displacement, immediateOperand, function);
                operandes.Add(aa);
            }
            else
            {
                if (immediateOperand != null)
                {
                    if (isJumpOrCall)
                    {
                        imOperandInt = (int)position + BinaryCode.Length / 2 + imOperandInt + (int)offsetToCode;
                        if ((BinaryCode.Length % 2) == 1)
                            imOperandInt++;
                    }
                    else
                    {
                        imOperandInt = operandToInt(immediateOperand);
                    }
                    operandes.Add(pref + imOperandInt.ToString("x8"));
                }
            }
            #endregion
            #region select op
            int rightSyntax = 0;
            if (instruction.entries[selectedEntry].syntax[rightSyntax].src.Count > 0)
            {
                string op = "";
                if (instruction.entries[selectedEntry].syntax[rightSyntax].src[0].nr != null)
                    op += instruction.entries[selectedEntry].syntax[rightSyntax].src[0].nr;
            }
            if (instruction.entries[selectedEntry].syntax[rightSyntax].dest.Count > 0)
            {
                string op = "";
                if (instruction.entries[selectedEntry].syntax[rightSyntax].dest[0].nr != null)
                    op += instruction.entries[selectedEntry].syntax[rightSyntax].dest[0].nr;
            }
            #endregion
            #region Opcode extension
            string[] regfascii = new string[] { "st(0)", "st(1)", "st(2)", "st(3)", "st(4)", "st(5)", "st(6)", "st(7)" };

            if (opCodeExtension != 0xff)
            {
                if (regNum != -1)
                    operandes.Add(regfascii[regNum]);
            }
            #endregion
        }
        /// <summary>
        ///  ParseStub
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="prefix"></param>
        /// <param name="FPU"></param>
        /// <param name="modRM"></param>
        /// <param name="displacement"></param>
        /// <param name="immediateOperand"></param>
        /// <param name="opCodeExtension"></param>
        /// <param name="position"></param>
        public CodeLine(Instruction ins, List<byte> prefix, byte FPU, byte modRM, byte[] displacement, byte[] immediateOperand, byte opCodeExtension, long position)
        {
            PositionOfStructureInFile = position;
            LengthInFile = 1;
            if (immediateOperand != null)
                LengthInFile += immediateOperand.Length;
            string[] opers = new string[4];
            string pref = "";
            operandes = new List<string>();
            this.rightSyntax = 0;
            this.Floating_Point_Préfix = FPU;
            this.opCode = ins.shortOpCode;// int.Parse(ins.opCode, System.Globalization.NumberStyles.HexNumber); //opCode; ;
            this.instruction = ins;
            int opcodeLoc = opCode;
            this.displacement = displacement;
            this.immediateOperand = immediateOperand;
            if (FPU > 0)
                opcodeLoc = FPU;
            else
            {
                #region Find predefined operandes for one byte instructions
                int first = opcodeLoc >> 4;
                int sec = opcodeLoc & 0x0F;
                if ((first < 4) || (first == 0xE))
                {
                    switch (sec)
                    {
                        case 0x04:
                        case 0x0C:
                            opers[0] = "al";
                            operandes.Add("al");
                            break;
                        case 0x03: // ????
                        case 0x05:
                        case 0x0D:
                            opers[0] = "eax";
                            operandes.Add("eax");
                            break;
                    }
                }
                else if (first == 0xA)
                {
                    switch (sec)
                    {
                        case 0x00:
                        case 0x08:
                        case 0x0c:
                            opers[0] = "al";
                            operandes.Add("al");
                            break;
                        case 0x02:
                            opers[1] = "al";
                            operandes.Add("al");//second
                            break;
                        case 0x01:
                        case 0x09:
                        case 0x0D:
                            opers[0] = "eax";
                            operandes.Add("eax");
                            break;
                        case 0x03:
                            opers[1] = "eax";
                            operandes.Add("eax");//second
                            break;
                    }
                }
                else if (first == 0xC)
                {
                    switch (sec)
                    {
                        case 4:
                            opers[0] = "es";//regcascii
                            operandes.Add("es");
                            break;
                        case 5:
                            opers[0] = "ds";
                            operandes.Add("ds");
                            break;
                        case 8:
                        case 9:
                            opers[0] = "ebp";
                            operandes.Add("ebp");
                            break;
                    }
                }
                switch (opcodeLoc)
                {
                    case 0x9E:
                    case 0x9F:
                        opers[0] = "ah";
                        operandes.Add("ah");
                        break;
                    case 0xD4:
                    case 0xD5:
                        opers[0] = "al";
                        opers[0] = "ah";
                        operandes.Add("al");
                        operandes.Add("ah");
                        break;
                    case 0xD6:
                    case 0xD7:
                        opers[0] = "al";
                        operandes.Add("al");
                        break;
                }
                if (ins.ModRMHasRegister)
                {
                    switch (modRM)
                    {
                        case 0xC0:
                            opers[1] = "eax";
                            break;
                        case 0xd0:
                            opers[1] = "ax";
                            break;
                    }
                }

                operandes = new List<string>();
                foreach (string s in opers)
                    operandes.Add(s);
                if (operandes.Count > 0)
                {
                }
                #endregion
            }
             int directionBit = (opcodeLoc & 2) >> 1;//Direction. 1 = Register is Destination, 0 = Register is source.
            int wordBit = (opcodeLoc & 1);//Operation size. 1 = Word, 0 = byte
            #region Parse ModRm
            short imOperandInt = 0;
            if (isJumpOrCall)
            {
                if (displacement != null)
                    displaceInt = operandToInt(displacement);
                if (immediateOperand != null)
                    imOperandInt = (short)operandToInt(immediateOperand);
            }
            if (immediateOperand != null)
            {
                if (isJumpOrCall)
                {
                    //      imOperandInt = (int)position + BinaryCode.Length / 2 + imOperandInt + (int)offsetToCode;
                    if ((BinaryCode.Length % 2) == 1)
                        imOperandInt++;
                }
                else
                {
                    if (immediateOperand.Length == 2)
                    {
                        imOperandInt = (short)operandToInt(immediateOperand);
                        operandes.Add(pref + imOperandInt.ToString("x4"));
                    }
                    else
                    {
                        imOperandInt = immediateOperand[0];
                        operandes.Add(pref + immediateOperand[0].ToString("x2"));
                    }
                }
            }
            #endregion
            #region select op
            int rightSyntax = 0;
            if (instruction.entries[selectedEntry].syntax[rightSyntax].src.Count > 0)
            {
                string op = "";
                if (instruction.entries[selectedEntry].syntax[rightSyntax].src[0].nr != null)
                    op += instruction.entries[selectedEntry].syntax[rightSyntax].src[0].nr;
            }
            if (instruction.entries[selectedEntry].syntax[rightSyntax].dest.Count > 0)
            {
                string op = "";
                if (instruction.entries[selectedEntry].syntax[rightSyntax].dest[0].nr != null)
                    op += instruction.entries[selectedEntry].syntax[rightSyntax].dest[0].nr;
            }
            #endregion
            #region Opcode extension
            if (opCodeExtension != 0xff)
            {
            }
            #endregion
        }
        private string operandToText(byte[] operand)
        {
            string op = "0x";
            switch (operand.Length)
            {
                case 1: return op += operand[0].ToString("x2");
                case 2: return op += (operand[1] * 256 + operand[0]).ToString("x2");
                case 4: return op += BitConverter.ToInt32(operand, 0).ToString("x8");
                default: return "";
            }
        }
        private int operandToInt(byte[] operand)
        {
            if (operand == null) return 0;
            switch (operand.Length)
            {
                case 1:
                    uint x = operand[0];
                    if ((x & 0x80) == 0x80)// negative
                    {
                        x += 0xFFFFFF00;
                    }
                    int y = (int)x;
                    return y;
                case 2:
                    uint z = (uint)(operand[1] * 256 + operand[0]);
                    if ((z & 0x8000) == 0x8000)// negative
                    {
                        z += 0xFFFF0000;
                    }
                    return (int)z;
                case 4: return BitConverter.ToInt32(operand, 0);
                default: return -1;
            }
        }
        public string BinaryCode
        {
            get
            {
                if (text != null)
                    LengthInFile = text.Length;

                string s = "";
                if (dtaInt != 0)
                {
                    s = dtaInt.ToString("x8");
                    LengthInFile = s.Length / 2;
                    return s;
                }
                if (data != null)
                {
                    if (data.Length < 100)
                    {
                        foreach (byte a in data)
                            s += a.ToString("x2");
                        LengthInFile = s.Length / 2;
                        return s;
                    }
                    else
                    {
                    }
                }
                List<byte> codes = new List<byte>();
                if (prefix != null)
                    foreach (byte b in prefix)
                        codes.Add(b);
                if (Floating_Point_Préfix != 0)
                    codes.Add((byte)Floating_Point_Préfix);
                codes.Add((byte)opCode);
                if (HasModRM)
                    codes.Add((byte)modRM);
                if (Scale_Index_Base != 0xff)
                    codes.Add((byte)Scale_Index_Base);
                if (displacement != null)
                    foreach (byte b in displacement)
                        codes.Add(b);
                if (immediateOperand != null)
                    foreach (byte b in immediateOperand)
                        codes.Add(b);
                foreach (int i in codes)
                    s += i.ToString("x2");
                LengthInFile = s.Length / 2;
                return s;
            }
        }
        public override string ToString()
        {
            if (text != null)
                return text;
            string c = "";// PositionOfStructureInFile.ToString("x8") + " : ";
            #region Prefix
            string pref = "";
            if (prefix != null)
            {
                foreach (byte p in prefix)
                {
                    switch (p)
                    {
                        case 0x2E: // CS segment override 
                            pref += "cs:";
                            break;
                        case 0x36: // SS segment override 
                            pref += "ss:";
                            break;
                        case 0x3E: // DS segment override 
                            pref += "ds:";
                            break;
                        case 0x26: // ES segment override 
                            pref += "es:";
                            break;
                        case 0x64: // FS segment override 
                            pref += "fs:";
                            break;
                        case 0x65: // GS segment override 
                            pref += "gs:";
                            break;
                        case 0x66: // Operand-size override prefix : 16 bits operand
                            break;
                        case 0x67: // Address-size override prefix 
                            break;
                        case 0xF0: // LOCK prefix 
                            pref += "LOCK";
                            break;
                        case 0xF2: // REPNE/REPNZ prefix 
                            pref += "REP";
                            break;
                        case 0xF3: // REP or REPE/REPZ prefix 
                            pref += "REP";
                            break;
                    }
                }
                c += pref;
            }
            #endregion

            if (instruction != null)
            {
                c += instruction.entries[selectedEntry].syntax[rightSyntax].mnemo + " ";
                for (int i = 0; i < operandes.Count; i++)
                {
                    if ((operandes[i] != null) & (operandes[i] != ""))
                    {
                        c += operandes[i];
                        if (i < operandes.Count - 1)
                            if (!operandes[i].Contains("s:"))
                                c += ", ";
                    }
                }
     /*           if (immediateOperand != null)
                {
                    c += " 0x";
                    foreach (byte b in immediateOperand)
                        c += b.ToString("x2");
                }*/
            }
            else
            {
                c += opCode.ToString("x2") + " " + modRM.ToString("x2");
            }
            return c.Trim();
        }
    }
    enum Direction {Register_is_Source, Register_is_Destination }
    enum WordSize { Word, Byte }
}

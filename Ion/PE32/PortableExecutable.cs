using System.Text;

namespace Ion.PE32;

public sealed class PortableExecutable
{
    private const int RelativeFileHeaderOffset = 0x4;

    private readonly PortableExecutableBinary _binary;

    private readonly Lazy<ImageDosHeader> _lazyDosHeader;
    private readonly Lazy<ImageFileHeader> _lazyFileHeader;
    private readonly Lazy<ImageOptionalHeader64> _lazyOptionalHeader;

    private PortableExecutable(PortableExecutableBinary binary)
    {
        _binary = binary;
        _lazyDosHeader = new Lazy<ImageDosHeader>(() => binary.Read<ImageDosHeader>(0));
        _lazyFileHeader = new Lazy<ImageFileHeader>(() => binary.Read<ImageFileHeader>(DosHeader.e_lfanew + RelativeFileHeaderOffset));
        _lazyOptionalHeader = new Lazy<ImageOptionalHeader64>(() => binary.Read<ImageOptionalHeader64>(OptionalHeaderOffset));
    }

    private int OptionalHeaderOffset => DosHeader.e_lfanew + RelativeFileHeaderOffset + ImageFileHeader.StructSize;

    public ImageDosHeader DosHeader => _lazyDosHeader.Value;
    public ImageFileHeader FileHeader => _lazyFileHeader.Value;
    public ImageOptionalHeader64 OptionalHeader => _lazyOptionalHeader.Value;
    public ImageDataDirectory this[ImageDirectoryEntry entry] => OptionalHeader.DataDirectory[(int)entry];
    public ImageSectionHeader this[ImageDataDirectory directory] => EnumerateSections().First(section => directory.VirtualAddress >= section.VirtualAddress && directory.VirtualAddress < section.VirtualAddress + section.VirtualSize);

    public ImageSectionHeader GetSection(string name) => EnumerateSections().First(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    public IReadOnlyCollection<ImageSectionHeader> GetSections() => EnumerateSections().ToArray();
    public T Read<T>(int offset) where T : struct => _binary.Read<T>(offset);
    public string ReadText(int offset) => _binary.Read(offset, Encoding.UTF8, 256);

    public T ReadDataTable<T>(ImageDirectoryEntry entry) where T : struct
    {
        var directory = this[entry];
        var section = this[directory];
        var offset = RvaToFileOffset(section, directory.VirtualAddress);

        return Read<T>(offset);
    }

    public IEnumerable<ImageSectionHeader> EnumerateSections()
    {
        var sectionsNumber = FileHeader.NumberOfSections;
        var offset = OptionalHeaderOffset + FileHeader.SizeOfOptionalHeader;

        for (var i = 0; i < sectionsNumber; i++)
        {
            yield return _binary.Read<ImageSectionHeader>(offset);
            offset += ImageSectionHeader.StructSize;
        }
    }

    public static int RvaToFileOffset(ImageSectionHeader section, int rva)
    {
        return section.PointerToRawData + (rva - section.VirtualAddress);
    }

    public static PortableExecutable Load(string path) => new PortableExecutable(PortableExecutableBinary.Load(path));
    public static PortableExecutable Load(byte[] data) => new PortableExecutable(PortableExecutableBinary.Load(data));
}
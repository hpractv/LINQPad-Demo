<Query Kind="Program">
  <NuGetReference>DotNetZip</NuGetReference>
  <Namespace>analyticsLibrary.dbObjects</Namespace>
  <Namespace>analyticsLibrary.library</Namespace>
  <Namespace>Ionic</Namespace>
  <Namespace>Ionic.Zip</Namespace>
  <Namespace>static UserQuery</Namespace>
</Query>

void Main()
{

}

public enum EntityType { CSV, File, Folder, DB, ZipFile, }

public interface IEntry
{
	int Index { get; }
}

public abstract class Entry : IEntry
{
	public int Index { get; }
	public Entry(int index) => this.Index = index;
}
public interface IEntryCollection<T> where T : IEntry
{
	EntityType Source { get; }
	IEnumerable<T> Entries { get; }
	IEntryCollection<IEntry> ToEntryCollection();
}
public abstract class EntryCollection<T> : IEntryCollection<T> where T : IEntry
{
	protected List<T> _entries = new List<T>();
	public abstract EntityType Source { get; }
	public abstract IEnumerable<T> Entries { get; }

	public EntryCollection(IEnumerable<T> entries)
	{
		_entries.AddRange(entries);
	}
	
	public IEntryCollection<IEntry> ToEntryCollection() => this as IEntryCollection<IEntry>;
}

public interface IFolderOutput
{
	IEnumerable<IFile> ToFileCollection();
}
public interface IFile : IEntry
{

}
public class File : Entry, IFile
{
	public File(int index) : base(index) { }
}
public class Folder : EntryCollection<IFile>, IFolderOutput, IZipFileOutput
{
	public override EntityType Source => EntityType.Folder;
	
	public override IEnumerable<IFile> Entries => throw new NotImplementedException();
	
	public Folder(IEnumerable<IFile> entries) : base(entries) { }

	public static Folder buildFromPath(string path)
		=> throw new NotImplementedException();
		
	public IEnumerable<IFile> ToFileCollection()
		=> throw new NotImplementedException();

	public IEnumerable<IZipFileEntry> ToZipEntryCollection()
		=> throw new NotImplementedException();
}

public interface IDbOupt
{
	IEnumerable<IRecord> ToRecordCollection();
}
public interface IRecord : IEntry
{
	object[] Values { get; }
}
public class Record : Entry, IRecord
{
	public object[] Values => throw new NotImplementedException();

	public Record(int index) : base(index) { }
}
public class DbQuery : EntryCollection<IRecord>, IDbOupt
{
	public override EntityType Source => EntityType.DB;

	public override IEnumerable<IRecord> Entries => _entries;

	public DbQuery(IEnumerable<IRecord> entries) : base(entries) { }

	public DbQuery buildFromSql(string sql)
		=>
		//var entries = new List<IRecord>();
		//return new DbQuery(entries);
		throw new NotImplementedException();

	public IEnumerable<IRecord> ToRecordCollection()
		=> throw new NotImplementedException();

}
public class CSV : EntryCollection<IRecord>, IDbOupt
{
	public override EntityType Source => EntityType.DB;

	public override IEnumerable<IRecord> Entries => _entries;

	public CSV(IEnumerable<IRecord> entries) : base(entries) { }

	public static CSV buildFromFile(string path)
		=>
		//var entries = new List<IRecord>();
		//return new DbQuery(entries);
		throw new NotImplementedException();

	public IEnumerable<IRecord> ToRecordCollection()
		=> throw new NotImplementedException();
}

public interface IZipFileOutput
{
	IEnumerable<IZipFileEntry> ToZipEntryCollection();
}
public interface IZipFileEntry : IEntry
{
	ZipEntry ZipEntry { get; }
}
public class ZipFileEntry : Entry, IZipFileEntry
{
	public ZipFileEntry(int index) : base(index) { }
	public ZipEntry ZipEntry { get; private set; }

	public ZipFileEntry buildFromZipEntry(int index, ZipEntry zipEntry)
		=> new ZipFileEntry(index) { ZipEntry = zipEntry };

}
public class ZipFileEntries : EntryCollection<IZipFileEntry>, IFolderOutput, IZipFileOutput
{
	public override EntityType Source => EntityType.ZipFile;
	public override IEnumerable<IZipFileEntry> Entries => _entries;

	public ZipFileEntries(IEnumerable<IZipFileEntry> entries) : base(entries) { }
	
	public static ZipFileEntries buildFromPath(string path)
		=> throw new NotImplementedException();

	public IEnumerable<IFile> ToFileCollection()
		=> throw new NotImplementedException();

	public IEnumerable<IZipFileEntry> ToZipEntryCollection()
		=> throw new NotImplementedException();
}

public static class extensions
{
	public static void ToCsv(this EntryCollection<IRecord> recordCollection, string path, char seperator = ',', bool useQuotes = false)
		=>
		//write the code to output the csv
		throw new NotImplementedException();

	private static void ToFolder<IFolderOutput>(this IFolderOutput fileCollection, string path, Func<IFile, IFile> fileTransformation = null)
		=>
		//write the code to output the csv
		throw new NotImplementedException();
		
	public static void ToZip(this IZipFileOutput zipFileEntryCollection, string path, Func<IZipFileEntry, IZipFileEntry> zipFileEntryTranfom = null , bool encrypt = false, EncryptionAlgorithm alogrithm = EncryptionAlgorithm.WinZipAes256, string key = null)
		=>
		//write the code to output the csv
		throw new NotImplementedException();
		
	public static void ToDb(this IDbOupt recordCollection)
		=>
		//write the code to output the csv
		throw new NotImplementedException();
}

public interface ITransporter
{
	void Transport(Action<IEntryCollection<IEntry>> entryCollection);
}

public class Transporter<T> : ITransporter where T : IEntry
{
	public void Transport(Action<IEntryCollection<IEntry>> entryCollection)
	{
		switch(entryCollection){
			case IDbOupt records:
				break;
			
			default:
				break;
		}
	}
}
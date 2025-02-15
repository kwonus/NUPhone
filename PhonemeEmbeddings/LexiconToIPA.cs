namespace PhonemeEmbeddings
{
    using System.Collections.Generic;
    using System.IO.Compression;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Xml.Linq;

    // reading from: https://github.com/open-dict-data/ipa-dict/blob/master/data/en_US.txt
    //
    public class LexiconIPA
	{
		private static LexiconIPA? SELF = null;
        public static LexiconIPA Instance 
		{
			get
			{
				if (SELF == null)
				{
					SELF = new LexiconIPA();
				}
				return SELF;
			}
		}

        public Dictionary<string, string[]> ipa_primatives { get; private set; }
        private char[] newlines = ['\n', '\r'];
        private LexiconIPA(string? home = null)
		{
			LexiconIPA.SELF = this;

            this.ipa_primatives = new();

            int first = 0;
            int last  = -1;
            int len   = en_US.Contents.Length;

            try
            {
                do
                {
                    last = len > first ? en_US.Contents.IndexOfAny(newlines, first + 1) : first;
                    if (last < 0)
                    {
                        break;
                    }
                    if (last > first)
                    {
                        string line = en_US.Contents.Substring(first, last - first).Trim();

                        if (line.Length > 0)
                        {
                            var parts = line.Split('/', 2);
                            if (parts.Length < 2)
                                continue;
                            parts[0] = parts[0].Trim();
                            if (parts[0].Length < 1)
                                continue;
                            var variants = parts[1].Split(',');
                            for (int v = 0; v < variants.Length; v++)
                            {
                                var variant = variants[v].Trim();
                                if (variant.EndsWith('/') && (variant.StartsWith('/') || (v == 0)))
                                {
                                    variants[v] = Features.Instance.NormalizeIntoNUPhone(variant.Replace("/", ""));
                                }
                                else
                                {
                                    System.Console.WriteLine(line);
                                    goto bad_record;
                                }
                            }
                            this.ipa_primatives[parts[0]] = variants;
                        bad_record:
                            ;
                        }              
                    }
                    for (first = last + 1; first < len && (en_US.Contents[first] == '\n' || en_US.Contents[first] == '\r'); first++)
                        ;

                }   while (first < len);
			}
			catch
			{
				System.Console.WriteLine("IO Error encountered");
                this.ipa_primatives.Clear();
            }
		}
        // obsolete method is likely just cruft
        public static string GetMicrosoftStoreFolder(string name)
        {
            string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AV-Bible");
            if (!Directory.Exists(appdata))
                Directory.CreateDirectory(appdata);
            string folder = Path.Combine(appdata, name);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }
        private static string OBSOLETE_DownloadFiles(string name, byte timeout_seconds = 20)
        {
            var task = OBSOLETE_DownloadFilesAsync(name);

            task.Wait(timeout_seconds * 1024);
            if (task.IsCompleted)
                return task.Result;

            return string.Empty;
        }
        private static async Task<string> OBSOLETE_DownloadFilesAsync(string name)
        {
            string url = "https://github.com/kwonus/AV-Bible/raw/refs/heads/main/Release-9.25.2";
            string zip = url + "/" + name + ".zip";

            string path = Path.GetDirectoryName(GetMicrosoftStoreFolder(name));

            using (HttpClient client = new HttpClient())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Download the zip file into the memory stream
                byte[] zipData = await client.GetByteArrayAsync(url);
                await memoryStream.WriteAsync(zipData, 0, zipData.Length);
                memoryStream.Position = 0; // Reset the stream position to the beginning

                // Extract the contents of the zip file directly from the memory stream
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(path, entry.FullName);

                        // Ensure the directory structure is in place
                        if (entry.FullName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(destinationPath);
                            continue;
                        }

                        // Extract the file
                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                        {
                            await entryStream.CopyToAsync(fileStream);
                        }
                    }
                }
            }
            return path;
        }
        // Test for folder only when file is null (otherwise, file must exist within folder)
        private static string OBSOLETE_GetProgramDirDefault(string collection, string? file = null)
        {
            string dir = GetMicrosoftStoreFolder(collection);
            if (System.IO.Directory.Exists(dir))
            {
                if (file == null)
                    return dir;
                string item = Path.Combine(collection, file);
                if (!System.IO.File.Exists(item))
                    OBSOLETE_DownloadFiles(item);
                if (System.IO.File.Exists(item))
                    return item;
            }
            return String.Empty;
        }
    }
}
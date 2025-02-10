namespace PhonemeEmbeddings
{
    using System.Collections.Generic;

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
		private LexiconIPA(string? home = null)
		{
			LexiconIPA.SELF = this;

            this.ipa_primatives = new();

            string path = ENUS.Replace('\\', '/');

            try
            {
				if (File.Exists(path))
				{
					using (StreamReader file = new StreamReader(path))
					{
						for (string? line = file.ReadLine(); line != null; line = file.ReadLine())
						{
							if (string.IsNullOrWhiteSpace(line))
								continue;
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
				}
			}
			catch
			{
				System.Console.WriteLine("IO Error encountered");
                this.ipa_primatives.Clear();
            }
		}
        private static string _enus = null;
        private static string ENUS
        {
            get
            {
                if (_enus != null)
                    return _enus;

                string cwd = Directory.GetCurrentDirectory();
                for (string en = Path.Combine(cwd, "Digital-AV", "en_us.txt"); en.Length > @"X:\Digital-AV\en_us.txt".Length; en = Path.Combine(cwd, "Digital-AV", "en_us.txt"))
                {
                    if (System.IO.File.Exists(en))
                    {
                        _enus = en;
                        return en;
                    }
                    var parent = Directory.GetParent(cwd);
                    if (parent == null)
                        break;
                    cwd = parent.FullName;
                }
                cwd = Directory.GetCurrentDirectory();
                for (string en = Path.Combine(cwd, "AV-Bible", "Digital-AV", "en_us.txt"); en.Length > @"X:\Digital-AV\en_us.txt".Length; en = Path.Combine(cwd, "AV-Bible", "Digital-AV", "en_us.txt"))
                {
                    if (System.IO.File.Exists(en))
                    {
                        _enus = en;
                        return en;
                    }
                    var parent = Directory.GetParent(cwd);
                    if (parent == null)
                        break;
                    cwd = parent.FullName;
                }
                cwd = Directory.GetCurrentDirectory();
                for (string en = Path.Combine(cwd, "AVBible", "Digital-AV", "en_us.txt"); en.Length > @"X:\Digital-AV\en_us.txt".Length; en = Path.Combine(cwd, "AVBible", "Digital-AV", "en_us.txt"))
                {
                    if (System.IO.File.Exists(en))
                    {
                        _enus = en;
                        return en;
                    }
                    var parent = Directory.GetParent(cwd);
                    if (parent == null)
                        break;
                    cwd = parent.FullName;
                }
                cwd = Directory.GetCurrentDirectory();
                for (string en = Path.Combine(cwd, "en_us.txt"); en.Length > @"X:\en_us.txt".Length; en = Path.Combine(cwd, "en_us.txt"))
                {
                    if (System.IO.File.Exists(en))
                    {
                        _enus = en;
                        return en;
                    }
                    var parent = Directory.GetParent(cwd);
                    if (parent == null)
                        break;
                    cwd = parent.FullName;
                }
                _enus = GetProgramDirDefault("Digital-AV", "en_us.txt");
                return _enus;
            }
        }
        private static string GetProgramDirDefault(string collection, string file)
        {
            var folders = new string[] { "AV-Bible", "Digital-AV", "DigitalAV" };

            foreach (string folder in folders)
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string candidate = Path.Combine(appdata, "Programs", folder, collection, file);
                if (System.IO.File.Exists(candidate))
                {
                    return candidate;
                }
            }

            var roots = new string[0];
            try
            {
                roots = new string[] { Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()), @"C:\", @"D:\", @"E:\", @"F:\" };
            }
            catch
            {
                roots = new string[] { @"C:\", @"D:\", @"E:\", @"F:\" };
            }

            foreach (string root in roots)
            {
                foreach (string folder in folders)
                {
                    string candidate = Path.Combine(root, "Program Files", folder, collection, file);
                    if (System.IO.File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }

            foreach (string root in roots)
            {
                foreach (string folder in folders)
                {
                    string candidate = Path.Combine(root, "Program Files (x86)", folder, collection, file);
                    if (System.IO.File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }
            return String.Empty;
        }
    }
}
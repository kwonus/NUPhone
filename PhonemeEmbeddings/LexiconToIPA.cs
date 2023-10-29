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

            string path = Path.Combine((home != null ? home : "C:\\src\\NUPhone\\PhonemeEmbeddings"), "en_US.txt").Replace('\\', '/');
			try
			{
				if (File.Exists(path))
				{
					using (StreamReader file = new StreamReader(path))
					{
						for (string line = file.ReadLine(); line != null; line = file.ReadLine())
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
	}
}
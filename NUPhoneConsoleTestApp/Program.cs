namespace NUPhoneConsoleTestApp
{
    using PhonemeEmbeddings;
    using System.Text;

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var greetings = new NUPhoneGen("hello");
            var world = new NUPhoneGen("wood");
            var whirl = new NUPhoneGen("would");

            Console.WriteLine(greetings.Phonetic + " " + world.Phonetic);

            UInt16? diff = world.Compare(whirl, 70);

            if (diff.HasValue)
            {
                var score = (diff.Value / 100).ToString();
                diff %= 100;
                if (diff != 0)
                {
                    score += ".";
                    score += (diff < 10) ? ("0" + diff.ToString()) : diff.ToString();
                }
                Console.WriteLine(world.Word + " differs from " + whirl.Word + " with a similarity score of " + score + "%");
            }
            else
            {
                Console.WriteLine(world.Word + " is not compared with " + whirl.Word + " because it is below threshold");
            }
        }
    }
}
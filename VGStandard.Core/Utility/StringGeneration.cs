using System.Text;

namespace VGStandard.Core.Utility;

public static class StringGeneration
{
    public static string LoremIpsum(int wordCount)
    {

        var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                            "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                            "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat", };
        var rand = new Random();
        StringBuilder result = new StringBuilder();

        for (int w = 0; w < wordCount; w++)
        {
            if (w > 0) { result.Append(" "); }
            result.Append(words[rand.Next(words.Length)]);
        }

        return result.ToString();
    }
}

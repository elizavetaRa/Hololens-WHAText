using System.Collections.Generic;

namespace AssemblyCSharpWSA.Scripts.Utils
{
    class RecipeResult
    {
        public class Attribution
        {
            public string html { get; set; }
            public string url { get; set; }
            public string text { get; set; }
            public string logo { get; set; }
        }

        public class Flavors
        {
            public double Salty { get; set; }
            public double Meaty { get; set; }
            public int Piquant { get; set; }
            public double Bitter { get; set; }
            public double Sour { get; set; }
            public double Sweet { get; set; }
        }

        public class Unit
        {
            public string name { get; set; }
            public string abbreviation { get; set; }
            public string plural { get; set; }
            public string pluralAbbreviation { get; set; }
        }

        public class NutritionEstimate
        {
            public string attribute { get; set; }
            public string description { get; set; }
            public double value { get; set; }
            public Unit unit { get; set; }
        }

        public class Image
        {
            public string hostedLargeUrl { get; set; }
            public string hostedSmallUrl { get; set; }
        }

        public class Attributes
        {
            public List<string> holiday { get; set; }
            public List<string> cuisine { get; set; }
        }

        public class Source
        {
            public string sourceRecipeUrl { get; set; }
            public string sourceSiteUrl { get; set; }
            public string sourceDisplayName { get; set; }
        }

        public class RootObject
        {
            public Attribution attribution { get; set; }
            public List<string> ingredientLines { get; set; }
            public Flavors flavors { get; set; }
            public List<NutritionEstimate> nutritionEstimates { get; set; }
            public List<Image> images { get; set; }
            public string name { get; set; }
            public string yield { get; set; }
            public string totalTime { get; set; }
            public Attributes attributes { get; set; }
            public int totalTimeInSeconds { get; set; }
            public double rating { get; set; }
            public int numberOfServings { get; set; }
            public Source source { get; set; }
            public string id { get; set; }
        }
    }
}

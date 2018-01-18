using System.Collections.Generic;

namespace AssemblyCSharpWSA.Scripts.Utils
{
    public class RecipeResult
    {
        public class Attribution
        {
            public string html { get; set; }
            public string url { get; set; }
            public string text { get; set; }
            public string logo { get; set; }
        }

        public class FacetCounts
        {
        }

        public class Attributes
        {
            public List<string> course { get; set; }
            public List<string> cuisine { get; set; }
            public List<string> holiday { get; set; }
        }

        public class Flavors
        {
            public double salty { get; set; }
            public double sour { get; set; }
            public double sweet { get; set; }
            public double bitter { get; set; }
            public double meaty { get; set; }
            public double piquant { get; set; }
        }

        public class Match
        {
            public Attributes attributes { get; set; }
            public Flavors flavors { get; set; }
            public double rating { get; set; }
            public string id { get; set; }
            public List<object> smallImageUrls { get; set; }
            public string sourceDisplayName { get; set; }
            public int totalTimeInSeconds { get; set; }
            public List<string> ingredients { get; set; }
            public string recipeName { get; set; }
        }

        public class FlavorPiquant
        {
            public double min { get; set; }
            public int max { get; set; }
        }

        public class AttributeRanges
        {
            public FlavorPiquant flavorPiquant { get; set; }
    }

    public class FAT
    {
        public int min { get; set; }
        public int max { get; set; }
    }

    public class NutritionRestrictions
    {
        public FAT FAT { get; set; }
    }

    public class Criteria
    {
        public int maxResults { get; set; }
        public List<string> excludedIngredients { get; set; }
        public List<object> excludedAttributes { get; set; }
        public List<string> allowedIngredients { get; set; }
        public AttributeRanges attributeRanges { get; set; }
        public NutritionRestrictions nutritionRestrictions { get; set; }
        public List<string> allowedDiets { get; set; }
        public int resultsToSkip { get; set; }
        public bool requirePictures { get; set; }
        public List<object> facetFields { get; set; }
        public List<string> terms { get; set; }
        public List<string> allowedAttributes { get; set; }
    }

    public class RootObject
    {
        public Attribution attribution { get; set; }
        public int totalMatchCount { get; set; }
        public FacetCounts facetCounts { get; set; }
        public List<Match> matches { get; set; }
        public Criteria criteria { get; set; }
    }
}
}

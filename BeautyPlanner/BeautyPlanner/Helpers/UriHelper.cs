using System.Text.RegularExpressions;

namespace BeautyPlanner.Helpers
{
    public class UriHelper
    {
        public static bool IsLink(string testString)
        {
            var regex = new Regex(@"https?://");
            
            if (regex.Matches(testString).Count > 0)
            {
                return true;
            }
            return false;
        }

        
    }
}

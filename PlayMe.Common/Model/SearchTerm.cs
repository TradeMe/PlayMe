using System;

namespace PlayMe.Common.Model
{
    public class SearchTerm : DataObject
    {
        private string term;

        public SearchTerm(string searchTerm) 
        {
            Term = searchTerm;
            SearchCount = 1;
            LastDateSearched = DateTime.Now;
        }

        /// <summary>
        /// Always lower case
        /// </summary>
        public string Term
        {
            get
            {
                return term;
            }
            set 
            { 
                term = value.ToLower();
            }
        }
        public int SearchCount { get; set; }
        public DateTime LastDateSearched { get; set; }
    }
}

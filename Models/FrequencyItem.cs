
public class FrequencyItem {
    public string? subreddit { get; set; }
    public int? timesContributed { get; set; }
    public DateTime? datetime { get; set; }
    public string? recency { get; set; }
    public string? permalink { get; set; }
    public string? type { get; set; }
    public int? postCount { get; set; }
    public int? commentCount { get; set; }
    List<Listing> subredditHistory;

    public FrequencyItem(string s) {
        this.subreddit = s; 
        this.timesContributed = 1;
        this.postCount = 0;
        this.commentCount = 0;
    }

    public void addListingToHistory(Listing listing) {
        if (this.subredditHistory != null) {
            this.subredditHistory.Add(listing);
        } else {
            this.subredditHistory = new List<Listing>();
            this.subredditHistory.Add(listing);
        }
    }

    public List<Listing> getSubredditHistory() {
        if (this.subredditHistory != null) {
            return this.subredditHistory;
        } else {
            return new List<Listing>();
        }
        
    }
    
    public string toString() {
        string toString = "Subreddit:     " + this.subreddit+ "\nContributions: " + this.timesContributed + "\n";
        return toString;
    }
}
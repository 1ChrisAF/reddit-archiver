
public class FrequencyItem {
    public string? subreddit { get; set; }
    public int? timesContributed { get; set; }
    public DateTime? datetime { get; set; }
    public string? recency { get; set; }
    public string? permalink { get; set; }
    public string? type { get; set; }
    public int? postCount { get; set; }
    public int? commentCount { get; set; }

    public FrequencyItem(string s) {
        this.subreddit = s; 
        this.timesContributed = 1;
        this.postCount = 0;
        this.commentCount = 0;
    }
    
    public string toString() {
        string toString = "Subreddit:     " + this.subreddit+ "\nContributions: " + this.timesContributed + "\n";
        return toString;
    }
}
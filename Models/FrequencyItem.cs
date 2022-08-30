
public class FrequencyItem {
    public string? subreddit { get; set; }
    public int? timesContributed { get; set; }
    public DateTime? datetime { get; set;}

    public FrequencyItem(string s) {
        this.subreddit = s; 
        this.timesContributed = 1;
    }
    
    public string toString() {
        string toString = "Subreddit:     " + this.subreddit+ "\nContributions: " + this.timesContributed + "\n";
        return toString;
    }
}
public class Listing {
    public string? data_type { get; set; }
    public string? data_subreddit { get; set; } 
    public string? data_author { get; set; }
    public string? data_permalink { get; set; }
    public DateTime datetime {get; set;}

    public Listing(string t, string s, string a, string p, DateTime d) {
        this.data_type = t;
        this.data_subreddit = s;
        this.data_author = a;
        this.data_permalink = p;
        this.datetime = d;
    }

    public string toString() {
        string returnString = $"data-type:      {this.data_type}\ndata-subreddit: {this.data_subreddit}\ndata-author:    {this.data_author}\ndata-permalink: {this.data_permalink}\ndatetime:       {this.datetime}\n";
        return returnString;
    }
}
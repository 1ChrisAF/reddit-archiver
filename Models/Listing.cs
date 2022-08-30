public class Listing {
    public string? data_type { get; set; }
    public string? data_subreddit { get; set; } 
    public string? data_author { get; set; }
    public string? data_permalink { get; set; }
    public DateTime datetime {get; set;}

    public Listing(string data_type, string data_subreddit, string data_author, string data_permalink, DateTime datetime) {
        this.data_type = data_type;
        this.data_subreddit = data_subreddit;
        this.data_author = data_author;
        this.data_permalink = data_permalink;
        this.datetime = datetime;
    }

    public string toString() {
        string returnString = $"data-type:      {this.data_type}\ndata-subreddit: {this.data_subreddit}\ndata-author:    {this.data_author}\ndata-permalink: {this.data_permalink}\ndatetime:       {this.datetime}\n";
        return returnString;
    }
}
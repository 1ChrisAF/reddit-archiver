using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using archiver_net.Models;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace archiver_net.Controllers;

public class UserHistoryController : Controller {
    private readonly ILogger<HomeController> _logger;

    public UserHistoryController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Index(string username) {
        List<Listing> info = parseThroughProfile(username);
        string infoJSON = JsonSerializer.Serialize(info);
        TempData["fullInfo"] = infoJSON;
        List<List<FrequencyItem>> sortedInfo = new List<List<FrequencyItem>>();
        sortedInfo.Add(getContributionCounts(info));
        sortedInfo.Add(getContributionRecent(info));
        sortedInfo.Add(getContributionTypes(info));
        sortedInfo.Add(getHistoryBySubreddit(info));
        ViewBag.title = username;
        ViewBag.info = sortedInfo;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /*** UTILITY METHODS FOR GETTING/SORTING USER HISTORY ***/

    /*** RETRIEVE AND PARSE USER HISTORY ***/

    private List<Listing> parseThroughProfile(string username) {
        // List to be returned
        List<Listing> listingList = new List<Listing>();
        // Temp list for each profile page
        List<Listing> newListings = new List<Listing>();
        // For url of page of user profile, initially just username
        string url = "https://old.reddit.com/user/" + username;
        Task<String> htmlTask;
        string pageContents;
        MatchCollection listings;
        do {
            // Call getProfileHTML and pull HTML from relevant
            // user profile page
            htmlTask = getProfileHTML(url);
            // Read Task into string
            pageContents = htmlTask.Result;
            // Collect all user posts and comments on page
            listings = getListings(pageContents);
            // Parse info from collected posts/comments into List of
            // Listing objs from this page
            newListings = parseListings(listings);
            foreach (Listing listing in newListings) {
                // Push Listing obs from THIS page into list of Listing
                // objs for ALL pages
                listingList.Add(listing);
            }
            // Assign the url from the 'next button' and loop through the next page
            url = getNextButtonLink(pageContents);
            // If url is empty, we are on the last page and may break
        } while(url != "");
        // Return all Listing objs from parse
        return listingList;
    }
    private async Task<String> getProfileHTML(string url) {
        HttpClient client = new HttpClient();
        Console.WriteLine("Checking {0}...", url);
        var response = await client.GetAsync(url);
        var pageContents = await response.Content.ReadAsStringAsync();
        return pageContents;
    }
    private MatchCollection getListings(string pageContents) {
        // Regex to pull all user comments and posts from profile page
        Regex regex = new Regex("(class=\"[^\"]*?thing[^\"]*?\")[\\s\\S]*?(class=\"child\")");
        MatchCollection listings = regex.Matches(pageContents);
        return listings;
    }
    private List<Listing> parseListings(MatchCollection listings) {
        string data_type;
        string data_subreddit;
        string data_author;
        string data_permalink;
        DateTime datetime;
        List<Listing> listingList = new List<Listing>();
        Regex regex;
        foreach (Match listing in listings) {
            string? listingValue = listing.Value;
            // Regex for data_type
            regex = new Regex("(?<=data-type=\")\\S*(?=\")");
            data_type = regex.Match(listingValue).Value;
            if(data_type == "link") {
                data_type = "post";
            }
            // Regex for data_subreddit
            regex = new Regex("(?<=data-subreddit=\")\\S*(?=\")");
            data_subreddit = regex.Match(listingValue).Value;
            // Regex for data_author
            regex = new Regex("(?<=data-author=\")\\S*(?=\")");
            data_author = regex.Match(listingValue).Value;
            // Regex for data_permalink
            regex = new Regex("(?<=data-permalink=\")\\S*(?=\")");
            data_permalink = regex.Match(listingValue).Value;
            // Regex for datetime
            regex = new Regex("(?<=datetime=\")\\S*(?=\")");
            datetime = DateTime.Parse(regex.Match(listingValue).Value);
            Listing newListing = new Listing(data_type, data_subreddit, data_author, data_permalink, datetime);
            listingList.Add(newListing);
        }
        return listingList;
    }
    private string getNextButtonLink(string pageContents) {
        string link = "";
        // Grab span tag with URL
        Regex regex = new Regex("<span class=\"[^\"]*?next-button[^\"]*?\">(.*?)<\\/span>");
        link = regex.Match(pageContents).Value;
        link = link.Replace("&amp;", "&");
        // Grab URL from span tag
        regex = new Regex("(?<=href=\")\\S*(?=\")");
        link = regex.Match(link).Value;
        link = link.Replace("&amp;", "&");
        return link;
    }

    /*** SORT USER HISTORY ***/

    // Return list of subreddits with related sums of user contributions of all types
    List<FrequencyItem> getContributionCounts(List<Listing> listingList) {
        List<FrequencyItem> frequencyList = new List<FrequencyItem>();
        List<String> subreddits = new List<String>();
        foreach (Listing listing in listingList) {
            string currentSubreddit = listing.data_subreddit;
            if (!subreddits.Contains(currentSubreddit)) {
                subreddits.Add(currentSubreddit);
                FrequencyItem newItem = new FrequencyItem(currentSubreddit);
                frequencyList.Add(newItem);
            } else {
                frequencyList[subreddits.IndexOf(currentSubreddit)].timesContributed += 1;
            }
        }
        List<FrequencyItem> sortedFrequencyListDescending = frequencyList.OrderByDescending(i => i.timesContributed).ToList();
        return sortedFrequencyListDescending;
    }
    // Return list of subreddits sorted by user contribution recency
    List<FrequencyItem> getContributionRecent(List<Listing> listingList) {
        DateTime veryRecently = DateTime.Now.AddDays(-7);
        DateTime recently = DateTime.Now.AddMonths(-1);
        List<FrequencyItem> recentsList = new List<FrequencyItem>();
        List<string> subreddits = new List<string>();
        foreach (Listing listing in listingList) {
            if (!subreddits.Contains(listing.data_subreddit)) {
                subreddits.Add(listing.data_subreddit);
                FrequencyItem newItem = new FrequencyItem(listing.data_subreddit);
                newItem.datetime = listing.datetime;
                newItem.permalink = listing.data_permalink;
                if (newItem.datetime > veryRecently) {
                    newItem.recency = "Very Recently";
                } else if (newItem.datetime > recently) {
                    newItem.recency = "Recently";
                } else {
                    newItem.recency = "Historically";
                }
                newItem.type = listing.data_type;
                recentsList.Add(newItem);
            } else {
                if (recentsList[subreddits.IndexOf(listing.data_subreddit)].datetime < listing.datetime) {
                    FrequencyItem newItem = new FrequencyItem(listing.data_subreddit);
                    newItem.datetime = listing.datetime;
                    newItem.permalink = listing.data_permalink;
                    newItem.type = listing.data_type;
                    recentsList[subreddits.IndexOf(listing.data_subreddit)] = newItem;
                }
            }
        }
        List<FrequencyItem> sortedRecentsListDescending = recentsList.OrderByDescending(i => i.datetime).ToList();
        return sortedRecentsListDescending;
    }
    // Return list of subreddits with contribution sum broken down by type
    List<FrequencyItem> getContributionTypes(List<Listing> listingList) {
        List<FrequencyItem> typeList = new List<FrequencyItem>();
        List<string> subreddits = new List<string>();
        List<FrequencyItem> sortedTypeListAscending;
        foreach (Listing listing in listingList) {
            if (!subreddits.Contains(listing.data_subreddit)) {
                subreddits.Add(listing.data_subreddit);
                FrequencyItem newItem = new FrequencyItem(listing.data_subreddit);
                if (listing.data_type == "post") {
                    newItem.postCount += 1;
                } else if (listing.data_type == "comment") {
                    newItem.commentCount += 1;
                }
                typeList.Add(newItem);
            } else {
                if (listing.data_type == "post") {
                    typeList[subreddits.IndexOf(listing.data_subreddit)].postCount += 1;
                } else if (listing.data_type == "comment") {
                    typeList[subreddits.IndexOf(listing.data_subreddit)].commentCount += 1;
                }
            }
        }
        sortedTypeListAscending = typeList.OrderBy(i => i.subreddit).ToList();
        return sortedTypeListAscending;
    }
    List<FrequencyItem> getHistoryBySubreddit(List<Listing> listingList) {
        List<FrequencyItem> historiesBySubreddit = new List<FrequencyItem>();
        List<string> subreddits = new List<string>();
        foreach(Listing listing in listingList) {
            if (!subreddits.Contains(listing.data_subreddit)) {
                subreddits.Add(listing.data_subreddit);
                FrequencyItem newItem = new FrequencyItem(listing.data_subreddit);
                newItem.addListingToHistory(listing);
                historiesBySubreddit.Add(newItem);
            } else {
                historiesBySubreddit[subreddits.IndexOf(listing.data_subreddit)].addListingToHistory(listing);
            }
        }
        return historiesBySubreddit;
    }
}
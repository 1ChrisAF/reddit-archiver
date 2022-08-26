export class RawListItem {
    subreddit = '';
    type = '';
    date;
    link;
    title = '';

    constructor(a, b, c, d, e) {
        this.subreddit = a;
        this.type = b;
        this.date = c;
        this.link = d;
        this.title = e;
    }
}
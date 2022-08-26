export class FrequencyListNode {
    subredditName = '';
    frequency = 0;

    constructor(subreddit) {
        this.subredditName = subreddit;
        this.frequency = 1;
    }

    updateFrequency() {
        this.frequency += 1;    
    }

    to_String() {
        return 'Subreddit: ' + this.subredditName + '\nCount: ' + this.frequency + '\n';
    }
}
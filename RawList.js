import { FrequencyList } from './FrequencyList.js';
import { RawListItem } from './RawListItem.js';
export class RawList {
    rawItems = [];
    rawItemsJSON = [];

    add(subreddit, type, date, link, title) {
        this.rawItems.push(subreddit);
        let newItem = new RawListItem(subreddit, type, date, link, title);
        this.rawItemsJSON.push(newItem);
    }

    getLength() {
        this.rawItems.length;
    }

    getRawList() {
        return this.rawItems;
    }

    getRawItemsJSON() {
        return this.rawItemsJSON;
    }

    toFrequencyList() {
        let frequencyList = new FrequencyList();
        for (let item of this.rawItems) {
            frequencyList.addSubreddit(item);
        }
        return frequencyList;
    }
}
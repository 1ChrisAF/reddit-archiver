import { FrequencyListNode } from './FrequencyListNode.js';

export class FrequencyList {
    indexTracker = [];
    frequencies = [];
    first;

    addSubreddit(subreddit) {
        if (this.contains(subreddit)) {
            this.frequencies[this.getIndexOf(subreddit)].updateFrequency();  
        } else {
            let newSub = new FrequencyListNode(subreddit);
            this.frequencies.push(newSub);
            this.indexTracker.push(subreddit);
        }
    }

    contains(subreddit) {
        if (this.getIndexOf(subreddit) === -1) {
            return false;
        } else {
            return true;
        }
    }

    getIndexOf(subreddit) {
        let index = this.indexTracker.indexOf(subreddit);
        return index;
    }

    getLength() {
        return this.frequencies.length;
    }
    
    swap(items, leftIndex, rightIndex){
        let temp = items[leftIndex];
        items[leftIndex] = items[rightIndex];
        items[rightIndex] = temp;
    }

    partition(items, left, right) {
        let pivot   = items[Math.floor((right + left) / 2)].frequency, 
            i       = left, 
            j       = right; 
        while (i <= j) {
            while (items[i].frequency < pivot) {
                i++;
            }
            while (items[j].frequency > pivot) {
                j--;
            }
            if (i <= j) {
                this.swap(items, i, j); 
                i++;
                j--;
            }
        }
        return i;
    }
    
    quickSort(items, left, right) {
        let index;
        if (items.length > 1) {
            index = this.partition(items, left, right); 
            if (left < index - 1) { 
                this.quickSort(items, left, index - 1);
            }
            if (index < right) {
                this.quickSort(items, index, right);
            }
        }
        return items;
    }

    sortListByFrequency() {
        // Sorts in ASCENDING order
        this.frequencies = this.quickSort(this.frequencies, 0, this.frequencies.length - 1);
    }

    to_String() {
        let fullString = '';
        for (let node of this.frequencies) {
            fullString += node.to_String();
        }
        return fullString;
    }
}